namespace GymX.Application.Modules.Employees.DTOs.Response;

public class EmployeeProfileDto
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateOnly HiredAt { get; set; }
    public string? NationalId { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;
}
