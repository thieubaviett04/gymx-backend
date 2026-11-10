using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Packages;
using MediatR;

namespace GymX.Application.Modules.Packages.Commands.UpdatePackage;

public class UpdatePackageCommandHandler(
    IPackageRepository packageRepository,
    IPackageCategoryRepository packageCategoryRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePackageCommand, Result<PackageDetailDto>>
{
    public async Task<Result<PackageDetailDto>> Handle(UpdatePackageCommand command, CancellationToken cancellationToken)
    {
        var package = await packageRepository.GetByIdAsync(command.PackageId, cancellationToken);
        if (package is null || package.IsDeleted)
        {
            return Result.Failure<PackageDetailDto>(
                Error.NotFound("Package.NotFound", "Không tìm thấy thông tin gói tập."));
        }

        string? categoryCode = null;
        string? categoryName = null;
        var targetCategoryId = command.CategoryId ?? package.CategoryId;

        if (targetCategoryId.HasValue)
        {
            var category = await packageCategoryRepository.GetByIdAsync(targetCategoryId.Value, cancellationToken);
            if (category is null || category.IsDeleted)
            {
                return Result.Failure<PackageDetailDto>(
                    Error.NotFound("PackageCategory.NotFound", "Không tìm thấy loại hình gói tập đã chọn."));
            }

            categoryCode = category.Code;
            categoryName = category.Name;
        }

        package.UpdateInfo(
            command.Name,
            command.Price ?? package.Price,
            command.DurationMonths,
            command.EffectiveStartDate,
            command.Status,
            command.Description,
            targetCategoryId);

        packageRepository.Update(package);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var detail = await packageRepository.GetPackageDetailByIdAsync(package.Id, cancellationToken);
        return Result.Success(detail!);
    }
}
