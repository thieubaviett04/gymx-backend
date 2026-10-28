namespace GymX.Application.Modules.Employees.DTOs;

public class UpdateEmployeeRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string Position { get; set; } = string.Empty;
    public decimal BaseSalaryRate { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;

    public Guid? SpecializationId { get; set; }
    public short? ExperienceYears { get; set; }
    public decimal? SalaryCoefficient { get; set; }
    public string? Description { get; set; }
}
