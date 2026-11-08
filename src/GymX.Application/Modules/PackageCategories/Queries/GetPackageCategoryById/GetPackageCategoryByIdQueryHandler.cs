using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Queries.GetPackageCategoryById;

public class GetPackageCategoryByIdQueryHandler(IPackageCategoryRepository packageCategoryRepository)
    : IRequestHandler<GetPackageCategoryByIdQuery, Result<PackageCategorySummaryDto>>
{
    public async Task<Result<PackageCategorySummaryDto>> Handle(
        GetPackageCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var dto = await packageCategoryRepository.GetSummaryByIdAsync(request.Id, cancellationToken);
        if (dto is null)
        {
            return Result.Failure<PackageCategorySummaryDto>(PackageCategoryErrors.NotFound(request.Id));
        }

        return Result.Success(dto);
    }
}

