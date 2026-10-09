using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Finance;

public class Invoice : EntityAuditBase
{
    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid? MemberId { get; private set; }
    public string? CustomerName { get; private set; }
    public string? CustomerPhone { get; private set; }
    public string ReferenceType { get; private set; } = string.Empty;
    public Guid ReferenceId { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Status { get; private set; } = PaymentTransactionStatus.Pending;
    public DateTime IssuedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public Guid? CreatorId { get; private set; }

    protected Invoice() { }

    public static Invoice Create(string invoiceNumber, string referenceType, Guid referenceId, decimal subtotal, decimal totalAmount, Guid? memberId = null, string? customerName = null, string? customerPhone = null, Guid? creatorId = null)
    {
        return new Invoice
        {
            InvoiceNumber = invoiceNumber,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Subtotal = subtotal,
            TotalAmount = totalAmount,
            MemberId = memberId,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            CreatorId = creatorId,
            IssuedAt = DateTime.UtcNow,
            Status = InvoiceStatus.Pending
        };
    }

    public void MarkAsPaid()
    {
        Status = InvoiceStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }
    
    public void Cancel()
    {
        Status = InvoiceStatus.Cancelled;
    }
}
