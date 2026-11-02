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
    string? Mode) : IRequest<Result<PenaltyRuleDetailDto>>
{
    public static UpdatePenaltyRuleCommand FromRequest(Guid ruleId, UpdatePenaltyRuleRequest request) =>
        new(
            ruleId,
            request.Name,
            request.FineAmount ?? 0,
            request.SeverityLevel,
            request.EffectiveFrom,
            request.Status,
            request.Description,
            request.Mode);
}
