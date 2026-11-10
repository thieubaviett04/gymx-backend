using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Commands.DiscontinuePenaltyRule;

public class DiscontinuePenaltyRuleCommandHandler(
    IPenaltyRuleRepository penaltyRuleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DiscontinuePenaltyRuleCommand, Result>
{
    public async Task<Result> Handle(DiscontinuePenaltyRuleCommand command, CancellationToken cancellationToken)
    {
        var penaltyRule = await penaltyRuleRepository.GetByIdAsync(command.RuleId, cancellationToken);
        if (penaltyRule is null || penaltyRule.IsDeleted)
        {
            return Result.Failure(Error.NotFound("PenaltyRule.NotFound", "Không tìm thấy thông tin quy định phạt."));
        }

        penaltyRule.Discontinue();
        penaltyRuleRepository.Update(penaltyRule);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
