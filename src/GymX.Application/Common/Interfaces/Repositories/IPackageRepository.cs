using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Packages;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IPackageRepository : IGenericRepository<Package>
{
    Task<Package?> GetByCodeAsync(string packageCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsCodeAsync(string packageCode, CancellationToken cancellationToken = default);
    Task<bool> IsPackageInUseAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task<int> CountActiveMembersAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task<PagedResult<PackageSummaryDto>> GetPackagesPagedAsync(
        string? search,
        string? durationFilter,
        string? status,
        string? sortBy,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<PackageDetailDto?> GetPackageDetailByIdAsync(Guid packageId, CancellationToken cancellationToken = default);
}
