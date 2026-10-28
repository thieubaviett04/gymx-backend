using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Contracts;

namespace GymX.Domain.Entities.Employees;

public class Employee : EntityAuditBase, ISoftDelete
{
    public Guid UserId { get; private set; }
    public string EmployeeCode { get; private set; } = string.Empty;
    public string? NationalId { get; private set; }
    public string Position { get; private set; } = string.Empty;
    public decimal BaseSalaryRate { get; private set; }
    public DateOnly HiredAt { get; private set; }
    public string EmploymentStatus { get; private set; } = EmployeeStatus.Active;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    protected Employee() { }

    public static Employee Create(Guid userId, string employeeCode, string position, decimal baseSalaryRate, DateOnly hiredAt, string? nationalId = null)
    {
        return new Employee
        {
            UserId = userId,
            EmployeeCode = employeeCode,
            Position = position,
            BaseSalaryRate = baseSalaryRate,
            HiredAt = hiredAt,
            NationalId = nationalId,
            EmploymentStatus = EmployeeStatus.Active
        };
    }

    public void UpdateNationalId(string? nationalId)
    {
        NationalId = nationalId;
    }

    public void UpdateInfo(string position, decimal baseSalaryRate, string employmentStatus)
    {
        Position = position;
        BaseSalaryRate = baseSalaryRate;
        EmploymentStatus = employmentStatus;
    }

    public void Restore(string position, decimal baseSalaryRate, DateOnly hiredAt)
    {
        IsDeleted = false;
        DeletedDate = null;
        Position = position;
        BaseSalaryRate = baseSalaryRate;
        HiredAt = hiredAt;
        EmploymentStatus = EmployeeStatus.Active;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedDate = DateTime.UtcNow;
        EmploymentStatus = EmployeeStatus.Inactive;
    }

    public void Terminate()
    {
        EmploymentStatus = EmployeeStatus.Inactive;
    }
}
