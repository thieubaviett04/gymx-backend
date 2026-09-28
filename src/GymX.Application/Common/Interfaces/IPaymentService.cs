using GymX.Domain.Common.Models;

namespace GymX.Application.Common.Interfaces;

/// <summary>
/// Interface trung gian cho dịch vụ thanh toán.
/// Hiện tại implement bằng PayOS — có thể thay bằng VNPAY/Momo mà không đụng Controller.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Tạo link thanh toán VietQR trên PayOS.
    /// Trả về CheckoutUrl và mã QR để hiển thị cho Hội viên.
    /// </summary>
    Task<Result<CreatePaymentResponse>> CreatePaymentLinkAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra trạng thái đơn hàng theo OrderCode.
    /// </summary>
    Task<Result<string>> GetPaymentStatusAsync(
        long orderCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Huỷ đơn hàng khi hết giờ hoặc khách từ chối thanh toán.
    /// </summary>
    Task<Result<bool>> CancelPaymentAsync(
        long orderCode,
        CancellationToken cancellationToken = default);
}

public record CreatePaymentRequest(
    long OrderCode,
    decimal Amount,
    string Description,
    string? BuyerName = null,
    string? BuyerEmail = null,
    string? BuyerPhone = null);

public record CreatePaymentResponse(
    string CheckoutUrl,
    string QrCode,
    string PaymentLinkId,
    long OrderCode);
