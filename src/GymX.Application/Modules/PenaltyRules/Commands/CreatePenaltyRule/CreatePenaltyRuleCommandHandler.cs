using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payroll;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Commands.CreatePenaltyRule;

public class CreatePenaltyRuleCommandHandler(
    IPenaltyRuleRepository penaltyRuleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreatePenaltyRuleCommand, Result<PenaltyRuleDetailDto>>
{
    private const int MaxSaveRetries = 3;

    public async Task<Result<PenaltyRuleDetailDto>> Handle(CreatePenaltyRuleCommand command, CancellationToken cancellationToken)
    {
        var penaltyRule = PenaltyRule.Create(
            command.Name,
            command.FineAmount,
            command.SeverityLevel,
            command.EffectiveFrom,
            command.Description,
            command.Status,
            violationType: command.ViolationType);

        await penaltyRuleRepository.AddAsync(penaltyRule, cancellationToken);

        var saveSucceeded = false;
        for (var attempt = 1; attempt <= MaxSaveRetries; attempt++)
        {
            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                saveSucceeded = true;
                break;
            }
            catch (Exception ex) when (IsUniqueConstraintViolation(ex) && attempt < MaxSaveRetries)
            {
                var newGeneratedCode = PenaltyRule.GenerateRandomCode();
                penaltyRule.RegenerateCode(newGeneratedCode);
            }
        }

        if (!saveSucceeded)
        {
            return Result.Failure<PenaltyRuleDetailDto>(
                Error.Conflict("PenaltyRule.CodeCollision", "Không thể sinh mã quy định phạt. Vui lòng thử lại."));
        }

        return Result.Success(MapPenaltyRuleDetail(penaltyRule));
    }

    private static bool IsUniqueConstraintViolation(Exception ex)
    {
        var exceptionText = ex.ToString();
        return exceptionText.Contains("23505") ||
               exceptionText.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
               exceptionText.Contains("unique constraint", StringComparison.OrdinalIgnoreCase);
    }

    private static PenaltyRuleDetailDto MapPenaltyRuleDetail(PenaltyRule rule) => new()
    {
        Id = rule.Id,
        RuleCode = rule.RuleCode,
        Name = rule.Name,
        ViolationType = rule.ViolationType,
        FineAmount = rule.FineAmount,
        SeverityLevel = rule.SeverityLevel,
        Status = rule.Status,
        EffectiveDate = rule.EffectiveDate,
        EffectiveFrom = rule.EffectiveFrom,
        Description = rule.Description,
        PreviousRuleId = rule.PreviousRuleId,
        ViolationsCount = 0,
        CreatedDate = rule.CreatedDate,
        LastModifiedDate = rule.LastModifiedDate
    };
}
