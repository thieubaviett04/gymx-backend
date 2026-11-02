using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Commands.DeletePenaltyRule;

public record DeletePenaltyRuleCommand(Guid RuleId) : IRequest<Result>;
