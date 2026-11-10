using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Packages.Queries.GetPackageById;

public record GetPackageByIdQuery(Guid PackageId) : IRequest<Result<PackageDetailDto>>;
