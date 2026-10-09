using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Contracts;

namespace GymX.Domain.Entities.Employees;

public class TrainingService : EntityAuditBase, ISoftDelete
{
    public Guid SpecializationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal Price { get; private set; }
    public string Status { get; private set; } = EmployeeStatus.Active;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    protected TrainingService() { }

    public static TrainingService Create(Guid specializationId, string name, int durationMinutes, decimal price, string? description = null)
    {
        return new TrainingService
        {
            SpecializationId = specializationId,
            Name = name,
            DurationMinutes = durationMinutes,
            Price = price,
            Description = description,
            Status = EmployeeStatus.Active
        };
    }

    public void Deactivate()
    {
        Status = EmployeeStatus.Inactive;
    }
}
