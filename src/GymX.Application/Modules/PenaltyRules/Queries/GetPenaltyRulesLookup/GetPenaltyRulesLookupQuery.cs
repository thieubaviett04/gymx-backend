using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Queries.GetPenaltyRulesLookup;

public record GetPenaltyRulesLookupQuery(string? TargetAudience = null)
    : IRequest<Result<List<PenaltyRuleLookupDto>>>;

