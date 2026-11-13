using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Commands.UpdateKpiTemplate;

public sealed record UpdateKpiTemplateCommand(
    Guid Id,
    string Name,
    string TargetRole,
    decimal TargetRevenue,
    decimal TargetBonusAmount,
    int? TargetPtSessions,
    int? TargetNewMembers,
    string? Description,
    string Status) : IRequest<Result<KpiTemplateDto>>;
