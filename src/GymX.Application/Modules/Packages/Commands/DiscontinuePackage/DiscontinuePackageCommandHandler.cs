using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Packages.Commands.DiscontinuePackage;

public class DiscontinuePackageCommandHandler(
    IPackageRepository packageRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DiscontinuePackageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DiscontinuePackageCommand command, CancellationToken cancellationToken)
    {
        var package = await packageRepository.GetByIdAsync(command.PackageId, cancellationToken);
        if (package is null || package.IsDeleted)
        {
            return Result.Failure<bool>(
                Error.NotFound("Package.NotFound", "Không tìm thấy thông tin gói tập."));
        }

        package.Discontinue();
        packageRepository.Update(package);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
