
using GymX.Application.Common.Interfaces;
using GymX.Domain.Entities.Identity;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Common.Models.Authentication;
using GymX.Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace GymX.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IUnitOfWork unitOfWork,
        IMemoryCache memoryCache,
        IGenericRepository<RefreshToken> refreshTokenRepository
        ) : IRequestHandler<LoginCommand, Result<AuthTokenDto>>
    {
        public async Task<Result<AuthTokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
            
            if (user is null)
            {
                var cacheKey = $"OTP_VerifyEmail_{request.Email}";
                return memoryCache.TryGetValue(cacheKey, out _) 
                    ? Result.Failure<AuthTokenDto>(Error.Forbidden("Auth.UnverifiedEmail", "Tài khoản chưa xác thực email. Vui lòng xác thực mã OTP gửi về email"))
                    : Result.Failure<AuthTokenDto>(Error.Unauthorized("Auth.InvalidCredentials", "Email hoặc mật khẩu không chính xác"));
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return Result.Failure<AuthTokenDto>(
                    Error.Unauthorized("Auth.InvalidCredentials", "Tài khoản này được đăng ký bằng Google. Vui lòng đăng nhập bằng Google"));
            }

            var isPasswordValid = passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Result.Failure<AuthTokenDto>(
                    Error.Unauthorized("Auth.InvalidCredentials", "Email hoặc mật khẩu không chính xác"));
            }

            var resultLogin = user.CanLogin();
            if (!resultLogin.IsSuccess)
            {
                return Result.Failure<AuthTokenDto>(resultLogin.Error);
            }

            var roles = await userRepository.GetUserRolesAsync(user.Id, cancellationToken);
            var jwtResult = jwtService.GenerateTokens(user, roles);
            var refreshTokenEntity = RefreshToken.Create(
                user.Id,
                jwtResult.RefreshToken,
                jwtResult.RefreshTokenExpiresAt,
                jwtResult.SessionId);

            await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

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
}
