using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetKpiTemplatesLookup;

public sealed record GetKpiTemplatesLookupQuery(string? TargetRole) : IRequest<Result<List<KpiTemplateLookupDto>>>;
