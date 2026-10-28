using GymX.Domain.Common.Models;

namespace GymX.Domain.Entities.Employees;

public class Trainer : EntityAuditBase
{
    public Guid EmployeeId { get; private set; }
    public Guid SpecializationId { get; private set; }
    public short ExperienceYears { get; private set; }
    public decimal? SalaryCoefficient { get; private set; }
    public string? Description { get; private set; }
    public decimal RatingAvg { get; private set; }
    public int RatingCount { get; private set; }

    protected Trainer() { }

    public static Trainer Create(Guid employeeId, Guid specializationId, short experienceYears, decimal? salaryCoefficient = null, string? description = null)
    {
        return new Trainer
        {
            EmployeeId = employeeId,
            SpecializationId = specializationId,
            ExperienceYears = experienceYears,
            SalaryCoefficient = salaryCoefficient,
            Description = description,
            RatingAvg = 0,
            RatingCount = 0
        };
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void UpdateTrainerInfo(Guid specializationId, short experienceYears, decimal? salaryCoefficient, string? description)
    {
        SpecializationId = specializationId;
        ExperienceYears = experienceYears;
        SalaryCoefficient = salaryCoefficient;
        Description = description;
    }

    public void UpdateRating(decimal newRating)
    {
        decimal totalRating = (RatingAvg * RatingCount) + newRating;
        RatingCount++;
        RatingAvg = totalRating / RatingCount;
    }
}
