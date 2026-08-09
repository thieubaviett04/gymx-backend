
namespace GymX.Application.Common.Interfaces.Authencation
{
    // Kết quả trả về từ Google sau khi validate ID Token thành công
    public record GoogleUserInfo(string Email, string FullName, string GoogleId, string? AvatarUrl);
    // Abstraction xác thực Google ID Token (giúp dễ dàng mock khi viết Unit Test)
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo?> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken = default);
    }
}
