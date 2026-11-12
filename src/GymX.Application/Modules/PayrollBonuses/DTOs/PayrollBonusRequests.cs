namespace GymX.Application.Modules.PayrollBonuses.DTOs;

public class CreatePayrollBonusRequest
{
    public int Month { get; set; }
    public int Year { get; set; }
    public string BonusType { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string? Position { get; set; }
    public List<Guid>? EmployeeIds { get; set; }
    public Guid? EmployeeId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Content { get; set; }
}

public class UpdatePayrollBonusRequest
{
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string BonusType { get; set; } = string.Empty;
}
