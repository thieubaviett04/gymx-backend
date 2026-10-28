using GymX.Application.Modules.Auth.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result<ResetPasswordResponse>>
{
    public static ResetPasswordCommand FromRequest(ResetPasswordRequest req) => new(
        req.Token.Trim(),
        req.NewPassword);
}

public record ResetPasswordResponse
{
    public string Email { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
