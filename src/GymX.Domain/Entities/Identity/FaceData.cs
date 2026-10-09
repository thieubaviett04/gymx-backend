using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Identity;

public class FaceData : EntityAuditBase
{
    public Guid UserId { get; private set; }
    public string Provider { get; private set; } = FaceDataProvider.FacePlusPlus;
    public string? ExternalFaceId { get; private set; }
    public string? FaceTemplate { get; private set; }
    public Guid? EnrolledBy { get; private set; }
    public string Status { get; private set; } = "ACTIVE";
    public DateTime EnrolledAt { get; private set; }

    protected FaceData() { }

    public static FaceData Create(Guid userId, string externalFaceId, string faceTemplate, Guid? enrolledBy = null)
    {
        return new FaceData
        {
            UserId = userId,
            ExternalFaceId = externalFaceId,
            FaceTemplate = faceTemplate,
            EnrolledBy = enrolledBy,
            Status = "ACTIVE",
            EnrolledAt = DateTime.UtcNow
        };
    }

    public void Deactivate()
    {
        Status = "INACTIVE";
    }
}
