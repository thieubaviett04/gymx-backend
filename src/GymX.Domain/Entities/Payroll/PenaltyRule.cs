using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Contracts;

namespace GymX.Domain.Entities.Payroll;

public class PenaltyRule : EntityAuditBase, ISoftDelete
{
    public string Name { get; private set; } = string.Empty;
    public string ViolationType { get; private set; } = string.Empty;
    public decimal FineAmount { get; private set; }
    public string? SeverityLevel { get; private set; }
    public string? Description { get; private set; }
    public string Status { get; private set; } = EmployeeStatus.Active;
    public DateOnly EffectiveDate { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    protected PenaltyRule() { }

    public static PenaltyRule Create(string name, string violationType, decimal fineAmount, DateOnly effectiveDate, string? severityLevel = null, string? description = null)
    {
        return new PenaltyRule
        {
            Name = name,
            ViolationType = violationType,
            FineAmount = fineAmount,
            EffectiveDate = effectiveDate,
            SeverityLevel = severityLevel,
            Description = description,
            Status = EmployeeStatus.Active
        };
    }
}
