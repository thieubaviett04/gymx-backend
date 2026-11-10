namespace GymX.Application.Modules.Payroll.DTOs.Request;

public sealed record CalculatePayrollRequest(
    int Month,
    int Year);

public sealed record LockPayrollPeriodRequest;
