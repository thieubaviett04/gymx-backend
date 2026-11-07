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

        var appliedViolationsCount = await penaltyRuleRepository.CountViolationsAsync(existingPenaltyRule.Id, cancellationToken);
        return Result.Success(MapPenaltyRuleDetail(existingPenaltyRule, violationsCount: appliedViolationsCount));
    }

    private static PenaltyRuleDetailDto MapPenaltyRuleDetail(PenaltyRule rule, string? previousCode = null, int violationsCount = 0) => new()
    {
        Id = rule.Id,
        RuleCode = rule.RuleCode,
        Name = rule.Name,
        ViolationType = rule.ViolationType,
        FineAmount = rule.FineAmount,
        SeverityLevel = rule.SeverityLevel,
        Status = rule.Status,
        EffectiveFrom = rule.EffectiveFrom,
        EffectiveTo = rule.EffectiveTo,
        Description = rule.Description,
        PreviousRuleId = rule.PreviousRuleId,
        PreviousRuleCode = previousCode,
        ViolationsCount = violationsCount,
        CreatedDate = rule.CreatedDate,
        LastModifiedDate = rule.LastModifiedDate
    };
}
