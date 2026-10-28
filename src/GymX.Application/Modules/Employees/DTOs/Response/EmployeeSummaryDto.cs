namespace GymX.Application.Modules.Employees.DTOs.Response;

public class EmployeeSummaryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? NationalId { get; set; }
    public string Position { get; set; } = string.Empty;
    public decimal BaseSalaryRate { get; set; }
    public DateOnly HiredAt { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? TrainerSpecialization { get; set; }
    public short? TrainerExperienceYears { get; set; }
    public DateTime CreatedDate { get; set; }
}
