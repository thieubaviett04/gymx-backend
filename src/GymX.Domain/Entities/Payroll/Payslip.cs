using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Payroll;

public class Payslip : EntityAuditBase
{
    public Guid PayrollPeriodId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public decimal BaseSalary { get; private set; }
    public int WorkedShifts { get; private set; }
    public int LateCount { get; private set; }
    public int EarlyLeaveCount { get; private set; }
    public decimal BonusAmount { get; private set; }
    public decimal PenaltyAmount { get; private set; }
    public decimal TotalSalary { get; private set; }
    public string Status { get; private set; } = PayslipStatus.Draft;
    public string? Note { get; private set; }
    public DateTime GeneratedAt { get; private set; }

    protected Payslip() { }

    public static Payslip Create(Guid payrollPeriodId, Guid employeeId, decimal baseSalary, int workedShifts, int lateCount, int earlyLeaveCount, decimal bonusAmount, decimal penaltyAmount, string? note = null)
    {
        var total = baseSalary + bonusAmount - penaltyAmount;
        if (total < 0) total = 0;

        return new Payslip
        {
            PayrollPeriodId = payrollPeriodId,
            EmployeeId = employeeId,
            BaseSalary = baseSalary,
            WorkedShifts = workedShifts,
            LateCount = lateCount,
            EarlyLeaveCount = earlyLeaveCount,
            BonusAmount = bonusAmount,
            PenaltyAmount = penaltyAmount,
            TotalSalary = total,
            Note = note,
            Status = PayslipStatus.Draft,
            GeneratedAt = DateTime.UtcNow
        };
    }

    public void FinalizePayslip()
    {
        Status = PayslipStatus.Finalized;
    }
}
