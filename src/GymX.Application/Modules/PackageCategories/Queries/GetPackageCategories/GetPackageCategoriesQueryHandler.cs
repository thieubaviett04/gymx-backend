using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Queries.GetPackageCategories;

public class GetPackageCategoriesQueryHandler(IPackageCategoryRepository packageCategoryRepository)
    : IRequestHandler<GetPackageCategoriesQuery, Result<PagedResult<PackageCategorySummaryDto>>>
{
    public async Task<Result<PagedResult<PackageCategorySummaryDto>>> Handle(
        GetPackageCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var pageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;
        var pageSize = request.PageSize <= 0 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

        var result = await packageCategoryRepository.GetCategoriesPagedAsync(
            request.Search,
            request.Status,
            pageIndex,
            pageSize,
            cancellationToken);

        return Result.Success(result);
    }
}
