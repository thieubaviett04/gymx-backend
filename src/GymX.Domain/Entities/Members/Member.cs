using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Contracts;

namespace GymX.Domain.Entities.Members;

public class Member : EntityAuditBase, ISoftDelete
{
    public Guid UserId { get; private set; }
    public string MemberCode { get; private set; } = string.Empty;
    public DateOnly JoinDate { get; private set; }
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactPhone { get; private set; }
    public string? HealthNote { get; private set; }
    public string MemberStatus { get; private set; } = Constants.MemberStatus.Active;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    protected Member() { }

    public static Member Create(Guid userId, string memberCode, DateOnly joinDate, string? emergencyContactName = null, string? emergencyContactPhone = null, string? healthNote = null)
    {
        return new Member
        {
            UserId = userId,
            MemberCode = memberCode,
            JoinDate = joinDate,
            EmergencyContactName = emergencyContactName,
            EmergencyContactPhone = emergencyContactPhone,
            HealthNote = healthNote,
            MemberStatus = Constants.MemberStatus.Active
        };
    }

    public void Suspend()
    {
        MemberStatus = Constants.MemberStatus.Suspended;
    }

    public void Reactivate()
    {
        MemberStatus = Constants.MemberStatus.Active;
    }
}
