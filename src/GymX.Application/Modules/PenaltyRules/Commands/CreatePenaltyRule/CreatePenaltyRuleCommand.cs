using GymX.Application.Modules.PenaltyRules.DTOs.Request;
using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Commands.CreatePenaltyRule;

public record CreatePenaltyRuleCommand(
    string Name,
    decimal FineAmount,
    string SeverityLevel,
    DateTime EffectiveFrom,
    string? Status,
    string? Description,
    string? ViolationType) : IRequest<Result<PenaltyRuleDetailDto>>
{
    public static CreatePenaltyRuleCommand FromRequest(CreatePenaltyRuleRequest request)
    {
        DateTime effectiveFrom;
        if (request.EffectiveDate.HasValue && request.EffectiveDate.Value.Year >= 2000)
        {
            effectiveFrom = request.EffectiveDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        }
        else if (request.EffectiveFrom.HasValue && request.EffectiveFrom.Value.Year >= 2000)
        {
            effectiveFrom = request.EffectiveFrom.Value;
        }
        else
        {
            effectiveFrom = DateTime.UtcNow;
        }

        var isMember = string.Equals(request.ViolationType?.Trim(), RuleTargetAudience.Member, StringComparison.OrdinalIgnoreCase);
        var fineAmount = isMember ? 0 : (request.FineAmount ?? 0);

        return new(
            request.Name,
            fineAmount,
            request.SeverityLevel,
            effectiveFrom,
            string.IsNullOrWhiteSpace(request.Status) ? PenaltyRuleStatus.Active : request.Status,
            request.Description,
            request.ViolationType);
    }
}
