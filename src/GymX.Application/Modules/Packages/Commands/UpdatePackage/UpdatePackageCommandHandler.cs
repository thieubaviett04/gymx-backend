using GymX.Application.Common.Exceptions;
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
    private const int MaxSaveRetries = 3;

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

        var isBeingUsed = await packageRepository.IsPackageInUseAsync(package.Id, cancellationToken);

        if (isBeingUsed)
        {
            if (command.Mode == "DISCONTINUE_ONLY")
            {
                package.Discontinue();
                packageRepository.Update(package);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(await MapDetailAsync(package, cancellationToken, categoryCode: categoryCode, categoryName: categoryName));
            }

            if (command.Mode == "DISCONTINUE_AND_CREATE")
            {
                package.Discontinue();
                packageRepository.Update(package);

                var newPackage = Package.Create(
                    command.Name,
                    command.Price!.Value,
                    command.DurationMonths,
                    command.EffectiveStartDate,
                    command.EffectiveEndDate,
                    command.Description,
                    previousPackageId: package.Id,
                    categoryId: targetCategoryId);

                await packageRepository.AddAsync(newPackage, cancellationToken);

                for (var attempt = 1; attempt <= MaxSaveRetries; attempt++)
                {
                    try
                    {
                        await unitOfWork.SaveChangesAsync(cancellationToken);
                        return Result.Success(await MapDetailAsync(newPackage, cancellationToken, previousCode: package.PackageCode, categoryCode: categoryCode, categoryName: categoryName));
                    }
                    catch (UniqueConstraintException)
                    {
                        if (attempt < MaxSaveRetries)
                        {
                            newPackage.RegenerateCode(Package.GenerateRandomCode());
                        }
                    }
                }

                return Result.Failure<PackageDetailDto>(
                    Error.Conflict("Package.CodeCollision", "Không thể sinh mã cho gói tập mới. Vui lòng thử lại."));
            }

            return Result.Failure<PackageDetailDto>(
                Error.Conflict("Package.InUse", "Gói tập này đang có người sử dụng. Bạn có muốn cập nhật trạng thái 'Ngừng kinh doanh' và tạo 1 gói tập mới với các trường thông tin đã sửa không?"));
        }

        package.UpdateInfo(
            command.Name,
            command.Price!.Value,
            command.DurationMonths,
            command.EffectiveStartDate,
            command.EffectiveEndDate,
            command.Status,
            command.Description,
            targetCategoryId);

        packageRepository.Update(package);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(await MapDetailAsync(package, cancellationToken, categoryCode: categoryCode, categoryName: categoryName));
    }

    private async Task<PackageDetailDto> MapDetailAsync(Package pkg, CancellationToken ct, string? previousCode = null, string? categoryCode = null, string? categoryName = null)
    {
        var activeCount = await packageRepository.CountActiveMembersAsync(pkg.Id, ct);
        if (pkg.CategoryId.HasValue && categoryCode == null)
        {
            var category = await packageCategoryRepository.GetByIdAsync(pkg.CategoryId.Value, ct);
            categoryCode = category?.Code;
            categoryName = category?.Name;
        }

        return new PackageDetailDto
        {
            Id = pkg.Id,
            PackageCode = pkg.PackageCode,
            Name = pkg.Name,
            Price = pkg.Price,
            DurationMonths = pkg.DurationMonths,
            DurationDays = pkg.DurationDays,
            Description = pkg.Description,
            Status = pkg.Status,
            EffectiveStartDate = pkg.EffectiveStartDate,
            EffectiveEndDate = pkg.EffectiveEndDate,
            CategoryId = pkg.CategoryId,
            CategoryCode = categoryCode,
            CategoryName = categoryName,
            PreviousPackageId = pkg.PreviousPackageId,
            PreviousPackageCode = previousCode,
            ActiveMembersCount = activeCount,
            CreatedDate = pkg.CreatedDate,
            LastModifiedDate = pkg.LastModifiedDate
        };
    }
}
