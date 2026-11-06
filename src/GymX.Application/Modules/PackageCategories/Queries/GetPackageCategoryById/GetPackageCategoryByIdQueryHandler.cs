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
        var category = await packageCategoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null || category.IsDeleted)
        {
            return Result.Failure<PackageCategorySummaryDto>(PackageCategoryErrors.NotFound(request.Id));
        }

        var dto = new PackageCategorySummaryDto
        {
            Id = category.Id,
            Code = category.Code,
            Name = category.Name,
            Description = category.Description,
            Status = category.Status,
            PackagesCount = 0,
            CreatedDate = category.CreatedDate,
            LastModifiedDate = category.LastModifiedDate
        };

        return Result.Success(dto);
    }
}

