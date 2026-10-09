using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Finance;

public class Payment : EntityAuditBase
{
    public Guid InvoiceId { get; private set; }
    public string Method { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string? GatewayTransactionRef { get; private set; }
    public string Status { get; private set; } = PaymentTransactionStatus.Pending;
    public Guid? ReceivedBy { get; private set; }
    public DateTime? PaidAt { get; private set; }

    protected Payment() { }

    public static Payment Create(Guid invoiceId, string method, decimal amount, string? gatewayTransactionRef = null, Guid? receivedBy = null)
    {
        return new Payment
        {
            InvoiceId = invoiceId,
            Method = method,
            Amount = amount,
            GatewayTransactionRef = gatewayTransactionRef,
            ReceivedBy = receivedBy,
            Status = PaymentTransactionStatus.Pending
        };
    }

    public void MarkAsSuccess()
    {
        Status = PaymentTransactionStatus.Success;
        PaidAt = DateTime.UtcNow;
    }
    
    public void Fail()
    {
        Status = PaymentTransactionStatus.Failed;
    }
}
