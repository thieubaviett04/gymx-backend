using GymX.Application.Modules.Auth.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result<ForgotPasswordResponse>>
{
    public static ForgotPasswordCommand FromRequest(ForgotPasswordRequest req) => new(
        req.Email.Trim().ToLowerInvariant());
}

public record ForgotPasswordResponse
{
    public string Email { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
