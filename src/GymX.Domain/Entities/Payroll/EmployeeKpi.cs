using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Payroll;
using GymX.Domain.Entities.Employees;

namespace GymX.Domain.Entities.Payroll;

public class EmployeeKpi : EntityAuditBase
{
    public Guid EmployeeId { get; private set; }
    public Guid KpiTemplateId { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }

    public int? TargetPtSessions { get; private set; }
    public decimal TargetRevenue { get; private set; }
    public int? TargetNewMembers { get; private set; }
    public decimal TargetBonusAmount { get; private set; }

    public string? Note { get; private set; }
    public string Status { get; private set; } = KpiStatus.InProgress;
    public Guid AssignedBy { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public virtual Employee Employee { get; private set; } = null!;
    public virtual KpiTemplate KpiTemplate { get; private set; } = null!;

    protected EmployeeKpi() { }

    public static EmployeeKpi Create(
        Guid employeeId,
        Guid kpiTemplateId,
        int month,
        int year,
        decimal targetRevenue,
        decimal targetBonusAmount,
        Guid assignedBy,
        int? targetPtSessions = null,
        int? targetNewMembers = null,
        string? note = null)
    {
        return new EmployeeKpi
        {
            EmployeeId = employeeId,
            KpiTemplateId = kpiTemplateId,
            Month = month,
            Year = year,
            TargetRevenue = targetRevenue < 0 ? 0 : targetRevenue,
            TargetBonusAmount = targetBonusAmount < 0 ? 0 : targetBonusAmount,
            TargetPtSessions = targetPtSessions,
            TargetNewMembers = targetNewMembers,
            Note = note?.Trim(),
            AssignedBy = assignedBy,
            AssignedAt = DateTime.UtcNow,
            Status = KpiStatus.InProgress
        };
    }

    public void UpdateTargets(
        decimal targetRevenue,
        decimal targetBonusAmount,
        int? targetPtSessions,
        int? targetNewMembers,
        string? note)
    {
        TargetRevenue = targetRevenue < 0 ? 0 : targetRevenue;
        TargetBonusAmount = targetBonusAmount < 0 ? 0 : targetBonusAmount;
        TargetPtSessions = targetPtSessions;
        TargetNewMembers = targetNewMembers;
        Note = note?.Trim();
    }

    public void UpdateStatus(string status)
    {
        Status = status.Trim().ToUpperInvariant();
    }
}

