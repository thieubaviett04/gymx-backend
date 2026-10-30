using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Packages;
using MediatR;

namespace GymX.Application.Modules.Packages.Commands.UpdatePackage;

public class UpdatePackageCommandHandler(
    IPackageRepository packageRepository,
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

        var isBeingUsed = await packageRepository.IsPackageInUseAsync(package.Id, cancellationToken);

        if (isBeingUsed)
        {
            if (command.Mode == "DISCONTINUE_ONLY")
            {
                package.Discontinue();
                packageRepository.Update(package);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(await MapDetailAsync(package, cancellationToken));
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
                    command.MaxCheckins,
                    command.Description,
                    previousPackageId: package.Id);

                await packageRepository.AddAsync(newPackage, cancellationToken);

                var saveSucceeded = false;
                for (var attempt = 1; attempt <= MaxSaveRetries; attempt++)
                {
                    try
                    {
                        await unitOfWork.SaveChangesAsync(cancellationToken);
                        saveSucceeded = true;
                        break;
                    }
                    catch (Exception ex) when (IsUniqueConstraintViolation(ex) && attempt < MaxSaveRetries)
                    {
                        newPackage.RegenerateCode(Package.GenerateRandomCode());
                    }
                }

                if (!saveSucceeded)
                {
                    return Result.Failure<PackageDetailDto>(
                        Error.Conflict("Package.CodeCollision", "Không thể sinh mã cho gói tập mới. Vui lòng thử lại."));
                }

                return Result.Success(await MapDetailAsync(newPackage, cancellationToken, previousCode: package.PackageCode));
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
            command.MaxCheckins,
            command.Description);

        packageRepository.Update(package);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(await MapDetailAsync(package, cancellationToken));
    }

    private static bool IsUniqueConstraintViolation(Exception ex)
    {
        var text = ex.ToString();
        return text.Contains("23505") ||
               text.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("unique constraint", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<PackageDetailDto> MapDetailAsync(Package pkg, CancellationToken ct, string? previousCode = null)
    {
        var activeCount = await packageRepository.CountActiveMembersAsync(pkg.Id, ct);
        return new PackageDetailDto
        {
            Id = pkg.Id,
            PackageCode = pkg.PackageCode,
            Name = pkg.Name,
            Price = pkg.Price,
            DurationMonths = pkg.DurationMonths,
            DurationDays = pkg.DurationDays,
            MaxCheckins = pkg.MaxCheckins,
            Description = pkg.Description,
            Status = pkg.Status,
            EffectiveStartDate = pkg.EffectiveStartDate,
            EffectiveEndDate = pkg.EffectiveEndDate,
            PreviousPackageId = pkg.PreviousPackageId,
            PreviousPackageCode = previousCode,
            ActiveMembersCount = activeCount,
            CreatedDate = pkg.CreatedDate,
            LastModifiedDate = pkg.LastModifiedDate
        };
    }
}
