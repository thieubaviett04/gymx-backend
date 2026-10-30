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

    public async Task<bool> IsPackageInUseAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await Context.MemberPackages.AnyAsync(mp =>
            mp.PackageId == packageId &&
            mp.Status == MemberPackageStatus.Active &&
            mp.EndDate >= today,
            cancellationToken);
    }

    public async Task<int> CountActiveMembersAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await Context.MemberPackages.CountAsync(mp =>
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
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(p => !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(p =>
                EF.Functions.ILike(p.PackageCode, $"%{keyword}%") ||
                EF.Functions.ILike(p.Name, $"%{keyword}%"));
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

        if (!string.IsNullOrWhiteSpace(status))
        {
            var stat = status.Trim().ToUpperInvariant();
            query = query.Where(p => p.Status == stat);
        }

        query = sortBy?.Trim().ToLower() switch
        {
            "price_asc" or "price-asc" => query.OrderBy(p => p.Price),
            "price_desc" or "price-desc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderByDescending(p => p.CreatedDate)
        };

        var totalCount = await query.LongCountAsync(cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PackageSummaryDto
            {
                Id = p.Id,
                PackageCode = p.PackageCode,
                Name = p.Name,
                Price = p.Price,
                DurationMonths = p.DurationMonths,
                DurationDays = p.DurationDays,
                MaxCheckins = p.MaxCheckins,
                Description = p.Description,
                Status = p.Status,
                EffectiveStartDate = p.EffectiveStartDate,
                EffectiveEndDate = p.EffectiveEndDate,
                PreviousPackageId = p.PreviousPackageId,
                PreviousPackageCode = p.PreviousPackage != null ? p.PreviousPackage.PackageCode : null,
                ActiveMembersCount = Context.MemberPackages.Count(mp =>
                    mp.PackageId == p.Id &&
                    mp.Status == MemberPackageStatus.Active &&
                    mp.EndDate >= today),
                CreatedDate = p.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PackageSummaryDto>(items, totalCount, pageIndex, pageSize);
    }

    public async Task<PackageDetailDto?> GetPackageDetailByIdAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var package = await DbSet.AsNoTracking()
            .Include(p => p.PreviousPackage)
            .FirstOrDefaultAsync(p => p.Id == packageId && !p.IsDeleted, cancellationToken);

        if (package is null) return null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
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
            MaxCheckins = package.MaxCheckins,
            Description = package.Description,
            Status = package.Status,
            EffectiveStartDate = package.EffectiveStartDate,
            EffectiveEndDate = package.EffectiveEndDate,
            PreviousPackageId = package.PreviousPackageId,
            PreviousPackageCode = package.PreviousPackage?.PackageCode,
            ActiveMembersCount = activeCount,
            CreatedDate = package.CreatedDate,
            LastModifiedDate = package.LastModifiedDate
        };
    }
}
