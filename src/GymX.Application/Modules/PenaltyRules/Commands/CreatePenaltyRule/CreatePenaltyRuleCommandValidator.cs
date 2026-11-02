using FluentValidation;
using GymX.Domain.Constants;

namespace GymX.Application.Modules.PenaltyRules.Commands.CreatePenaltyRule;

public class CreatePenaltyRuleCommandValidator : AbstractValidator<CreatePenaltyRuleCommand>
{
    public CreatePenaltyRuleCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Tên quy định phạt không được để trống.")
            .MaximumLength(150).WithMessage("Tên quy định phạt tối đa 150 ký tự.");

        RuleFor(command => command.FineAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Số tiền phạt phải là số thực không âm.");

        RuleFor(command => command.SeverityLevel)
            .NotEmpty().WithMessage("Mức độ nghiêm trọng không được để trống.")
            .Must(severity => severity == PenaltyRuleSeverity.Low || severity == PenaltyRuleSeverity.Medium || severity == PenaltyRuleSeverity.High)
            .WithMessage("Mức độ nghiêm trọng phải là LOW, MEDIUM hoặc HIGH.");

        RuleFor(command => command.EffectiveFrom)
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Thời điểm áp dụng không được chọn thời điểm trong quá khứ.");
    }
}
