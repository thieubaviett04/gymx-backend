using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Packages.Queries.GetPackageById;

public class GetPackageByIdQueryHandler(IPackageRepository packageRepository)
    : IRequestHandler<GetPackageByIdQuery, Result<PackageDetailDto>>
{
    public async Task<Result<PackageDetailDto>> Handle(GetPackageByIdQuery request, CancellationToken cancellationToken)
    {
        var detail = await packageRepository.GetPackageDetailByIdAsync(request.PackageId, cancellationToken);
        if (detail is null)
        {
            return Result.Failure<PackageDetailDto>(
                Error.NotFound("Package.NotFound", "Không tìm thấy thông tin gói tập."));
        }

        return Result.Success(detail);
    }
}
