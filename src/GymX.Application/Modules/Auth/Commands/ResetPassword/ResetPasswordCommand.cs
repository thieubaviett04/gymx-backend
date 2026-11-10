using GymX.Application.Modules.Auth.DTOs.Request;
using GymX.Application.Modules.Auth.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result<ResetPasswordResponse>>
{
    public static ResetPasswordCommand FromRequest(ResetPasswordRequest req) => new(
        req.Token.Trim(),
        req.NewPassword);
}
