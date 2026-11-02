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
    private const int MaxSaveRetries = 3;

    public async Task<Result<PenaltyRuleDetailDto>> Handle(UpdatePenaltyRuleCommand command, CancellationToken cancellationToken)
    {
        var existingPenaltyRule = await penaltyRuleRepository.GetByIdAsync(command.RuleId, cancellationToken);
        if (existingPenaltyRule is null || existingPenaltyRule.IsDeleted)
        {
            return Result.Failure<PenaltyRuleDetailDto>(
                Error.NotFound("PenaltyRule.NotFound", "Không tìm thấy thông tin quy định phạt."));
        }

        var isPenaltyRuleApplied = await penaltyRuleRepository.IsRuleAppliedAsync(existingPenaltyRule.Id, cancellationToken);

        if (isPenaltyRuleApplied)
        {
            if (command.Mode == "DISCONTINUE_AND_CREATE")
            {
                existingPenaltyRule.Discontinue();
                penaltyRuleRepository.Update(existingPenaltyRule);

                var newPenaltyRule = PenaltyRule.Create(
                    command.Name,
                    command.FineAmount,
                    command.SeverityLevel,
                    DateTime.UtcNow,
                    command.Description,
                    status: command.Status,
                    previousRuleId: existingPenaltyRule.Id);

                await penaltyRuleRepository.AddAsync(newPenaltyRule, cancellationToken);

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
                        newPenaltyRule.RegenerateCode(newGeneratedCode);
                    }
                }

                if (!saveSucceeded)
                {
                    return Result.Failure<PenaltyRuleDetailDto>(
                        Error.Conflict("PenaltyRule.CodeCollision", "Không thể sinh mã cho quy định phạt mới. Vui lòng thử lại."));
                }

                return Result.Success(MapPenaltyRuleDetail(newPenaltyRule, previousCode: existingPenaltyRule.RuleCode));
            }

            return Result.Failure<PenaltyRuleDetailDto>(
                Error.Conflict("PenaltyRule.InUse", "Quy định này đang được áp dụng. Bạn có xác nhận thay đổi không?"));
        }

        existingPenaltyRule.UpdateInfo(
            command.Name,
            command.FineAmount,
            command.SeverityLevel,
            command.EffectiveFrom,
            command.Status,
            command.Description);

        penaltyRuleRepository.Update(existingPenaltyRule);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var appliedViolationsCount = await penaltyRuleRepository.CountViolationsAsync(existingPenaltyRule.Id, cancellationToken);
        return Result.Success(MapPenaltyRuleDetail(existingPenaltyRule, violationsCount: appliedViolationsCount));
    }

    private static bool IsUniqueConstraintViolation(Exception ex)
    {
        var exceptionText = ex.ToString();
        return exceptionText.Contains("23505") ||
               exceptionText.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
               exceptionText.Contains("unique constraint", StringComparison.OrdinalIgnoreCase);
    }

    private static PenaltyRuleDetailDto MapPenaltyRuleDetail(PenaltyRule rule, string? previousCode = null, int violationsCount = 0) => new()
    {
        Id = rule.Id,
        RuleCode = rule.RuleCode,
        Name = rule.Name,
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
