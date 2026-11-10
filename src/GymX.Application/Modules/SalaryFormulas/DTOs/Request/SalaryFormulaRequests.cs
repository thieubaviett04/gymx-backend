namespace GymX.Application.Modules.SalaryFormulas.DTOs.Request;

public sealed record CreateSalaryFormulaRequest(
    string Name,
    string TargetRole,
    string BaseRateType,
    decimal BaseRate,
    int StandardWorkDays,
    decimal PricePerPtSession,
    decimal SalesCommissionPercent,
    DateOnly EffectiveDate,
    string? Description);

public sealed record UpdateSalaryFormulaRequest(
    string Name,
    string TargetRole,
    string BaseRateType,
    decimal BaseRate,
    int StandardWorkDays,
    decimal PricePerPtSession,
    decimal SalesCommissionPercent,
    DateOnly EffectiveDate,
    string? Description);

public sealed record ToggleSalaryFormulaStatusRequest(
    bool IsActive);
