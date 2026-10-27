using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Common.Models.Authentication;
using GymX.Application.Modules.Auth.Commands.Register;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Entities.Identity;
using GymX.Domain.Entities.Members;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace GymX.Application.Modules.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IGenericRepository<Member> memberRepository,
    IJwtService jwtService,
    IMemoryCache memoryCache,
    IUnitOfWork unitOfWork)
    : IRequestHandler<VerifyEmailCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var cacheKey = $"OTP_VerifyEmail_{request.Email}";
        if (!memoryCache.TryGetValue(cacheKey, out PendingRegistrationCacheItem? pendingData) || pendingData is null)
        {
            return Result.Failure<AuthTokenDto>(Error.Validation("Auth.InvalidOtp", "Mã OTP đã hết hạn hoặc không tồn tại. Vui lòng đăng ký lại."));
        }

        if (pendingData.OtpCode != request.OtpCode)
        {
            return Result.Failure<AuthTokenDto>(Error.Validation("Auth.InvalidOtp", "Mã OTP không chính xác."));
        }

        var existingUser = await userRepository.GetByEmailAsync(pendingData.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Result.Failure<AuthTokenDto>(Error.Conflict("Auth.EmailExists", "Email này đã được sử dụng."));
        }

        var existingPhone = await userRepository.GetByPhoneNumberAsync(pendingData.PhoneNumber, cancellationToken);
        if (existingPhone is not null)
        {
            return Result.Failure<AuthTokenDto>(Error.Conflict("Auth.PhoneExists", "Số điện thoại này đã được sử dụng."));
        }

        var memberRole = await roleRepository.GetByCodeAsync(RoleCode.Member, cancellationToken);
        if (memberRole is null)
        {
            return Result.Failure<AuthTokenDto>(Error.NotFound("Auth.RoleNotFound", "Hệ thống lỗi: Không tìm thấy Role MEMBER."));
        }

        var user = User.CreateWithPassword(pendingData.Email, pendingData.FullName, pendingData.PasswordHash);
        user.UpdateProfile(pendingData.PhoneNumber, null, null);
        user.VerifyEmail();
        user.AssignRole(memberRole.Id);

        var memberCode = $"MEM{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
        var member = Member.Create(user.Id, memberCode, DateOnly.FromDateTime(DateTime.UtcNow));

        var roles = new List<string> { RoleCode.Member };
        var jwtResult = jwtService.GenerateTokens(user, roles);

        var refreshTokenEntity = RefreshToken.Create(
            user.Id,
            jwtResult.RefreshToken,
            jwtResult.RefreshTokenExpiresAt,
            jwtResult.SessionId);

        await userRepository.AddAsync(user, cancellationToken);
        await memberRepository.AddAsync(member, cancellationToken);
        await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        memoryCache.Remove(cacheKey);

        return Result.Success(new AuthTokenDto
        {
            AccessToken = jwtResult.AccessToken,
            RefreshToken = jwtResult.RefreshToken,
            ExpiresAt = jwtResult.AccessTokenExpiresIn,
            Roles = roles,
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl
        });
    }
}
