using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Queries.GetPenaltyRuleById;

public class GetPenaltyRuleByIdQueryHandler(IPenaltyRuleRepository penaltyRuleRepository)
    : IRequestHandler<GetPenaltyRuleByIdQuery, Result<PenaltyRuleDetailDto>>
{
    public async Task<Result<PenaltyRuleDetailDto>> Handle(GetPenaltyRuleByIdQuery query, CancellationToken cancellationToken)
    {
        var penaltyRuleDetail = await penaltyRuleRepository.GetPenaltyRuleDetailByIdAsync(query.RuleId, cancellationToken);
        if (penaltyRuleDetail is null)
        {
            return Result.Failure<PenaltyRuleDetailDto>(
                Error.NotFound("PenaltyRule.NotFound", "Không tìm thấy thông tin quy định phạt."));
        }

        return Result.Success(penaltyRuleDetail);
    }
}
