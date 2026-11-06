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

        var isBeingUsed = await packageRepository.IsPackageInUseAsync(package.Id, cancellationToken);
        if (isBeingUsed)
        {
            return Result.Failure<bool>(
                Error.Conflict("Package.CannotDeleteInUse", "Gói tập này hiện đang có hội viên sử dụng, không thể xóa."));
        }

        package.SoftDelete();
        packageRepository.Update(package);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
