using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Packages.Queries.GetPackages;

public class GetPackagesQueryHandler(IPackageRepository packageRepository)
    : IRequestHandler<GetPackagesQuery, Result<PagedResult<PackageSummaryDto>>>
{
    public async Task<Result<PagedResult<PackageSummaryDto>>> Handle(GetPackagesQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;
        var pageSize = request.PageSize <= 0 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

        var result = await packageRepository.GetPackagesPagedAsync(
            request.Search,
            request.DurationFilter,
            request.Status,
            request.SortBy,
            pageIndex,
            pageSize,
            cancellationToken);

        return Result.Success(result);
    }
}
