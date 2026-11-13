using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Commands.AssignEmployeeKpis;

public sealed record AssignEmployeeKpisCommand(
    Guid KpiTemplateId,
    int Month,
    int Year,
    List<Guid> EmployeeIds,
    string? Note,
    Guid CurrentUserId) : IRequest<Result<AssignKpiResultDto>>;
