using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Queries.GetPenaltyRuleById;

public record GetPenaltyRuleByIdQuery(Guid RuleId) : IRequest<Result<PenaltyRuleDetailDto>>;
