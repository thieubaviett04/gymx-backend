using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetEmployeeKpiDetail;

public sealed record GetEmployeeKpiDetailQuery(Guid Id) : IRequest<Result<EmployeeKpiDetailDto>>;
