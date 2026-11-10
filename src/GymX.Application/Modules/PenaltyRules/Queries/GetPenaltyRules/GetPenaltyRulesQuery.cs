using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Queries.GetPenaltyRules;

public record GetPenaltyRulesQuery(
    string? Search,
    string? Severity,
    string? Status,
    string? TargetAudience,
    string? SortBy,
    int PageIndex,
    int PageSize) : IRequest<Result<PagedResult<PenaltyRuleSummaryDto>>>;
