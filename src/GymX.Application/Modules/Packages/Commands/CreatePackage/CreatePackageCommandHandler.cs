using GymX.Application.Common.Exceptions;
using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Packages;
using MediatR;

namespace GymX.Application.Modules.Packages.Commands.CreatePackage;

public class CreatePackageCommandHandler(
    IPackageRepository packageRepository,
    IPackageCategoryRepository packageCategoryRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreatePackageCommand, Result<PackageDetailDto>>
{
    private const int MaxSaveRetries = 3;

    public async Task<Result<PackageDetailDto>> Handle(CreatePackageCommand command, CancellationToken cancellationToken)
    {
        string? categoryCode = null;
        string? categoryName = null;

        if (command.CategoryId.HasValue)
        {
            var category = await packageCategoryRepository.GetByIdAsync(command.CategoryId.Value, cancellationToken);
            if (category is null || category.IsDeleted)
            {
                return Result.Failure<PackageDetailDto>(
                    Error.NotFound("PackageCategory.NotFound", "Không tìm thấy loại hình gói tập đã chọn."));
            }

            categoryCode = category.Code;
            categoryName = category.Name;
        }

        string packageCode;
        do
        {
            packageCode = Package.GenerateRandomCode();
        } while (await packageRepository.ExistsCodeAsync(packageCode, cancellationToken));

        var package = Package.Create(
            command.Name,
            command.Price!.Value,
            command.DurationMonths,
            command.EffectiveStartDate,
            command.Description,
            packageCode,
            categoryId: command.CategoryId,
            status: command.Status);

        await packageRepository.AddAsync(package, cancellationToken);

        for (var attempt = 1; attempt <= MaxSaveRetries; attempt++)
        {
            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var detail = await packageRepository.GetPackageDetailByIdAsync(package.Id, cancellationToken);
                return Result.Success(detail!);
            }
            catch (UniqueConstraintException)
            {
                if (attempt < MaxSaveRetries)
                {
                    package.RegenerateCode(Package.GenerateRandomCode());
                }
            }
        }

        return Result.Failure<PackageDetailDto>(
            Error.Conflict("Package.CodeCollision", "Không thể sinh mã gói tập duy nhất. Vui lòng thử lại."));
    }
}
