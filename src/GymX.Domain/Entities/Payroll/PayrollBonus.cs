using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Payroll;

namespace GymX.Domain.Entities.Payroll;

public class PayrollBonus : EntityAuditBase
{
    public Guid EmployeeId { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }
    public string BonusType { get; private set; } = PayrollBonusTypes.Kpi;
    public decimal Amount { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public Guid? PayrollPeriodId { get; private set; }

    protected PayrollBonus() { }

    public static PayrollBonus Create(
        Guid employeeId,
        int month,
        int year,
        string bonusType,
        decimal amount,
        string reason,
        Guid? payrollPeriodId = null)
    {
        return new PayrollBonus
        {
            EmployeeId = employeeId,
            Month = month,
            Year = year,
            BonusType = string.IsNullOrWhiteSpace(bonusType) ? PayrollBonusTypes.Kpi : bonusType.Trim().ToUpperInvariant(),
            Amount = amount < 0 ? 0 : amount,
            Reason = reason.Trim(),
            PayrollPeriodId = payrollPeriodId
        };
    }

    public void Update(decimal amount, string reason, string bonusType)
    {
        Amount = amount < 0 ? 0 : amount;
        Reason = reason.Trim();
        BonusType = string.IsNullOrWhiteSpace(bonusType) ? PayrollBonusTypes.Kpi : bonusType.Trim().ToUpperInvariant();
    }

    public void AssignPayrollPeriod(Guid payrollPeriodId)
    {
        PayrollPeriodId = payrollPeriodId;
    }
}
