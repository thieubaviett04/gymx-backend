using FluentValidation;

namespace GymX.Application.Modules.Violations.Commands.CreateViolation;

public class CreateViolationCommandValidator : AbstractValidator<CreateViolationCommand>
{
    public CreateViolationCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Nhân viên không được để trống.");

        RuleFor(x => x.PenaltyRuleId)
            .NotEmpty().WithMessage("Quy định phạt không được để trống.");

        RuleFor(x => x.OccurredAt)
            .NotEmpty().WithMessage("Thời gian vi phạm không được để trống.")
            .Must(date => date <= DateTime.UtcNow.AddMinutes(5))
            .WithMessage("Thời gian vi phạm không được lớn hơn thời điểm hiện tại.");

        RuleFor(x => x.EvidenceUrl)
            .MaximumLength(1000).WithMessage("Đường dẫn bằng chứng không được vượt quá 1000 ký tự.")
            .When(x => !string.IsNullOrEmpty(x.EvidenceUrl));

        RuleFor(x => x.Note)
            .MaximumLength(1000).WithMessage("Ghi chú không được vượt quá 1000 ký tự.")
            .When(x => !string.IsNullOrEmpty(x.Note));
    }
}

