using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Payroll;

public class SalaryFormula : EntityAuditBase
{
    public string Name { get; private set; } = string.Empty;
    public string TargetRole { get; private set; } = "ALL";
    public string? Description { get; private set; }
    public string BaseRateType { get; private set; } = SalaryBaseRateType.Monthly;
    public decimal BaseRate { get; private set; }
    public int StandardWorkDays { get; private set; } = 26;
    public decimal PricePerPtSession { get; private set; }
    public decimal SalesCommissionPercent { get; private set; }
    public DateOnly EffectiveDate { get; private set; }
    public string Status { get; private set; } = EmployeeStatus.Active;
    public Guid CreatorId { get; private set; }

    protected SalaryFormula() { }

    public static SalaryFormula Create(
        string name,
        string targetRole,
        string baseRateType,
        decimal baseRate,
        int standardWorkDays,
        decimal pricePerPtSession,
        decimal salesCommissionPercent,
        DateOnly effectiveDate,
        Guid creatorId,
        string? description = null)
    {
        return new SalaryFormula
        {
            Name = name.Trim(),
            TargetRole = targetRole.Trim().ToUpperInvariant(),
            BaseRateType = baseRateType.Trim().ToUpperInvariant(),
            BaseRate = baseRate,
            StandardWorkDays = standardWorkDays,
            PricePerPtSession = pricePerPtSession,
            SalesCommissionPercent = salesCommissionPercent,
            EffectiveDate = effectiveDate,
            CreatorId = creatorId,
            Description = description?.Trim(),
            Status = EmployeeStatus.Active
        };
    }

    public void Update(
        string name,
        string targetRole,
        string baseRateType,
        decimal baseRate,
        int standardWorkDays,
        decimal pricePerPtSession,
        decimal salesCommissionPercent,
        DateOnly effectiveDate,
        string? description)
    {
        Name = name.Trim();
        TargetRole = targetRole.Trim().ToUpperInvariant();
        BaseRateType = baseRateType.Trim().ToUpperInvariant();
        BaseRate = baseRate;
        StandardWorkDays = standardWorkDays;
        PricePerPtSession = pricePerPtSession;
        SalesCommissionPercent = salesCommissionPercent;
        EffectiveDate = effectiveDate;
        Description = description?.Trim();
    }

    public void Activate()
    {
        Status = EmployeeStatus.Active;
    }

    public void Deactivate()
    {
        Status = EmployeeStatus.Inactive;
    }
}
