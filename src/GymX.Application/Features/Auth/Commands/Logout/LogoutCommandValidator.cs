using FluentValidation;

namespace GymX.Application.Features.Auth.Commands.Logout
{
    public class  LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Cần có refresh token");
        }
    }
}
