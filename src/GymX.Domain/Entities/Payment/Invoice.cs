using GymX.Domain.Common.Models;

namespace GymX.Domain.Entities.Payment;

/// <summary>
/// Hóa đơn thanh toán — lưu trực tiếp vào DB với status PENDING.
/// Sau khi PayOS webhook xác nhận, status chuyển sang PAID.
/// </summary>
public class Invoice : AggregateRoot
{
    // OrderCode kiểu long — đây là mã PayOS dùng để định danh đơn hàng
    // Cũng là field dùng để kiểm tra Idempotency khi webhook gọi nhiều lần
    public long OrderCode { get; private set; }

    // ID của Hội viên / Nhân viên liên quan đến hóa đơn này
    // Null nếu đây là hóa đơn thanh toán tạm (chưa gắn với member cụ thể)
    public Guid? MemberId { get; private set; }

    public decimal Amount { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Pending;

    // Lưu ID từ PayOS để có thể huỷ đơn hàng khi cần
    public string PaymentLinkId { get; private set; } = string.Empty;

    // URL thanh toán trả về từ PayOS (FE dùng để redirect hoặc show QR)
    public string CheckoutUrl { get; private set; } = string.Empty;

    public DateTime? PaidAt { get; private set; }

    // EF Core constructor
    protected Invoice() { }

    public static Invoice Create(
        long orderCode,
        decimal amount,
        string description,
        string paymentLinkId,
        string checkoutUrl,
        Guid? memberId = null)
    {
        return new Invoice
        {
            OrderCode = orderCode,
            Amount = amount,
            Description = description,
            PaymentLinkId = paymentLinkId,
            CheckoutUrl = checkoutUrl,
            MemberId = memberId,
            Status = InvoiceStatus.Pending
        };
    }

    /// <summary>
    /// Gọi khi webhook PayOS xác nhận thanh toán thành công.
    /// </summary>
    public void MarkAsPaid()
    {
        Status = InvoiceStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gọi khi huỷ đơn hàng (hết giờ, khách từ chối...).
    /// </summary>
    public void Cancel()
    {
        Status = InvoiceStatus.Cancelled;
    }
}
