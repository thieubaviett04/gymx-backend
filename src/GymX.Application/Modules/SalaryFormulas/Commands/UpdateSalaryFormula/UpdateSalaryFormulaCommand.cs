using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.SalaryFormulas.Commands.UpdateSalaryFormula;

public sealed record UpdateSalaryFormulaCommand(
    Guid Id,
    string Name,
    string TargetRole,
    string BaseRateType,
    decimal BaseRate,
    int StandardWorkDays,
    decimal PricePerPtSession,
    decimal SalesCommissionPercent,
    DateOnly EffectiveDate,
    string? Description) : IRequest<Result<SalaryFormulaDto>>;
