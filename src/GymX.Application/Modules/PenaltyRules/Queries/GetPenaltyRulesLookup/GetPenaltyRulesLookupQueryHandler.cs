using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Queries.GetPenaltyRulesLookup;

public class GetPenaltyRulesLookupQueryHandler(IPenaltyRuleRepository penaltyRuleRepository)
    : IRequestHandler<GetPenaltyRulesLookupQuery, Result<List<PenaltyRuleLookupDto>>>
{
    public async Task<Result<List<PenaltyRuleLookupDto>>> Handle(
        GetPenaltyRulesLookupQuery request,
        CancellationToken cancellationToken)
    {
        var lookup = await penaltyRuleRepository.GetLookupAsync(request.TargetAudience, cancellationToken);
        return Result.Success(lookup);
    }
}

