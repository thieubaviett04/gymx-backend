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
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreatePackageCommand, Result<PackageDetailDto>>
{
    private const int MaxSaveRetries = 3;

    public async Task<Result<PackageDetailDto>> Handle(CreatePackageCommand command, CancellationToken cancellationToken)
    {
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
            command.EffectiveEndDate,
            command.Description,
            packageCode);

        await packageRepository.AddAsync(package, cancellationToken);

        for (var attempt = 1; attempt <= MaxSaveRetries; attempt++)
        {
            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var resultDto = new PackageDetailDto
                {
                    Id = package.Id,
                    PackageCode = package.PackageCode,
                    Name = package.Name,
                    Price = package.Price,
                    DurationMonths = package.DurationMonths,
                    DurationDays = package.DurationDays,
                    Description = package.Description,
                    Status = package.Status,
                    EffectiveStartDate = package.EffectiveStartDate,
                    EffectiveEndDate = package.EffectiveEndDate,
                    PreviousPackageId = null,
                    PreviousPackageCode = null,
                    ActiveMembersCount = 0,
                    CreatedDate = package.CreatedDate
                };

                return Result.Success(resultDto);
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
