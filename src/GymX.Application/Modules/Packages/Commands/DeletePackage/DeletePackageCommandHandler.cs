using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using MediatR;

namespace GymX.Application.Modules.Packages.Commands.DeletePackage;

public class DeletePackageCommandHandler(
    IPackageRepository packageRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePackageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeletePackageCommand command, CancellationToken cancellationToken)
    {
        var package = await packageRepository.GetByIdAsync(command.PackageId, cancellationToken);
        if (package is null || package.IsDeleted)
        {
            return Result.Failure<bool>(
                Error.NotFound("Package.NotFound", "Không tìm thấy thông tin gói tập."));
        }

        var hasEverBeenPurchased = await packageRepository.HasEverBeenPurchasedAsync(package.Id, cancellationToken);
        if (hasEverBeenPurchased)
        {
            var isBeingUsed = await packageRepository.IsPackageInUseAsync(package.Id, cancellationToken);
            if (isBeingUsed)
            {
                var message = package.Status == PackageStatus.Discontinued
                    ? "Gói tập này đã ngừng kinh doanh nhưng hiện vẫn còn hội viên đang trong thời hạn sử dụng thẻ, không thể xóa để bảo toàn dữ liệu lịch sử."
                    : "Không thể xóa do đang có người sử dụng, bạn có muốn cập nhật trạng thái 'Ngừng kinh doanh' cho gói tập này không?";

                return Result.Failure<bool>(
                    Error.Conflict("Package.CannotDeleteInUse", message));
            }

            return Result.Failure<bool>(
                Error.Conflict("Package.CannotDeleteHistoricPackage", "Gói tập này đã phát sinh dữ liệu đăng ký của hội viên trong lịch sử, không thể xóa để bảo toàn dữ liệu đối soát và hợp đồng."));
        }

        package.SoftDelete();
        packageRepository.Update(package);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
