using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetKpiTemplateById;

public sealed record GetKpiTemplateByIdQuery(Guid Id) : IRequest<Result<KpiTemplateDto>>;
