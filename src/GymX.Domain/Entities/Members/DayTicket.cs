using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Members;

public class DayTicket : EntityAuditBase
{
    public Guid? MemberId { get; private set; }
    public string? GuestName { get; private set; }
    public string? GuestPhone { get; private set; }
    public Guid SoldBy { get; private set; }
    public DateOnly TicketDate { get; private set; }
    public decimal PriceAtPurchase { get; private set; }
    public string Status { get; private set; } = TicketStatus.Active;
    public DateTime? UsedAt { get; private set; }

    protected DayTicket() { }

    public static DayTicket Create(Guid soldBy, decimal priceAtPurchase, DateOnly ticketDate, Guid? memberId = null, string? guestName = null, string? guestPhone = null)
    {
        return new DayTicket
        {
            SoldBy = soldBy,
            PriceAtPurchase = priceAtPurchase,
            TicketDate = ticketDate,
            MemberId = memberId,
            GuestName = guestName,
            GuestPhone = guestPhone,
            Status = TicketStatus.Active
        };
    }

    public void MarkAsUsed()
    {
        Status = TicketStatus.Used;
        UsedAt = DateTime.UtcNow;
    }
    
    public void Cancel()
    {
        Status = TicketStatus.Cancelled;
    }
}
