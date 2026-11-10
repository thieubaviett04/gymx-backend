using GymX.Application.Modules.Violations.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Violations.Queries.GetViolations;

public record GetViolationsQuery(
    string? Search,
    Guid? EmployeeId,
    Guid? PenaltyRuleId,
    string? Status,
    DateTime? FromDate,
    DateTime? ToDate,
    string? SortBy,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<ViolationSummaryDto>>>;

