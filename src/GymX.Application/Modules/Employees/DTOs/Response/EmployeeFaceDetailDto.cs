namespace GymX.Application.Modules.Employees.DTOs.Response;

public class EmployeeFaceDetailDto
{
    public Guid EmployeeId { get; set; }
    public Guid UserId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public bool HasFaceEnrolled { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Provider { get; set; }
    public string? ExternalFaceId { get; set; }
    public DateTime? EnrolledAt { get; set; }
    public Guid? EnrolledBy { get; set; }
}
