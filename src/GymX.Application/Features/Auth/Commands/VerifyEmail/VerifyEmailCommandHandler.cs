using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Features.Auth.Commands.Register;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Entities.Identity;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace GymX.Application.Features.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IGenericRepository<UserRole> userRoleRepository,
    IMemoryCache memoryCache,
    IUnitOfWork unitOfWork)
    : IRequestHandler<VerifyEmailCommand, Result<string>>
{
    public async Task<Result<string>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra OTP và dữ liệu Limbo trong Cache
        var cacheKey = $"OTP_VerifyEmail_{request.Email}";
        if (!memoryCache.TryGetValue(cacheKey, out PendingRegistrationCacheItem? pendingData) || pendingData is null)
        {
            return Result.Failure<string>(Error.Validation("Auth.InvalidOtp", "Mã OTP đã hết hạn hoặc không tồn tại. Vui lòng đăng ký lại."));
        }

        if (pendingData.OtpCode != request.OtpCode)
        {
            return Result.Failure<string>(Error.Validation("Auth.InvalidOtp", "Mã OTP không chính xác."));
        }


        // 2. Double-check: Đảm bảo trong 5 phút vừa qua không có ai nẫng tay trên Email/SĐT này
        var existingUser = await userRepository.GetByEmailAsync(pendingData.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Result.Failure<string>(Error.Conflict("Auth.EmailExists", "Email này vừa được đăng ký bởi người khác."));
        }

        var existingPhone = await userRepository.GetByPhoneNumberAsync(pendingData.PhoneNumber, cancellationToken);
        if (existingPhone is not null)
        {
            return Result.Failure<string>(Error.Conflict("Auth.PhoneExists", "Số điện thoại này vừa được sử dụng bởi người khác."));
        }


        // 3. Khởi tạo tài khoản chính thức (Trạng thái mặc định: PENDING_VERIFICATION)
        var user = User.CreateWithPassword(pendingData.Email, pendingData.FullName, pendingData.PasswordHash);
        user.UpdateProfile(pendingData.PhoneNumber, null, null);


        // 4. Đánh dấu đã xác thực Email ngay lập tức
        user.VerifyEmail();
        
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);


        // 5. Cấp quyền MEMBER
        var memberRole = await roleRepository.GetByCodeAsync(RoleCode.Member, cancellationToken);
        if (memberRole is null)
        {
            return Result.Failure<string>(Error.NotFound("Auth.RoleNotFound", "Hệ thống lỗi: Không tìm thấy Role MEMBER."));
        }

        var userRole = UserRole.Create(user.Id, memberRole.Id);
        await userRoleRepository.AddAsync(userRole, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        
        // 6. Xóa OTP khỏi Cache để tránh bị dùng lại (Replay Attack)
        memoryCache.Remove(cacheKey);


        // 7. Hoàn thành
        return Result.Success("Xác thực email thành công! Chào mừng bạn đến với GymX.");
    }
}
