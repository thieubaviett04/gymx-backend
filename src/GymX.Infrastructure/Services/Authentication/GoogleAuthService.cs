using Google.Apis.Auth;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GymX.Infrastructure.Services.Authentication;

// Triển khai thực tế gọi lên Google Server để xác minh chữ ký của Token
public class GoogleAuthService(IOptions<GoogleAuthOptions> options, ILogger<GoogleAuthService> logger) : IGoogleAuthService
{
    private readonly string _clientId = options.Value.ClientId;

    public async Task<GoogleUserInfo?> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        try
        {
            // Cấu hình truyền ClientId vào để Google kiểm tra Token này có đúng sinh ra cho App của mình không
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_clientId]
            };

            // Hàm này tự động gọi Google Server verify chữ ký (Signature) và ngày hết hạn
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new GoogleUserInfo(
                Email: payload.Email,
                FullName: payload.Name,
                GoogleId: payload.Subject,
                AvatarUrl: payload.Picture
            );
        }
        catch (Exception ex)
        {
            // Bắt lỗi nếu Token bị sửa đổi, giả mạo, hoặc đã hết hạn
            logger.LogError(ex, "Failed to validate Google ID Token.");
            return null;
        }
    }
}
