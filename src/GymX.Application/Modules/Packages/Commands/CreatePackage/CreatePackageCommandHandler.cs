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
            command.MaxCheckins,
            command.Description,
            packageCode);

        await packageRepository.AddAsync(package, cancellationToken);

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
                package.RegenerateCode(Package.GenerateRandomCode());
            }
        }

        if (!saveSucceeded)
        {
            return Result.Failure<PackageDetailDto>(
                Error.Conflict("Package.CodeCollision", "Không thể sinh mã gói tập duy nhất. Vui lòng thử lại."));
        }

        var resultDto = new PackageDetailDto
        {
            Id = package.Id,
            PackageCode = package.PackageCode,
            Name = package.Name,
            Price = package.Price,
            DurationMonths = package.DurationMonths,
            DurationDays = package.DurationDays,
            MaxCheckins = package.MaxCheckins,
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

    private static bool IsUniqueConstraintViolation(Exception ex)
    {
        var text = ex.ToString();
        return text.Contains("23505") ||
               text.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("unique constraint", StringComparison.OrdinalIgnoreCase);
    }
}
