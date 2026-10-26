using FluentValidation;


namespace GymX.Application.Features.Auth.Commands.VerifyOTP
{
    public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Email không hợp lệ.");
            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("Mã OTP không được để trống.")
                .Length(6).WithMessage("Mã OTP phải có 6 chữ số.");
        }
    }
}

