using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Application.Modules.Members.DTOs.Response;
using GymX.Application.Modules.Trainers.DTOs.Response;

namespace GymX.Application.Modules.Users.DTOs.Response;

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
    public TrainerProfileDto? TrainerInfo { get; set; }
}
