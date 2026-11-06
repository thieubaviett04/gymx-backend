using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Queries.GetPackageCategories;

public record GetPackageCategoriesQuery(
    string? Search,
    string? Status,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<PackageCategorySummaryDto>>>;
