using GymX.Application.Common.Interfaces;
using GymX.Domain.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayOS;
using PayOS.Models.V2.PaymentRequests;

namespace GymX.Infrastructure.Services;

public class PaymentService(
    PayOSClient payOSClient,
    IOptions<GymX.Infrastructure.Options.PayOSOptions> options,
    ILogger<PaymentService> logger) : IPaymentService
{
    private readonly GymX.Infrastructure.Options.PayOSOptions _options = options.Value;

    public async Task<Result<CreatePaymentResponse>> CreatePaymentLinkAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var paymentRequest = new CreatePaymentLinkRequest
            {
                OrderCode = request.OrderCode,
                Amount = (int)request.Amount,
                Description = request.Description,
                CancelUrl = _options.CancelUrl,
                ReturnUrl = _options.ReturnUrl,
                BuyerName = request.BuyerName,
                BuyerEmail = request.BuyerEmail,
                BuyerPhone = request.BuyerPhone
            };

            var paymentLink = await payOSClient.PaymentRequests.CreateAsync(paymentRequest);

            logger.LogInformation(
                "Tạo link thanh toán thành công. OrderCode: {OrderCode}, PaymentLinkId: {PaymentLinkId}",
                request.OrderCode,
                paymentLink.PaymentLinkId);

            return Result<CreatePaymentResponse>.Success(new CreatePaymentResponse(
                CheckoutUrl: paymentLink.CheckoutUrl,
                QrCode: paymentLink.QrCode,
                PaymentLinkId: paymentLink.PaymentLinkId,
                OrderCode: paymentLink.OrderCode
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi tạo link thanh toán PayOS. OrderCode: {OrderCode}", request.OrderCode);
            return Result<CreatePaymentResponse>.Failure(Error.Failure("Payment.CreateFailed", $"Lỗi: {ex.Message}"));
        }
    }

    public async Task<Result<string>> GetPaymentStatusAsync(
        long orderCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var paymentLinkInfo = await payOSClient.PaymentRequests.GetAsync(orderCode);
            return Result<string>.Success(paymentLinkInfo.Status.ToString());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi kiểm tra trạng thái thanh toán. OrderCode: {OrderCode}", orderCode);
            return Result<string>.Failure(Error.Failure("Payment.GetStatusFailed", $"Lỗi: {ex.Message}"));
        }
    }

    public async Task<Result<bool>> CancelPaymentAsync(
        long orderCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await payOSClient.PaymentRequests.CancelAsync(orderCode, "Cancelled by user");
            logger.LogInformation("Đã huỷ đơn hàng PayOS. OrderCode: {OrderCode}", orderCode);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi huỷ đơn hàng PayOS. OrderCode: {OrderCode}", orderCode);
            return Result<bool>.Failure(Error.Failure("Payment.CancelFailed", $"Lỗi: {ex.Message}"));
        }
    }
}
