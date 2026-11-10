namespace GymX.Application.Modules.SalaryFormulas.DTOs.Response;

public sealed class SalaryFormulaLookupDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string TargetRole { get; init; } = string.Empty;
    public decimal BaseRate { get; init; }
    public DateOnly EffectiveDate { get; init; }
}
