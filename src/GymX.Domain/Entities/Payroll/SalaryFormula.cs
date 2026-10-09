using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Payroll;

public class SalaryFormula : EntityAuditBase
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string BaseRateType { get; private set; } = string.Empty;
    public decimal BaseRate { get; private set; }
    public DateOnly EffectiveDate { get; private set; }
    public string Status { get; private set; } = EmployeeStatus.Active;
    public Guid CreatorId { get; private set; }

    protected SalaryFormula() { }

    public static SalaryFormula Create(string name, string baseRateType, decimal baseRate, DateOnly effectiveDate, Guid creatorId, string? description = null)
    {
        return new SalaryFormula
        {
            Name = name,
            BaseRateType = baseRateType,
            BaseRate = baseRate,
            EffectiveDate = effectiveDate,
            CreatorId = creatorId,
            Description = description,
            Status = EmployeeStatus.Active
        };
    }
}
