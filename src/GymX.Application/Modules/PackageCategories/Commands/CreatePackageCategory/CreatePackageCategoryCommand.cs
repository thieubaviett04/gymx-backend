using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Commands.CreatePackageCategory;

public record CreatePackageCategoryCommand(
    string Code,
    string Name,
    string? Description) : IRequest<Result<PackageCategorySummaryDto>>;
