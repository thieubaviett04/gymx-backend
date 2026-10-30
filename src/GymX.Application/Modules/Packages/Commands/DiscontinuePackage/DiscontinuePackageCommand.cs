using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Packages.Commands.DiscontinuePackage;

public record DiscontinuePackageCommand(Guid PackageId) : IRequest<Result<bool>>;
