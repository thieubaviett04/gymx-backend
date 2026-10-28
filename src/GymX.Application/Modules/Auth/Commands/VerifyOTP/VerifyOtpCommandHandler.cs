using GymX.Application.Modules.Auth.DTOs;
using GymX.Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace GymX.Application.Modules.Auth.Commands.VerifyOTP;

public class VerifyOtpCommandHandler(IMemoryCache memoryCache) : IRequestHandler<VerifyOtpCommand, Result<VerifyOtpResponse>>
{
    public async Task<Result<VerifyOtpResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        var otpCacheKey = $"OTP_ResetPassword_{request.Email}";
        if (!memoryCache.TryGetValue(otpCacheKey, out string? cachedOtp) || cachedOtp != request.OtpCode)
        {
            return Result.Failure<VerifyOtpResponse>(
                Error.Validation("Auth.InvalidOtp", "Mã OTP không chính xác"));
        }

        memoryCache.Remove(otpCacheKey);

        var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var resetTokenCache = $"ResetToken_{resetToken}";

        memoryCache.Set(resetTokenCache, request.Email, TimeSpan.FromMinutes(5));

        return Result.Success(new VerifyOtpResponse
        {
            Token = resetToken
        });
    }
}
