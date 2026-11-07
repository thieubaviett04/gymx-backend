using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Queries.GetPenaltyRules;

public class GetPenaltyRulesQueryHandler(IPenaltyRuleRepository penaltyRuleRepository)
    : IRequestHandler<GetPenaltyRulesQuery, Result<PagedResult<PenaltyRuleSummaryDto>>>
{
    public async Task<Result<PagedResult<PenaltyRuleSummaryDto>>> Handle(GetPenaltyRulesQuery query, CancellationToken cancellationToken)
    {
        var pagedPenaltyRules = await penaltyRuleRepository.GetPenaltyRulesPagedAsync(
            query.Search,
            query.Severity,
            query.Status,
            query.TargetAudience,
            query.SortBy,
            query.PageIndex,
            query.PageSize,
            cancellationToken);

        return Result.Success(pagedPenaltyRules);
    }
}
