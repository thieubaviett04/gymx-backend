namespace GymX.Application.Features.Users.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? EmailVerifiedAt { get; set; }
    public List<string> Roles { get; set; } = [];
    public MemberProfileDto? MemberInfo { get; set; }
    public EmployeeProfileDto? EmployeeInfo { get; set; }
}

public class MemberProfileDto
{
    public string MemberCode { get; set; } = string.Empty;
    public DateOnly JoinDate { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? HealthNote { get; set; }
    public string MemberStatus { get; set; } = string.Empty;
}

public class EmployeeProfileDto
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateOnly HiredAt { get; set; }
    public string? NationalId { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;
}
