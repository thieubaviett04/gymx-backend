using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetEmployeeKpisPaged;

public sealed record GetEmployeeKpisPagedQuery(
    int Month,
    int Year,
    string? Search,
    string? Position,
    string? Status,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<EmployeeKpiProgressSummaryDto>>>;
