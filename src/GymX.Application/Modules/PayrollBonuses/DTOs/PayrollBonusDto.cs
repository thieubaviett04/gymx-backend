namespace GymX.Application.Modules.PayrollBonuses.DTOs;

public sealed class PayrollBonusDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeCode { get; init; } = string.Empty;
    public string EmployeeName { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public int Month { get; init; }
    public int Year { get; init; }
    public string BonusType { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? MonthlyPerformance { get; init; }
    public string? PayrollPeriodStatus { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed class PayrollBonusPagedResultDto
{
    public IReadOnlyCollection<PayrollBonusDto> Items { get; init; } = [];
    public decimal TotalBonusAmount { get; init; }
    public int TotalCount { get; init; }
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public sealed class BonusEmployeeLookupDto
{
    public Guid EmployeeId { get; init; }
    public string EmployeeCode { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public string? MonthlyPerformance { get; init; }
}

