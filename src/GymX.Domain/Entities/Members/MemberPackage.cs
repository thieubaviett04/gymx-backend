using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Members;

public class MemberPackage : EntityAuditBase
{
    public Guid MemberId { get; private set; }
    public Guid PackageId { get; private set; }
    public decimal PriceAtPurchase { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public int? RemainingCheckins { get; private set; }
    public string Status { get; private set; } = MemberPackageStatus.PendingPayment;
    public Guid? RegisteredBy { get; private set; }

    protected MemberPackage() { }

    public static MemberPackage Create(Guid memberId, Guid packageId, decimal priceAtPurchase, DateOnly startDate, DateOnly endDate, int? remainingCheckins, Guid? registeredBy)
    {
        return new MemberPackage
        {
            MemberId = memberId,
            PackageId = packageId,
            PriceAtPurchase = priceAtPurchase,
            StartDate = startDate,
            EndDate = endDate,
            RemainingCheckins = remainingCheckins,
            RegisteredBy = registeredBy,
            Status = MemberPackageStatus.PendingPayment
        };
    }

    public void Activate()
    {
        Status = MemberPackageStatus.Active;
    }

    public void Expire()
    {
        Status = MemberPackageStatus.Expired;
    }

    public void Cancel()
    {
        Status = MemberPackageStatus.Cancelled;
    }

    public void DeductCheckin()
    {
        if (RemainingCheckins.HasValue && RemainingCheckins.Value > 0)
        {
            RemainingCheckins--;
        }
    }
}
