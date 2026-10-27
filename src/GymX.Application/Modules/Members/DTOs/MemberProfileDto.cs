namespace GymX.Application.Modules.Members.DTOs;

public class MemberProfileDto
{
    public string MemberCode { get; set; } = string.Empty;
    public DateOnly JoinDate { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? HealthNote { get; set; }
    public string MemberStatus { get; set; } = string.Empty;
}
