using FluentValidation;
using GymX.Domain.Constants;

namespace GymX.Application.Modules.PenaltyRules.Commands.UpdatePenaltyRule;

public class UpdatePenaltyRuleCommandValidator : AbstractValidator<UpdatePenaltyRuleCommand>
{
    public UpdatePenaltyRuleCommandValidator()
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

        RuleFor(command => command.Status)
            .Must(PenaltyRuleStatus.IsValid).WithMessage("Trạng thái quy định phạt không hợp lệ (ACTIVE, INACTIVE, APPLIED, DISCONTINUED).");

        RuleFor(command => command)
            .Must(command =>
            {
                if (command.Status == PenaltyRuleStatus.Pending)
                {
                    var normalized = command.EffectiveFrom.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(command.EffectiveFrom, DateTimeKind.Utc)
                        : command.EffectiveFrom.ToUniversalTime();
                    return normalized.Date > DateTime.UtcNow.Date;
                }
                return true;
            })
            .WithMessage("Nếu chọn trạng thái Chờ áp dụng, ngày hiệu lực bắt buộc phải là ngày trong tương lai (sau ngày hôm nay).");
    }
}
