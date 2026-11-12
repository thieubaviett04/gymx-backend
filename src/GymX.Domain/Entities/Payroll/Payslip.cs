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

    public int PtSessionsCount { get; private set; }
    public decimal PtTeachingAmount { get; private set; }
    public decimal SalesRevenue { get; private set; }
    public decimal CommissionAmount { get; private set; }

    protected Payslip() { }

    public static Payslip Create(
        Guid payrollPeriodId,
        Guid employeeId,
        decimal baseSalary,
        int workedShifts,
        int lateCount,
        int earlyLeaveCount,
        decimal bonusAmount,
        decimal penaltyAmount,
        int ptSessionsCount = 0,
        decimal ptTeachingAmount = 0m,
        decimal salesRevenue = 0m,
        decimal commissionAmount = 0m,
        string? note = null)
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
            PtSessionsCount = ptSessionsCount,
            PtTeachingAmount = ptTeachingAmount,
            SalesRevenue = salesRevenue,
            CommissionAmount = commissionAmount,
            TotalSalary = total,
            Note = note,
            Status = PayslipStatus.Draft,
            GeneratedAt = DateTime.UtcNow
        };
    }

    public void UpdateCalculatedSalary(
        decimal baseSalary,
        int workedShifts,
        int lateCount,
        int earlyLeaveCount,
        decimal bonusAmount,
        decimal penaltyAmount,
        int ptSessionsCount = 0,
        decimal ptTeachingAmount = 0m,
        decimal salesRevenue = 0m,
        decimal commissionAmount = 0m,
        string? note = null)
    {
        var total = baseSalary + bonusAmount - penaltyAmount;
        if (total < 0) total = 0;

        BaseSalary = baseSalary;
        WorkedShifts = workedShifts;
        LateCount = lateCount;
        EarlyLeaveCount = earlyLeaveCount;
        BonusAmount = bonusAmount;
        PenaltyAmount = penaltyAmount;
        PtSessionsCount = ptSessionsCount;
        PtTeachingAmount = ptTeachingAmount;
        SalesRevenue = salesRevenue;
        CommissionAmount = commissionAmount;
        TotalSalary = total;
        Note = note;
        GeneratedAt = DateTime.UtcNow;
    }

    public void FinalizePayslip()
    {
        Status = PayslipStatus.Finalized;
    }
}
