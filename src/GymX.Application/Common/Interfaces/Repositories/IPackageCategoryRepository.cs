using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Packages;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IPackageCategoryRepository : IGenericRepository<PackageCategory>
{
    Task<PackageCategory?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasPackagesAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<PagedResult<PackageCategorySummaryDto>> GetCategoriesPagedAsync(
        string? search,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<PackageCategorySummaryDto?> GetSummaryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<PackageCategoryLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
}

