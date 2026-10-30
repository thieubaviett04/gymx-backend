using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Packages.Queries.GetPackages;

public record GetPackagesQuery(
    string? Search,
    string? DurationFilter,
    string? Status,
    string? SortBy,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<PackageSummaryDto>>>;
