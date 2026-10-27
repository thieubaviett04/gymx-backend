using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Common.Models.Authentication;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Identity;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.RefreshTokens;

public class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IJwtService jwtService,
    IUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken is null)
        {
            return Result.Failure<AuthTokenDto>(
                Error.Unauthorized("Auth.InvalidRefreshToken", "Refresh token không hợp lệ hoặc không tồn tại."));
        }

        var tokenCheck = storedToken.CanRefresh();
        if (tokenCheck.IsFailure)
        {
            if (storedToken.IsRevoked)
            {
                await refreshTokenRepository.RevokeAllUserRefreshTokensAsync(storedToken.UserId, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result.Failure<AuthTokenDto>(tokenCheck.Error);
        }

        var user = storedToken.User;
        var userCheck = user.CanLogin();
        if (userCheck.IsFailure)
        {
            return Result.Failure<AuthTokenDto>(userCheck.Error);
        }

        var roles = await userRepository.GetUserRolesAsync(user.Id, cancellationToken);

        var jwtResult = jwtService.GenerateTokens(user, roles);

        var newRefreshTokenEntity = RefreshToken.Create(
            user.Id,
            jwtResult.RefreshToken,
            jwtResult.RefreshTokenExpiresAt,
            jwtResult.SessionId);

        storedToken.Revoke(newRefreshTokenEntity.Id);

        await refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);
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
