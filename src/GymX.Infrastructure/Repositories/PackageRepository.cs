using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Entities.Packages;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class PackageRepository(ApplicationDbContext context) 
    : GenericRepository<Package>(context), IPackageRepository
{
    public Task<Package?> GetByCodeAsync(string packageCode, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(p => p.PackageCode == packageCode && !p.IsDeleted, cancellationToken);

    public Task<bool> ExistsCodeAsync(string packageCode, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(p => p.PackageCode == packageCode && !p.IsDeleted, cancellationToken);

    public Task<bool> IsPackageInUseAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
        return Context.MemberPackages.AnyAsync(mp =>
            mp.PackageId == packageId &&
            mp.Status == MemberPackageStatus.Active &&
            mp.EndDate >= today,
            cancellationToken);
    }

    public Task<bool> HasEverBeenPurchasedAsync(Guid packageId, CancellationToken cancellationToken = default)
        => Context.MemberPackages.AnyAsync(mp => mp.PackageId == packageId, cancellationToken);

    public Task<int> CountActiveMembersAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
        return Context.MemberPackages.CountAsync(mp =>
            mp.PackageId == packageId &&
            mp.Status == MemberPackageStatus.Active &&
            mp.EndDate >= today,
            cancellationToken);
    }

    public async Task<PagedResult<PackageSummaryDto>> GetPackagesPagedAsync(
        string? search,
        string? durationFilter,
        string? status,
        string? sortBy,
        Guid? categoryId,
        int pageIndex,
        int pageSize,
        bool enforceEffectiveDates = false,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
        var query = DbSet.AsNoTracking().Where(p => !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(p =>
                EF.Functions.ILike(p.PackageCode, $"%{keyword}%") ||
                EF.Functions.ILike(p.Name, $"%{keyword}%"));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (enforceEffectiveDates)
        {
            query = query.Where(p => p.EffectiveStartDate <= today);
        }

        if (!string.IsNullOrWhiteSpace(durationFilter))
        {
            var filter = durationFilter.Trim().ToLower();
            query = filter switch
            {
                "<=1" or "1" => query.Where(p => p.DurationMonths <= 1 || p.DurationDays <= 31),
                "<=6" or "6" => query.Where(p => p.DurationMonths <= 6 || p.DurationDays <= 186),
                "<=12" or "12" => query.Where(p => p.DurationMonths <= 12 || p.DurationDays <= 366),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(status) && status.Trim().ToUpperInvariant() != "ALL")
        {
            var stat = status.Trim().ToUpperInvariant();
            if (stat == PackageStatus.Active)
            {
                query = query.Where(p => p.Status == PackageStatus.Active || (p.Status == PackageStatus.Pending && p.EffectiveStartDate <= today));
            }
            else if (stat == PackageStatus.Pending)
            {
                query = query.Where(p => p.Status == PackageStatus.Pending && p.EffectiveStartDate > today);
            }
            else
            {
                query = query.Where(p => p.Status == stat);
            }
        }

        query = sortBy?.Trim().ToLower() switch
        {
            "price_asc" or "price-asc" => query.OrderBy(p => p.Price),
            "price_desc" or "price-desc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderByDescending(p => p.CreatedDate)
        };

        var totalCount = await query.LongCountAsync(cancellationToken);

        var safePageIndex = Math.Max(1, pageIndex);
        var safePageSize = Math.Max(1, pageSize);

        var items = await query
            .Skip((safePageIndex - 1) * safePageSize)
            .Take(safePageSize)
            .Select(p => new PackageSummaryDto
            {
                Id = p.Id,
                PackageCode = p.PackageCode,
                Name = p.Name,
                Price = p.Price,
                DurationMonths = p.DurationMonths,
                DurationDays = p.DurationDays,
                Description = p.Description,
                Status = p.Status == PackageStatus.Pending && p.EffectiveStartDate <= today
                    ? PackageStatus.Active
                    : p.Status,
                EffectiveStartDate = p.EffectiveStartDate,
                CategoryId = p.CategoryId,
                CategoryCode = p.Category != null ? p.Category.Code : null,
                CategoryName = p.Category != null ? p.Category.Name : null,
                PreviousPackageId = p.PreviousPackageId,
                PreviousPackageCode = p.PreviousPackage != null ? p.PreviousPackage.PackageCode : null,
                ActiveMembersCount = Context.MemberPackages.Count(mp =>
                    mp.PackageId == p.Id &&
                    mp.Status == MemberPackageStatus.Active &&
                    mp.EndDate >= today),
                CreatedDate = p.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PackageSummaryDto>(items, totalCount, safePageIndex, safePageSize);
    }

    public async Task<PackageDetailDto?> GetPackageDetailByIdAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var package = await DbSet.AsNoTracking()
            .Include(p => p.PreviousPackage)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == packageId && !p.IsDeleted, cancellationToken);

        if (package is null) return null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
        var activeCount = await Context.MemberPackages.CountAsync(mp =>
            mp.PackageId == packageId &&
            mp.Status == MemberPackageStatus.Active &&
            mp.EndDate >= today,
            cancellationToken);

        return new PackageDetailDto
        {
            Id = package.Id,
            PackageCode = package.PackageCode,
            Name = package.Name,
            Price = package.Price,
            DurationMonths = package.DurationMonths,
            DurationDays = package.DurationDays,
            Description = package.Description,
            Status = package.Status == PackageStatus.Pending && package.EffectiveStartDate <= today
                ? PackageStatus.Active
                : package.Status,
            EffectiveStartDate = package.EffectiveStartDate,
            CategoryId = package.CategoryId,
            CategoryCode = package.Category?.Code,
            CategoryName = package.Category?.Name,
            PreviousPackageId = package.PreviousPackageId,
            PreviousPackageCode = package.PreviousPackage?.PackageCode,
            ActiveMembersCount = activeCount,
            CreatedDate = package.CreatedDate,
            LastModifiedDate = package.LastModifiedDate
        };
    }
}
