using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Queries.GetPackageCategoryById;

public record GetPackageCategoryByIdQuery(Guid Id) : IRequest<Result<PackageCategorySummaryDto>>;

