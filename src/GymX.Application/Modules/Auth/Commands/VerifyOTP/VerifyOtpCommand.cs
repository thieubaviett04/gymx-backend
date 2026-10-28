using GymX.Application.Modules.Auth.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.VerifyOTP;

public record VerifyOtpCommand(string Email, string OtpCode) : IRequest<Result<VerifyOtpResponse>>
{
    public static VerifyOtpCommand FromRequest(VerifyOtpRequest req) => new(
        req.Email.Trim().ToLowerInvariant(),
        req.OtpCode.Trim());
}
