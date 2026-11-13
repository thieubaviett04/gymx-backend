using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetMyKpi;

public sealed record GetMyKpiQuery(Guid UserId, int Month, int Year) : IRequest<Result<MyKpiDto>>;
