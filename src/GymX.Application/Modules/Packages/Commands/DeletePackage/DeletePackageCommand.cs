using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Packages.Commands.DeletePackage;

public record DeletePackageCommand(Guid PackageId) : IRequest<Result<bool>>;
