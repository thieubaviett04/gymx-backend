using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Finance;
using MediatR;

namespace GymX.Application.Features.Payments.Commands.CreateInvoice;

public record CreateInvoiceCommand(
    Guid? MemberId,
    decimal Amount,
    string Description,
    string? BuyerName = null,
    string? BuyerEmail = null,
    string? BuyerPhone = null) : IRequest<Result<CreatePaymentResponse>>;

public class CreateInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateInvoiceCommand, Result<CreatePaymentResponse>>
{
    public async Task<Result<CreatePaymentResponse>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        // 1. Tạo OrderCode duy nhất (dùng timestamp)
        var orderCode = long.Parse(DateTimeOffset.UtcNow.ToString("yyMMddHHmmssfff"));

        // 2. Gọi PayOS tạo QR code
        var paymentRequest = new CreatePaymentRequest(
            OrderCode: orderCode,
            Amount: request.Amount,
            Description: request.Description,
            BuyerName: request.BuyerName,
            BuyerEmail: request.BuyerEmail,
            BuyerPhone: request.BuyerPhone
        );

        var paymentResult = await paymentService.CreatePaymentLinkAsync(paymentRequest, cancellationToken);
        if (!paymentResult.IsSuccess)
        {
            return Result<CreatePaymentResponse>.Failure(paymentResult.Error);
        }

        // 3. Lưu Invoice vào DB với trạng thái Pending
        var invoice = Invoice.Create(
            invoiceNumber: orderCode.ToString(),
            referenceType: "GENERAL",
            referenceId: Guid.NewGuid(), // Placeholder
            subtotal: request.Amount,
            totalAmount: request.Amount,
            memberId: request.MemberId,
            customerName: request.BuyerName,
            customerPhone: request.BuyerPhone
        );

        await invoiceRepository.AddAsync(invoice, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreatePaymentResponse>.Success(paymentResult.Value);
    }
}
