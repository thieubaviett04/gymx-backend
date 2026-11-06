using GymX.Application.Common.Exceptions;
using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Packages;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Commands.CreatePackageCategory;

public class CreatePackageCategoryCommandHandler(
    IPackageCategoryRepository packageCategoryRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreatePackageCategoryCommand, Result<PackageCategorySummaryDto>>
{
    public async Task<Result<PackageCategorySummaryDto>> Handle(
        CreatePackageCategoryCommand command,
        CancellationToken cancellationToken)
    {
        if (await packageCategoryRepository.ExistsCodeAsync(command.Code, cancellationToken: cancellationToken))
        {
            return Result.Failure<PackageCategorySummaryDto>(PackageCategoryErrors.CodeAlreadyExists);
        }

        if (await packageCategoryRepository.ExistsNameAsync(command.Name, cancellationToken: cancellationToken))
        {
            return Result.Failure<PackageCategorySummaryDto>(PackageCategoryErrors.NameAlreadyExists);
        }

        var category = PackageCategory.Create(command.Code, command.Name, command.Description);

        await packageCategoryRepository.AddAsync(category, cancellationToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintException)
        {
            return Result.Failure<PackageCategorySummaryDto>(PackageCategoryErrors.CodeAlreadyExists);
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

