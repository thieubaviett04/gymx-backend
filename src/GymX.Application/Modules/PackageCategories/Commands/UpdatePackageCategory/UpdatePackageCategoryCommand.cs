using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Commands.UpdatePackageCategory;

public record UpdatePackageCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    string Status) : IRequest<Result<PackageCategorySummaryDto>>;

