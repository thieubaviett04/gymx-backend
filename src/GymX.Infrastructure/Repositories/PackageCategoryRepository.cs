using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Packages;
using GymX.Domain.Entities.Packages;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class PackageCategoryRepository(ApplicationDbContext context)
    : GenericRepository<PackageCategory>(context), IPackageCategoryRepository
{
    public Task<PackageCategory?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return DbSet.FirstOrDefaultAsync(c => c.Code == normalizedCode && !c.IsDeleted, cancellationToken);
    }

    public Task<bool> ExistsCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return DbSet.AnyAsync(
            c => c.Code == normalizedCode && !c.IsDeleted && (!excludeId.HasValue || c.Id != excludeId.Value),
            cancellationToken);
    }

    public Task<bool> ExistsNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToLower();
        return DbSet.AnyAsync(
            c => c.Name.ToLower() == normalizedName && !c.IsDeleted && (!excludeId.HasValue || c.Id != excludeId.Value),
            cancellationToken);
    }

    public Task<bool> HasPackagesAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => Context.Packages.AnyAsync(p => p.CategoryId == categoryId && !p.IsDeleted, cancellationToken);

    public async Task<PagedResult<PackageCategorySummaryDto>> GetCategoriesPagedAsync(
        string? search,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(c => c.Code.ToLower().Contains(searchLower) || c.Name.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            query = query.Where(c => c.Status == normalizedStatus);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var safePageIndex = Math.Max(1, pageIndex);
        var safePageSize = Math.Max(1, pageSize);

        var items = await query
            .OrderByDescending(c => c.CreatedDate)
            .Skip((safePageIndex - 1) * safePageSize)
            .Take(safePageSize)
            .Select(c => new PackageCategorySummaryDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Description = c.Description,
                Status = c.Status,
                PackagesCount = c.Packages.Count(p => !p.IsDeleted),
                CreatedDate = c.CreatedDate,
                LastModifiedDate = c.LastModifiedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PackageCategorySummaryDto>(items, totalCount, safePageIndex, safePageSize);
    }

    public Task<List<PackageCategoryLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .Where(c => !c.IsDeleted && c.Status == PackageCategoryStatus.Active)
            .OrderBy(c => c.Name)
            .Select(c => new PackageCategoryLookupDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name
            })
            .ToListAsync(cancellationToken);
    }
}

