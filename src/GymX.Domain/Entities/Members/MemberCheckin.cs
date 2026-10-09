using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Members;

public class MemberCheckin : EntityAuditBase
{
    public Guid MemberId { get; private set; }
    public Guid? MemberPackageId { get; private set; }
    public DateTime CheckInAt { get; private set; }
    public DateTime? CheckOutAt { get; private set; }
    public string Method { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public Guid? VerifiedBy { get; private set; }
    public string? Note { get; private set; }

    protected MemberCheckin() { }

    public static MemberCheckin Create(Guid memberId, Guid? memberPackageId, string method, string status, Guid? verifiedBy = null, string? note = null)
    {
        return new MemberCheckin
        {
            MemberId = memberId,
            MemberPackageId = memberPackageId,
            CheckInAt = DateTime.UtcNow,
            Method = method,
            Status = status,
            VerifiedBy = verifiedBy,
            Note = note
        };
    }

    public void MarkCheckOut()
    {
        CheckOutAt = DateTime.UtcNow;
    }
}
