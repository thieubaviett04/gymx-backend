using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Auth.DTOs.Response;
using GymX.Application.Common.Templates;
using GymX.Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace GymX.Application.Modules.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IEmailService emailService,
    IMemoryCache memoryCache)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Result.Failure<RegisterResponse>(
                Error.Conflict("Auth.EmailExists", "Email này đã được đăng ký. Vui lòng đăng nhập tài khoản."));
        }

        var existingPhone = await userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
        if (existingPhone is not null)
        {
            return Result.Failure<RegisterResponse>(
                Error.Conflict("Auth.PhoneExists", "Số điện thoại này đã được sử dụng. Vui lòng dùng số khác."));
        }

        var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        var cacheKey = $"OTP_VerifyEmail_{request.Email}";
        var passwordHash = passwordHasher.Hash(request.Password);
        
        var pendingRegistration = new PendingRegistrationCacheItem(
            FullName: request.FullName,
            Email: request.Email,
            PhoneNumber: request.PhoneNumber,
            PasswordHash: passwordHash,
            OtpCode: otpCode
        );

        memoryCache.Set(cacheKey, pendingRegistration, TimeSpan.FromMinutes(5));

        var emailContent = EmailTemplates.BuildOtpEmail(
            recipientName: request.FullName,
            otpCode: otpCode,
            purposeTitle: "Xác thực đăng ký tài khoản",
            purposeDescription: "Cảm ơn bạn đã lựa chọn đồng hành cùng GymFitness! Để hoàn tất quá trình đăng ký tài khoản, vui lòng nhập mã OTP xác thực dưới đây vào ứng dụng:");

        var emailSent = await emailService.SendEmailAsync(
            request.Email,
            "GymFitness - Mã OTP Xác thực đăng ký tài khoản",
            emailContent,
            cancellationToken);

        if (!emailSent)
        {
            memoryCache.Remove(cacheKey);
            return Result.Failure<RegisterResponse>(
                Error.Failure("Auth.EmailFailed", "Hệ thống đang gặp sự cố khi gửi email. Vui lòng thử lại sau."));
        }

        return Result.Success(new RegisterResponse
        {
            Email = request.Email,
            FullName = request.FullName,
            Message = "Đăng ký thành công! Vui lòng kiểm tra email để lấy mã OTP xác thực."
        });
    }
}
