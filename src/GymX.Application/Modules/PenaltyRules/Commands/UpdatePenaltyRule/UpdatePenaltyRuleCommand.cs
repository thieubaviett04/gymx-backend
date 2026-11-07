using GymX.Application.Modules.PenaltyRules.DTOs.Request;
using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Commands.UpdatePenaltyRule;

public record UpdatePenaltyRuleCommand(
    Guid RuleId,
    string Name,
    decimal FineAmount,
    string SeverityLevel,
    DateTime EffectiveFrom,
    string Status,
    string? Description,
    string? ViolationType,
    string? Mode) : IRequest<Result<PenaltyRuleDetailDto>>
{
    public static UpdatePenaltyRuleCommand FromRequest(Guid ruleId, UpdatePenaltyRuleRequest request)
    {
        var effectiveFrom = request.EffectiveDate.HasValue
            ? request.EffectiveDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
            : (request.EffectiveFrom ?? DateTime.UtcNow);

        return new(
            ruleId,
            request.Name,
            request.FineAmount ?? 0,
            request.SeverityLevel,
            effectiveFrom,
            request.Status,
            request.Description,
            request.ViolationType,
            request.Mode);
    }
}
