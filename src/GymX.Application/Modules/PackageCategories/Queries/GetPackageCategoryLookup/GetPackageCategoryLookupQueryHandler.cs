using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Queries.GetPackageCategoryLookup;

public class GetPackageCategoryLookupQueryHandler(IPackageCategoryRepository packageCategoryRepository)
    : IRequestHandler<GetPackageCategoryLookupQuery, Result<List<PackageCategoryLookupDto>>>
{
    public async Task<Result<List<PackageCategoryLookupDto>>> Handle(
        GetPackageCategoryLookupQuery request,
        CancellationToken cancellationToken)
    {
        var result = await packageCategoryRepository.GetLookupAsync(cancellationToken);
        return Result.Success(result);
    }
}

