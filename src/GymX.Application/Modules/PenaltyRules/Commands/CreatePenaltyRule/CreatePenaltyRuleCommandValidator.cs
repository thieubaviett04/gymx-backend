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

        When(command => string.Equals(command.ViolationType?.Trim(), RuleTargetAudience.Staff, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(command.ViolationType?.Trim(), RuleTargetAudience.Trainer, StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(command => command.FineAmount)
                .GreaterThan(0).WithMessage("Quy định kỷ luật nhân viên yêu cầu nhập số tiền phạt lớn hơn 0 ₫.");
        });

        When(command => string.Equals(command.ViolationType?.Trim(), RuleTargetAudience.Member, StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(command => command.FineAmount)
                .Equal(0).WithMessage("Nội quy hội viên áp dụng hình thức tích lũy nhắc nhở, tiền phạt mặc định là 0 ₫.");
        });

        RuleFor(command => command.SeverityLevel)
            .NotEmpty().WithMessage("Mức độ nghiêm trọng không được để trống.")
            .Must(severity => severity == PenaltyRuleSeverity.Low || severity == PenaltyRuleSeverity.Medium || severity == PenaltyRuleSeverity.High)
            .WithMessage("Mức độ nghiêm trọng phải là LOW, MEDIUM hoặc HIGH.");

        RuleFor(command => command.EffectiveFrom)
            .Must(effectiveFrom =>
            {
                var ruleDate = DateOnly.FromDateTime(effectiveFrom);
                var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)).AddDays(-1);
                return ruleDate >= yesterday;
            })
            .WithMessage("Thời điểm áp dụng không được chọn thời điểm trong quá khứ.");

        RuleFor(command => command.EffectiveFrom)
            .Must((command, effectiveFrom) =>
            {
                if (command.Status == PenaltyRuleStatus.Pending)
                {
                    var ruleDate = DateOnly.FromDateTime(effectiveFrom);
                    var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
                    return ruleDate > today;
                }
                return true;
            })
            .WithName("effectiveDate")
            .WithMessage("Trạng thái chưa áp dụng không thể chọn ngày hiện tại, quá khứ.");

        RuleFor(command => command.EffectiveFrom)
            .Must((command, effectiveFrom) =>
            {
                if (command.Status == PenaltyRuleStatus.Active || command.Status == PenaltyRuleStatus.Applied)
                {
                    var ruleDate = DateOnly.FromDateTime(effectiveFrom);
                    var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
                    return ruleDate <= today;
                }
                return true;
            })
            .WithName("effectiveDate")
            .WithMessage("Trạng thái đang áp dụng không thể chọn ngày tương lai.");
    }
}
