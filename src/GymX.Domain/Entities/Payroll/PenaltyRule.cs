using System.Security.Cryptography;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Contracts;

namespace GymX.Domain.Entities.Payroll;

public class PenaltyRule : EntityAuditBase, ISoftDelete
{
    private const string CodeCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string RuleCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string ViolationType { get; private set; } = "GENERAL";
    public decimal FineAmount { get; private set; }
    public string SeverityLevel { get; private set; } = PenaltyRuleSeverity.Medium;
    public string Status { get; private set; } = PenaltyRuleStatus.Applied;
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public DateOnly EffectiveDate { get; private set; }
    public string? Description { get; private set; }

    public Guid? PreviousRuleId { get; private set; }
    public PenaltyRule? PreviousRule { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    protected PenaltyRule() { }

    public static PenaltyRule Create(
        string name,
        decimal fineAmount,
        string severityLevel,
        DateTime effectiveFrom,
        string? description = null,
        string? status = null,
        Guid? previousRuleId = null,
        string? customCode = null)
    {
        var currentUtcTime = DateTime.UtcNow;
        var normalizedEffectiveFrom = effectiveFrom.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(effectiveFrom, DateTimeKind.Utc)
            : effectiveFrom.ToUniversalTime();

        string resolvedStatus;
        if (!string.IsNullOrWhiteSpace(status))
        {
            resolvedStatus = status.Trim().ToUpperInvariant();
        }
        else
        {
            resolvedStatus = normalizedEffectiveFrom > currentUtcTime.AddMinutes(2)
                ? PenaltyRuleStatus.Pending
                : PenaltyRuleStatus.Applied;
        }

        return new PenaltyRule
        {
            RuleCode = !string.IsNullOrWhiteSpace(customCode) ? customCode.Trim().ToUpperInvariant() : GenerateRandomCode(),
            Name = name.Trim(),
            FineAmount = fineAmount,
            SeverityLevel = severityLevel.Trim().ToUpperInvariant(),
            Status = resolvedStatus,
            EffectiveFrom = normalizedEffectiveFrom,
            EffectiveDate = DateOnly.FromDateTime(normalizedEffectiveFrom),
            Description = description?.Trim(),
            PreviousRuleId = previousRuleId
        };
    }

    public static string GenerateRandomCode()
    {
        return RandomNumberGenerator.GetString(CodeCharacters, 6);
    }

    public void RegenerateCode(string newRuleCode)
    {
        RuleCode = newRuleCode.Trim().ToUpperInvariant();
    }

    public void UpdateInfo(
        string name,
        decimal fineAmount,
        string severityLevel,
        DateTime effectiveFrom,
        string status,
        string? description = null)
    {
        Name = name.Trim();
        FineAmount = fineAmount;
        SeverityLevel = severityLevel.Trim().ToUpperInvariant();
        Status = status.Trim().ToUpperInvariant();
        Description = description?.Trim();

        EffectiveFrom = effectiveFrom.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(effectiveFrom, DateTimeKind.Utc)
            : effectiveFrom.ToUniversalTime();
        EffectiveDate = DateOnly.FromDateTime(EffectiveFrom);
    }

    public void Discontinue()
    {
        Status = PenaltyRuleStatus.Discontinued;
        EffectiveTo = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedDate = DateTime.UtcNow;
    }
}
