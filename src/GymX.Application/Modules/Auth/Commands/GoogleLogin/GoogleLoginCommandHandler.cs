using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Common.Models.Authentication;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Entities.Identity;
using GymX.Domain.Entities.Members;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.GoogleLogin;

public class GoogleLoginCommandHandler(
    IGoogleAuthService googleAuthService,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IGenericRepository<Member> memberRepository,
    IJwtService jwtService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GoogleLoginCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var googleUser = await googleAuthService.ValidateIdTokenAsync(request.IdToken, cancellationToken);
        if (googleUser is null)
        {
            return Result.Failure<AuthTokenDto>(Error.Unauthorized("Auth.Google", "Invalid Google ID Token."));
        }

        var normalizedEmail = googleUser.Email.Trim().ToLowerInvariant();
        var normalizedFullName = googleUser.FullName.Trim();

        var user = await userRepository.GetByGoogleIdAsync(googleUser.GoogleId, cancellationToken);
        var isNewUser = false;

        if (user is null)
        {
            user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (user is not null)
            {
                return Result.Failure<AuthTokenDto>(Error.Conflict("Auth.EmailExists", "Email này đã được đăng ký bằng Mật khẩu."));
            }

            var memberRole = await roleRepository.GetByCodeAsync(RoleCode.Member, cancellationToken);

            user = User.CreateWithGoogle(normalizedEmail, normalizedFullName, googleUser.GoogleId, googleUser.AvatarUrl);
            if (memberRole is not null)
            {
                user.AssignRole(memberRole.Id);
            }

            var memberCode = $"MEM{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
            var member = Member.Create(user.Id, memberCode, DateOnly.FromDateTime(DateTime.UtcNow));
            await memberRepository.AddAsync(member, cancellationToken);

            await userRepository.AddAsync(user, cancellationToken);
            isNewUser = true;
        }

        var roles = isNewUser
            ? [RoleCode.Member]
            : await userRepository.GetUserRolesAsync(user.Id, cancellationToken);

        if (roles.Count == 0)
        {
            var memberRole = await roleRepository.GetByCodeAsync(RoleCode.Member, cancellationToken);
            if (memberRole is not null)
            {
                user.AssignRole(memberRole.Id);
            }
            roles.Add(RoleCode.Member);
        }

        var jwtResult = jwtService.GenerateTokens(user, roles);

        var refreshTokenEntity = RefreshToken.Create(user.Id, jwtResult.RefreshToken, jwtResult.RefreshTokenExpiresAt, jwtResult.SessionId);
        user.RefreshTokens.Add(refreshTokenEntity);

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
