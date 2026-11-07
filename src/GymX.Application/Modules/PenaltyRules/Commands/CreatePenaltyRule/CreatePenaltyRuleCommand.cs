using GymX.Application.Modules.PenaltyRules.DTOs.Request;
using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
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
        var effectiveFrom = request.EffectiveDate.HasValue
            ? request.EffectiveDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
            : (request.EffectiveFrom ?? DateTime.UtcNow);

        return new(
            request.Name,
            request.FineAmount ?? 0,
            request.SeverityLevel,
            effectiveFrom,
            request.Status,
            request.Description,
            request.ViolationType);
    }
}
