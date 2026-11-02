using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Commands.DiscontinuePenaltyRule;

public record DiscontinuePenaltyRuleCommand(Guid RuleId) : IRequest<Result>;
