using FluentValidation;
using GymX.Domain.Constants.Finance;

namespace GymX.Application.Modules.Invoices.Commands.CreateInvoice;

public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Tên người nộp / khách hàng không được để trống.")
            .MaximumLength(150).WithMessage("Tên khách hàng không được vượt quá 150 ký tự.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Số tiền thu phải là số dương lớn hơn 0.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Phương thức thanh toán không được để trống.");

        RuleFor(x => x.ReferenceType)
            .NotEmpty().WithMessage("Nguồn thu không được để trống.")
            .Must(InvoiceReferenceType.IsValid).WithMessage("Nguồn thu không hợp lệ.");

        RuleFor(x => x.IssuedDate)
            .NotEmpty().WithMessage("Ngày thu tiền không được để trống.");
    }
}

