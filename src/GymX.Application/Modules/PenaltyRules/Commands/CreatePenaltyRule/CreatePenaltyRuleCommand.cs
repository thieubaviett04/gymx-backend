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
    public static CreatePenaltyRuleCommand FromRequest(CreatePenaltyRuleRequest request) =>
        new(
            request.Name,
            request.FineAmount ?? 0,
            request.SeverityLevel,
            request.EffectiveFrom ?? DateTime.UtcNow,
            request.Status,
            request.Description,
            request.ViolationType);
}
