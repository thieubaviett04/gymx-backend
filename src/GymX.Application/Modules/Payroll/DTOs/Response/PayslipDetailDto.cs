namespace GymX.Application.Modules.Payroll.DTOs.Response;

public sealed class PayslipDetailDto
{
    public Guid Id { get; init; }
    public Guid PayrollPeriodId { get; init; }
    public int PeriodMonth { get; init; }
    public int PeriodYear { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeCode { get; init; } = string.Empty;
    public string EmployeeName { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public decimal BaseSalary { get; init; }
    public int WorkedShifts { get; init; }
    public int LateCount { get; init; }
    public int EarlyLeaveCount { get; init; }
    public decimal BonusAmount { get; init; }
    public decimal PenaltyAmount { get; init; }
    public decimal TotalSalary { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Note { get; init; }
    public DateTime GeneratedAt { get; init; }
    public int PtSessionsCount { get; init; }
    public decimal PtTeachingAmount { get; init; }
    public decimal SalesRevenue { get; init; }
    public decimal CommissionAmount { get; init; }
}
