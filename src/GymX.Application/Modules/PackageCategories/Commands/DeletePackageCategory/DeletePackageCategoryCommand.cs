using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Commands.DeletePackageCategory;

public record DeletePackageCategoryCommand(Guid Id) : IRequest<Result<bool>>;

