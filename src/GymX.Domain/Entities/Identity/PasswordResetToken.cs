using GymX.Domain.Common.Models;

namespace GymX.Domain.Entities.Identity
{
    // Token đặt lại mật khẩu gửi qua email — dùng 1 lần, có thời hạn
    public class PasswordResetToken : EntityBase
    {
        public Guid UserId { get; private set; }
        // Lưu hash thay vì raw token — bảo mật tương tự như password
        public string TokenHash { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public DateTime? UsedAt { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public User User { get; private set; } = null!;

        // Computed properties — phòng chống replay attack (dùng lại token cũ)
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsUsed => UsedAt.HasValue;
        public bool IsValid => !IsExpired && !IsUsed;
        protected PasswordResetToken() { }

        // ExpiresAt thường là DateTime.UtcNow.AddMinutes(15)
        public static PasswordResetToken Create(Guid userId, string tokenHash, DateTime expiresAt) =>
            new() { UserId = userId, TokenHash = tokenHash, ExpiresAt = expiresAt };

        // Đánh dấu đã dùng — vô hiệu hóa token ngay sau lần reset đầu tiên
        public void MarkAsUsed() => UsedAt = DateTime.UtcNow;
    }
}
