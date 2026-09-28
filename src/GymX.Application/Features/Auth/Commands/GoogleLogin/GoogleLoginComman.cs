using FluentValidation;
using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Common.Models.Authentication;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Identity;
using MediatR;

namespace GymX.Application.Features.Auth.Commands.GoogleLogin;


public record GoogleLoginCommand(string IdToken) : IRequest<Result<AuthTokenDto>>;


public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginCommandValidator()
    {
        RuleFor(x => x.IdToken).NotEmpty().WithMessage("Google ID Token is required.");
    }
}

// Nơi chứa nghiệp vụ đăng nhập Google: Không dính dáng đến cấu hình DB hay JWT
public class GoogleLoginCommandHandler(
    IGoogleAuthService googleAuthService,
    IUserRepository userRepository,
    IJwtService jwtService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GoogleLoginCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Gọi server Google để xác thực tính hợp lệ của Token
        var googleUser = await googleAuthService.ValidateIdTokenAsync(request.IdToken, cancellationToken);
        if (googleUser is null)
            return Result.Failure<AuthTokenDto>(Error.Unauthorized("Auth.Google", "Invalid Google ID Token."));

        // 2. Tìm User trong DB xem đã từng đăng nhập bằng Google chưa
        var user = await userRepository.GetByGoogleIdAsync(googleUser.GoogleId, cancellationToken);
        bool isNewUser = false;

        if (user is null)
        {
            // 3. Nếu chưa, kiểm tra xem email này có bị trùng với tài khoản Mật khẩu thường không
            user = await userRepository.GetByEmailAsync(googleUser.Email, cancellationToken);
            if (user is not null)
                return Result.Failure<AuthTokenDto>(Error.Conflict("Auth.EmailExists", "Email này đã được đăng ký bằng Mật khẩu."));

            // 4. User hoàn toàn mới → Tạo mới và đánh dấu cờ isNewUser để tối ưu hiệu năng
            user = User.CreateWithGoogle(googleUser.Email, googleUser.FullName, googleUser.GoogleId, googleUser.AvatarUrl);
            await userRepository.AddAsync(user, cancellationToken);
            isNewUser = true;
        }

        // 5. Lấy Role. Tối ưu: Nếu là user mới thì gán cứng MEMBER, bỏ qua lệnh query DB dư thừa!
        var roles = new List<string>();
        if (isNewUser)
        {
            roles.Add(Domain.Constants.RoleCode.Member);
        }
        else
        {
            roles = await userRepository.GetUserRolesAsync(user.Id, cancellationToken);
            if (roles.Count == 0) roles.Add(Domain.Constants.RoleCode.Member);
        }

        // 6. Nhờ Infrastructure tạo bộ Token với cấu hình thời hạn lấy từ appsettings
        var jwtResult = jwtService.GenerateTokens(user, roles);

        // 7. Tạo Entity RefreshToken với thời gian hết hạn chuẩn xác từ Config
        var refreshTokenEntity = RefreshToken.Create(user.Id, jwtResult.RefreshToken, jwtResult.RefreshTokenExpiresAt);
        user.RefreshTokens.Add(refreshTokenEntity);

        // 8. Lưu tất cả thay đổi xuống DB trong 1 Transaction an toàn
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 9. Trả kết quả sạch sẽ cho Frontend, không có bất kỳ số Hardcode nào
        return Result.Success(new AuthTokenDto
        {
            AccessToken = jwtResult.AccessToken,
            RefreshToken = jwtResult.RefreshToken,
            ExpiresAt = jwtResult.AccessTokenExpiresIn
        });
    }
}
