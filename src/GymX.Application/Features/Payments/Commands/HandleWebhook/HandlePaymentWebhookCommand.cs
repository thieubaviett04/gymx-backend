using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payment;
using MediatR;

using Microsoft.Extensions.Logging;

namespace GymX.Application.Features.Payments.Commands.HandleWebhook;

public record HandlePaymentWebhookCommand(
    long OrderCode,
    bool Success) : IRequest<Result<bool>>;

public class HandlePaymentWebhookCommandHandler(
    IInvoiceRepository invoiceRepository,
    IUnitOfWork unitOfWork,
    ILogger<HandlePaymentWebhookCommandHandler> logger) : IRequestHandler<HandlePaymentWebhookCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(HandlePaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetByOrderCodeAsync(request.OrderCode, cancellationToken);
        if (invoice == null)
        {
            logger.LogWarning("Webhook PayOS nhận được OrderCode {OrderCode} không tồn tại trong hệ thống.", request.OrderCode);
            return Result<bool>.Failure(Error.NotFound("Invoice.NotFound", "Không tìm thấy hoá đơn."));
        }

        // Idempotency check: nếu đã xử lý rồi thì bỏ qua và trả về true luôn để PayOS không gọi lại
        if (invoice.Status == InvoiceStatus.Paid)
        {
            logger.LogInformation("Webhook PayOS gọi lại OrderCode {OrderCode} đã thanh toán. Bỏ qua.", request.OrderCode);
            return Result<bool>.Success(true);
        }

        if (request.Success)
        {
            invoice.MarkAsPaid();
            logger.LogInformation("Đã cập nhật hoá đơn {OrderCode} thành PAID.", request.OrderCode);
            
            // TODO: Ở phase sau (Module Hội viên), chỗ này sẽ phát event/SignalR để xử lý kích hoạt gói tập
        }
        else
        {
            invoice.Cancel();
            logger.LogInformation("Đã huỷ hoá đơn {OrderCode} theo webhook.", request.OrderCode);
        }

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
