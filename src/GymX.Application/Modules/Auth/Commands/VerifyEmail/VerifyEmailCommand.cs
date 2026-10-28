using GymX.Application.Common.Models.Authentication;
using GymX.Application.Modules.Auth.DTOs.Request;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(string Email, string OtpCode) : IRequest<Result<AuthTokenDto>>
{
    public static VerifyEmailCommand FromRequest(VerifyEmailRequest req) => new(
        req.Email.Trim().ToLowerInvariant(),
        req.OtpCode.Trim());
}
