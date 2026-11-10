using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Commands.DeletePackageCategory;

public class DeletePackageCategoryCommandHandler(
    IPackageCategoryRepository packageCategoryRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePackageCategoryCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeletePackageCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await packageCategoryRepository.GetByIdAsync(command.Id, cancellationToken);
        if (category is null || category.IsDeleted)
        {
            return Result.Failure<bool>(PackageCategoryErrors.NotFound(command.Id));
        }

        if (await packageCategoryRepository.HasPackagesAsync(command.Id, cancellationToken))
        {
            return Result.Failure<bool>(PackageCategoryErrors.CannotDeleteInUse);
        }

        category.SoftDelete();
        packageCategoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}

