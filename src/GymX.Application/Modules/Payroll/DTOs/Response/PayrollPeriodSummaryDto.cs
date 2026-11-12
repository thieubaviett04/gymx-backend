namespace GymX.Application.Modules.Payroll.DTOs.Response;

public sealed class PayrollPeriodSummaryDto
{
    public Guid Id { get; init; }
    public DateOnly PeriodStart { get; init; }
    public DateOnly PeriodEnd { get; init; }
    public Guid SalaryFormulaId { get; init; }
    public string SalaryFormulaName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal TotalSalary { get; init; }
    public decimal TotalBaseSalary { get; init; }
    public decimal TotalPtAmount { get; init; }
    public decimal TotalCommissionAmount { get; init; }
    public decimal TotalKpiBonusAmount { get; init; }
    public decimal TotalPenaltyAmount { get; init; }
    public int TotalEmployees { get; init; }
    public DateTime? CalculatedAt { get; init; }
    public DateTime? ConfirmedAt { get; init; }
    public string? ConfirmedByName { get; init; }
}
