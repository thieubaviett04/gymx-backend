using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace GymX.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IEmailService emailService,
    IMemoryCache memoryCache)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra Email và Số điện thoại đã tồn tại chưa
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


        // 2. Sinh mã OTP Bảo mật tuyệt đối (Cryptography)
        var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        var cacheKey = $"OTP_VerifyEmail_{request.Email}";


        // 3. Đóng gói dữ liệu Đăng ký và đưa thẳng vào Cache (Limbo State) - Sống 5 phút
        var passwordHash = passwordHasher.Hash(request.Password);
        
        var pendingRegistration = new PendingRegistrationCacheItem(
            FullName: request.FullName,
            Email: request.Email,
            PhoneNumber: request.PhoneNumber,
            PasswordHash: passwordHash,
            OtpCode: otpCode
        );

        memoryCache.Set(cacheKey, pendingRegistration, TimeSpan.FromMinutes(5));


        // 4. Gửi Email xác thực
        var emailContent = $@"
            <h3>Xin chào {request.FullName}!</h3>
            <p>Cảm ơn bạn đã đăng ký tài khoản GymX.</p>
            <p>Mã OTP xác thực của bạn là: <strong style='font-size: 1.5em; color: #2E86C1;'>{otpCode}</strong></p>
            <p><i>Mã này có hiệu lực trong vòng 5 phút. Vui lòng không chia sẻ cho bất kỳ ai.</i></p>";

        var emailSent = await emailService.SendEmailAsync(
            request.Email,
            "GymX - Mã OTP Xác thực tài khoản",
            emailContent,
            cancellationToken);

        if (!emailSent)
        {
            // Xóa rác trong Cache ngay nếu gửi email xịt
            memoryCache.Remove(cacheKey);
            return Result.Failure<RegisterResponse>(
                Error.Failure("Auth.EmailFailed", "Hệ thống đang gặp sự cố khi gửi email. Vui lòng thử lại sau."));
        }


        // 5. Trả về kết quả thành công cho Frontend
        return Result.Success(new RegisterResponse
        {
            Email = request.Email,
            FullName = request.FullName,
            Message = "Đăng ký thành công! Vui lòng kiểm tra email để lấy mã OTP xác thực."
        });
    }
}
