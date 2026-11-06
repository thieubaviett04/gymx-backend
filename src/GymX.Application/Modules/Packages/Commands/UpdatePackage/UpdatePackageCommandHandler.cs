using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
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

        var hasEverBeenPurchased = await packageRepository.HasEverBeenPurchasedAsync(package.Id, cancellationToken);
        if (hasEverBeenPurchased && package.Status == PackageStatus.Discontinued)
        {
            return Result.Failure<PackageDetailDto>(
                Error.Conflict("Package.CannotModifyDiscontinued", "Không thể chỉnh sửa gói tập đã ngừng kinh doanh để đảm bảo tính toàn vẹn của dữ liệu lịch sử và hợp đồng hội viên."));
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

        return Result.Success(await MapDetailAsync(package, cancellationToken, categoryCode: categoryCode, categoryName: categoryName));
    }

    private async Task<PackageDetailDto> MapDetailAsync(Package pkg, CancellationToken ct, string? categoryCode = null, string? categoryName = null)
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
            CategoryId = pkg.CategoryId,
            CategoryCode = categoryCode,
            CategoryName = categoryName,
            PreviousPackageId = pkg.PreviousPackageId,
            PreviousPackageCode = pkg.PreviousPackage?.PackageCode,
            ActiveMembersCount = activeCount,
            CreatedDate = pkg.CreatedDate,
            LastModifiedDate = pkg.LastModifiedDate
        };
    }
}
