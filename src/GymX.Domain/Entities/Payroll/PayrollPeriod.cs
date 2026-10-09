using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Payroll;

public class PayrollPeriod : EntityAuditBase
{
    public DateOnly PeriodStart { get; private set; }
    public DateOnly PeriodEnd { get; private set; }
    public Guid SalaryFormulaId { get; private set; }
    public string Status { get; private set; } = PayrollPeriodStatus.Draft;
    public DateTime? CalculatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public Guid? ConfirmedBy { get; private set; }

    protected PayrollPeriod() { }

    public static PayrollPeriod Create(DateOnly periodStart, DateOnly periodEnd, Guid salaryFormulaId)
    {
        return new PayrollPeriod
        {
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            SalaryFormulaId = salaryFormulaId,
            Status = PayrollPeriodStatus.Draft
        };
    }

    public void Confirm(Guid confirmedBy)
    {
        Status = PayrollPeriodStatus.Confirmed;
        ConfirmedBy = confirmedBy;
        ConfirmedAt = DateTime.UtcNow;
    }
}
