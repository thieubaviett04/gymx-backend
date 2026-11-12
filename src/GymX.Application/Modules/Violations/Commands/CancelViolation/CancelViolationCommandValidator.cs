using FluentValidation;

namespace GymX.Application.Modules.Violations.Commands.CancelViolation;

public class CancelViolationCommandValidator : AbstractValidator<CancelViolationCommand>
{
    public CancelViolationCommandValidator()
    {
        RuleFor(x => x.ViolationId)
            .NotEmpty().WithMessage("Mã định danh biên bản vi phạm không được để trống.");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Lý do hủy không được vượt quá 500 ký tự.");
    }
}
