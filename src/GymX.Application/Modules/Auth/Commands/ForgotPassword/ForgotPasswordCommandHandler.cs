using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Common.Templates;
using GymX.Application.Modules.Auth.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace GymX.Application.Modules.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IEmailService emailService,
    IMemoryCache memoryCache) : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponse>>
{
    public async Task<Result<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Result<ForgotPasswordResponse>.Success(new ForgotPasswordResponse
            {
                Email = request.Email,
                Message = "Nếu email tồn tại trong hệ thống, mã xác thực đã được gửi."
            });
        }

        var otpCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var cacheKey = $"OTP_ResetPassword_{request.Email}";
        memoryCache.Set(cacheKey, otpCode, TimeSpan.FromMinutes(5));

        var recipientName = string.IsNullOrWhiteSpace(user.FullName) ? "Hội viên" : user.FullName;
        var emailContent = EmailTemplates.BuildOtpEmail(
            recipientName: recipientName,
            otpCode: otpCode,
            purposeTitle: "Yêu cầu đặt lại mật khẩu",
            purposeDescription: "Hệ thống đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Vui lòng sử dụng mã OTP dưới đây để tiến hành thiết lập mật khẩu mới:");

        await emailService.SendEmailAsync(
            request.Email,
            "GymFitness - Mã OTP Đặt lại mật khẩu",
            emailContent,
            cancellationToken);

        return Result<ForgotPasswordResponse>.Success(new ForgotPasswordResponse
        {
            Email = request.Email,
            Message = "Mã xác thực OTP đã được gửi đến email của bạn."
        });
    }
}
