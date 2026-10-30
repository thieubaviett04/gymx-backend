using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
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

        // Kiểm tra xem có người đang sử dụng không (Luồng A-2)
        var isBeingUsed = await packageRepository.IsPackageInUseAsync(package.Id, cancellationToken);
        if (isBeingUsed)
        {
            return Result.Failure<bool>(
                Error.Conflict("Package.CannotDeleteInUse", "Không thể xóa do đang có người sử dụng, bạn có muốn cập nhật trạng thái 'Ngừng kinh doanh' cho gói tập này không?"));
        }

        // Không có ai dùng: Thực hiện xóa mềm
        package.SoftDelete();
        packageRepository.Update(package);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
