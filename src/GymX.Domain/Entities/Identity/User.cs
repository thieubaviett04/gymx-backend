using GymX.Domain.Common.Models;
using GymX.Domain.Contracts;
using GymX.Domain.Constants;


namespace GymX.Domain.Entities.Identity
{
    // Entity trung tâm — hỗ trợ đăng nhập Email/Password + Google, có soft delete
    public class User : EntityAuditBase, ISoftDelete
    {
        public string Email { get; private set; } = string.Empty;

        // PasswordHash là null khi user chỉ dùng Google OAuth
        public string? PasswordHash { get; private set; }
        public string? GoogleId { get; private set; }
        public string FullName { get; private set; } = string.Empty;
        public DateOnly? DateOfBirth { get; private set; }
        public string? Gender { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? Address { get; private set; }
        public string? AvatarUrl { get; private set; }
        public string Status { get; private set; } = AccountStatus.PendingVerification;
        public DateTime? EmailVerifiedAt { get; private set; }

        // ISoftDelete — SoftDeleteInterceptor tự set IsDeleted=true thay vì xóa khỏi DB
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }

        // Navigations: 1 User có nhiều role, nhiều token
        public ICollection<UserRole> UserRoles { get; private set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];
        public ICollection<PasswordResetToken> PasswordResetTokens { get; private set; } = [];
        protected User() { }

        // Tạo user đăng ký bằng Email — status mặc định là PENDING_VERIFICATION
        public static User CreateWithPassword(string email, string fullName, string passwordHash) =>
            new() { Email = email, FullName = fullName, PasswordHash = passwordHash };

        // Tạo user qua Google — email Google đã xác thực nên set ACTIVE ngay
        public static User CreateWithGoogle(string email, string fullName, string googleId, string? avatarUrl) =>
            new()
            {
                Email = email,
                FullName = fullName,
                GoogleId = googleId,
                AvatarUrl = avatarUrl,
                Status = AccountStatus.Active,
                EmailVerifiedAt = DateTime.UtcNow
            };

        // Gọi khi user click link xác thực email — chuyển trạng thái sang ACTIVE
        public void VerifyEmail()
        {
            EmailVerifiedAt = DateTime.UtcNow;
            Status = AccountStatus.Active;
        }

        // Cập nhật mật khẩu mới sau khi reset — chỉ thay hash, không lưu plaintext
        public void UpdatePassword(string newPasswordHash) =>
            PasswordHash = newPasswordHash;
    }
}
