using GymX.Application.Common.Exceptions;
using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Invoices.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Finance;
using MediatR;

namespace GymX.Application.Modules.Invoices.Commands.CreateInvoice;

public sealed class CreateInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateInvoiceCommand, Result<InvoiceSummaryDto>>
{
    private const int MaxRetryAttempts = 3;

    public async Task<Result<InvoiceSummaryDto>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.TryParse(currentUserService.UserId, out var uid) ? uid : (Guid?)null;

        Guid? memberId = null;
        if (!string.IsNullOrWhiteSpace(request.MemberCode))
        {
            memberId = await invoiceRepository.GetMemberIdByCodeAsync(request.MemberCode.Trim(), cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(request.CustomerPhone))
        {
            memberId = await invoiceRepository.GetMemberIdByPhoneAsync(request.CustomerPhone.Trim(), cancellationToken);
        }

        var nowVn = DateTime.UtcNow.AddHours(7);
        var timePart = new TimeOnly(nowVn.Hour, nowVn.Minute, nowVn.Second);
        var issuedAtUtc = request.IssuedDate.ToDateTime(timePart, DateTimeKind.Utc).AddHours(-7);

        var retryCount = 0;
        Invoice? createdInvoice = null;

        while (retryCount < MaxRetryAttempts)
        {
            try
            {
                var invoiceNumber = await invoiceRepository.GenerateNextInvoiceNumberAsync(request.IssuedDate, cancellationToken);
                var referenceId = Guid.NewGuid();

                createdInvoice = Invoice.CreatePaid(
                    invoiceNumber,
                    request.ReferenceType,
                    referenceId,
                    request.Amount,
                    request.Amount,
                    issuedAtUtc,
                    memberId,
                    request.CustomerName.Trim(),
                    request.CustomerPhone?.Trim(),
                    currentUserId);

                await invoiceRepository.AddAsync(createdInvoice, cancellationToken);

                var payment = Payment.CreateSuccess(
                    createdInvoice.Id,
                    request.PaymentMethod.Trim(),
                    request.Amount,
                    issuedAtUtc,
                    $"PAY-{invoiceNumber}",
                    currentUserId);

                await invoiceRepository.AddPaymentAsync(payment, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                break;
            }
            catch (UniqueConstraintException)
            {
                retryCount++;
                if (retryCount >= MaxRetryAttempts)
                {
                    return Result.Failure<InvoiceSummaryDto>(Error.Conflict("Invoice.DuplicateNumber", "Không thể sinh mã số hóa đơn duy nhất. Vui lòng thử lại."));
                }
            }
        }

        if (createdInvoice == null)
        {
            return Result.Failure<InvoiceSummaryDto>(Error.Failure("Invoice.CreateFailed", "Không thể tạo hóa đơn thu tiền."));
        }

        var summary = await invoiceRepository.GetInvoiceSummaryByIdAsync(createdInvoice.Id, cancellationToken);
        if (summary == null)
        {
            return Result.Failure<InvoiceSummaryDto>(Error.NotFound("Invoice.NotFound", "Không tìm thấy thông tin hóa đơn sau khi tạo."));
        }

        return Result.Success(summary);
    }
}

