using FluentValidation;

namespace GymX.Application.Modules.Auth.Commands.RefreshTokens;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Cần có refresh token");
    }
}
