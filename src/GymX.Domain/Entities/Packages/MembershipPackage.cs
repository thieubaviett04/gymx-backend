using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Contracts;

namespace GymX.Domain.Entities.Packages;

public class MembershipPackage : EntityAuditBase, ISoftDelete
{
    public string PackageCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int DurationDays { get; private set; }
    public int? MaxCheckins { get; private set; }
    public string? Description { get; private set; }
    public string Status { get; private set; } = PackageStatus.Active;
    public DateOnly EffectiveStartDate { get; private set; }
    public DateOnly? EffectiveEndDate { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    protected MembershipPackage() { }

    public static MembershipPackage Create(string packageCode, string name, decimal price, int durationDays, DateOnly effectiveStartDate, int? maxCheckins = null, string? description = null, DateOnly? effectiveEndDate = null)
    {
        return new MembershipPackage
        {
            PackageCode = packageCode,
            Name = name,
            Price = price,
            DurationDays = durationDays,
            MaxCheckins = maxCheckins,
            Description = description,
            EffectiveStartDate = effectiveStartDate,
            EffectiveEndDate = effectiveEndDate,
            Status = PackageStatus.Active
        };
    }

    public void Deactivate()
    {
        Status = PackageStatus.Inactive;
    }

    public void Activate()
    {
        Status = PackageStatus.Active;
    }
}
