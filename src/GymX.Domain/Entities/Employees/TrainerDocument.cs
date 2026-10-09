using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Contracts;

namespace GymX.Domain.Entities.Employees;

public class TrainerDocument : EntityAuditBase, ISoftDelete
{
    public Guid TrainerId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public string DocumentName { get; private set; } = string.Empty;
    public string? IssuingOrganization { get; private set; }
    public string? DocumentNumber { get; private set; }
    public DateOnly? IssuedDate { get; private set; }
    public DateOnly? ExpiryDate { get; private set; }
    public string FileUrl { get; private set; } = string.Empty;
    public string VerificationStatus { get; private set; } = Constants.VerificationStatus.Pending;
    public Guid? VerifiedBy { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    protected TrainerDocument() { }

    public static TrainerDocument Create(Guid trainerId, string documentType, string documentName, string fileUrl, string? issuingOrganization = null, string? documentNumber = null, DateOnly? issuedDate = null, DateOnly? expiryDate = null)
    {
        return new TrainerDocument
        {
            TrainerId = trainerId,
            DocumentType = documentType,
            DocumentName = documentName,
            FileUrl = fileUrl,
            IssuingOrganization = issuingOrganization,
            DocumentNumber = documentNumber,
            IssuedDate = issuedDate,
            ExpiryDate = expiryDate,
            VerificationStatus = Constants.VerificationStatus.Pending
        };
    }

    public void Verify(Guid verifiedBy)
    {
        VerificationStatus = Constants.VerificationStatus.Verified;
        VerifiedBy = verifiedBy;
        VerifiedAt = DateTime.UtcNow;
    }

    public void Reject(Guid verifiedBy, string reason)
    {
        VerificationStatus = Constants.VerificationStatus.Rejected;
        VerifiedBy = verifiedBy;
        VerifiedAt = DateTime.UtcNow;
        RejectionReason = reason;
    }
}
