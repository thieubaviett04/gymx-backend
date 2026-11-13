using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Payroll;
using GymX.Domain.Contracts;

namespace GymX.Domain.Entities.Payroll;

public class KpiTemplate : EntityAuditBase, ISoftDelete
{
    public string Name { get; private set; } = string.Empty;
    public string TargetRole { get; private set; } = string.Empty;
    public int? TargetPtSessions { get; private set; }
    public decimal TargetRevenue { get; private set; }
    public int? TargetNewMembers { get; private set; }
    public decimal TargetBonusAmount { get; private set; }
    public string? Description { get; private set; }
    public string Status { get; private set; } = KpiTemplateStatus.Active;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    protected KpiTemplate() { }

    public static KpiTemplate Create(
        string name,
        string targetRole,
        decimal targetRevenue,
        decimal targetBonusAmount,
        int? targetPtSessions = null,
        int? targetNewMembers = null,
        string? description = null,
        string? status = null)
    {
        return new KpiTemplate
        {
            Name = name.Trim(),
            TargetRole = targetRole.Trim().ToUpperInvariant(),
            TargetRevenue = targetRevenue < 0 ? 0 : targetRevenue,
            TargetBonusAmount = targetBonusAmount < 0 ? 0 : targetBonusAmount,
            TargetPtSessions = targetPtSessions,
            TargetNewMembers = targetNewMembers,
            Description = description?.Trim(),
            Status = !string.IsNullOrWhiteSpace(status) ? status.Trim().ToUpperInvariant() : KpiTemplateStatus.Active
        };
    }

    public void Update(
        string name,
        string targetRole,
        decimal targetRevenue,
        decimal targetBonusAmount,
        int? targetPtSessions,
        int? targetNewMembers,
        string? description,
        string status)
    {
        Name = name.Trim();
        TargetRole = targetRole.Trim().ToUpperInvariant();
        TargetRevenue = targetRevenue < 0 ? 0 : targetRevenue;
        TargetBonusAmount = targetBonusAmount < 0 ? 0 : targetBonusAmount;
        TargetPtSessions = targetPtSessions;
        TargetNewMembers = targetNewMembers;
        Description = description?.Trim();
        Status = status.Trim().ToUpperInvariant();
    }

    public void Deactivate()
    {
        Status = KpiTemplateStatus.Inactive;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedDate = DateTime.UtcNow;
        Status = KpiTemplateStatus.Inactive;
    }
}

