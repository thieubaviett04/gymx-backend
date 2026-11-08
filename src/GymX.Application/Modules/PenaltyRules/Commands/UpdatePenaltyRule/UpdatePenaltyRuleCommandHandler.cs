using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payroll;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Commands.UpdatePenaltyRule;

public class UpdatePenaltyRuleCommandHandler(
    IPenaltyRuleRepository penaltyRuleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePenaltyRuleCommand, Result<PenaltyRuleDetailDto>>
{
    public async Task<Result<PenaltyRuleDetailDto>> Handle(UpdatePenaltyRuleCommand command, CancellationToken cancellationToken)
    {
        var existingPenaltyRule = await penaltyRuleRepository.GetByIdAsync(command.RuleId, cancellationToken);
        if (existingPenaltyRule is null || existingPenaltyRule.IsDeleted)
        {
            return Result.Failure<PenaltyRuleDetailDto>(
                Error.NotFound("PenaltyRule.NotFound", "Không tìm thấy thông tin quy định phạt."));
        }

        existingPenaltyRule.UpdateInfo(
            command.Name,
            command.FineAmount,
            command.SeverityLevel,
            command.EffectiveFrom,
            command.Status,
            command.Description,
            command.ViolationType);

        penaltyRuleRepository.Update(existingPenaltyRule);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var detail = await penaltyRuleRepository.GetPenaltyRuleDetailByIdAsync(existingPenaltyRule.Id, cancellationToken);
        return Result.Success(detail!);
    }
}
