using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Commands.UpdatePackageCategory;

public class UpdatePackageCategoryCommandHandler(
    IPackageCategoryRepository packageCategoryRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePackageCategoryCommand, Result<PackageCategorySummaryDto>>
{
    public async Task<Result<PackageCategorySummaryDto>> Handle(
        UpdatePackageCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await packageCategoryRepository.GetByIdAsync(command.Id, cancellationToken);
        if (category is null || category.IsDeleted)
        {
            return Result.Failure<PackageCategorySummaryDto>(PackageCategoryErrors.NotFound(command.Id));
        }

        if (await packageCategoryRepository.ExistsNameAsync(command.Name, excludeId: command.Id, cancellationToken: cancellationToken))
        {
            return Result.Failure<PackageCategorySummaryDto>(PackageCategoryErrors.NameAlreadyExists);
        }

        category.Update(command.Name, command.Description, command.Status);
        packageCategoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await packageCategoryRepository.GetSummaryByIdAsync(category.Id, cancellationToken);
        return Result.Success(dto!);
    }
}

