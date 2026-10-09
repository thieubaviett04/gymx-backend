using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Contracts;

namespace GymX.Domain.Entities.Employees;

public class Specialization : EntityAuditBase, ISoftDelete
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Status { get; private set; } = EmployeeStatus.Active;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    protected Specialization() { }

    public static Specialization Create(string name, string? description = null)
    {
        return new Specialization
        {
            Name = name,
            Description = description,
            Status = EmployeeStatus.Active
        };
    }

    public void Deactivate()
    {
        Status = EmployeeStatus.Inactive;
    }
}
