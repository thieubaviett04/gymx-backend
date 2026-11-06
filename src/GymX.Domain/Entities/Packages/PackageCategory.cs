using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Packages;
using GymX.Domain.Contracts;

namespace GymX.Domain.Entities.Packages;

public class PackageCategory : EntityAuditBase, ISoftDelete
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Status { get; private set; } = PackageCategoryStatus.Active;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<Package> Packages { get; private set; } = new List<Package>();

    protected PackageCategory() { }

    public static PackageCategory Create(string code, string name, string? description = null)
    {
        return new PackageCategory
        {
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Status = PackageCategoryStatus.Active
        };
    }

    public void Update(string name, string? description, string status)
    {
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        if (PackageCategoryStatus.IsValid(status))
        {
            Status = status.Trim().ToUpperInvariant();
        }
    }

    public void Activate()
    {
        Status = PackageCategoryStatus.Active;
    }

    public void Deactivate()
    {
        Status = PackageCategoryStatus.Inactive;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedDate = DateTime.UtcNow;
    }
}
