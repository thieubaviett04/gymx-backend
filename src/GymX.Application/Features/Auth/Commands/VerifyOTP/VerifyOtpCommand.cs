using GymX.Domain.Common.Models;
using MediatR;
using System;

namespace GymX.Application.Features.Auth.Commands.VerifyOTP
{
    public record VerifyOtpCommand(string Email, string OtpCode) : IRequest<Result<VerifyOtpResponse>>;

    public record VerifyOtpResponse
    {
        public string Token { get; init; } = string.Empty;
    }

}
