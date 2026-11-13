using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetKpiTemplates;

public sealed record GetKpiTemplatesQuery(
    string? Search,
    string? TargetRole,
    string? Status,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<KpiTemplateSummaryDto>>>;
