using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Commands.CreateKpiTemplate;

public sealed record CreateKpiTemplateCommand(
    string Name,
    string TargetRole,
    decimal TargetRevenue,
    decimal TargetBonusAmount,
    int? TargetPtSessions = null,
    int? TargetNewMembers = null,
    string? Description = null,
    string? Status = null) : IRequest<Result<KpiTemplateDto>>;
