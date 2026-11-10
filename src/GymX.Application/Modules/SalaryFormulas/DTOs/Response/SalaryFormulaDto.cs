namespace GymX.Application.Modules.SalaryFormulas.DTOs.Response;

public sealed class SalaryFormulaDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string TargetRole { get; init; } = string.Empty;
    public string BaseRateType { get; init; } = string.Empty;
    public decimal BaseRate { get; init; }
    public int StandardWorkDays { get; init; }
    public decimal PricePerPtSession { get; init; }
    public decimal SalesCommissionPercent { get; init; }
    public DateOnly EffectiveDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string CreatorName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
