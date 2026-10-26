using MediatR;
using GymX.Domain.Common.Models;

namespace GymX.Application.Features.Auth.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result<ForgotPasswordResponse>>;

    public record ForgotPasswordResponse
    {
        public string Email { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }
     
}
