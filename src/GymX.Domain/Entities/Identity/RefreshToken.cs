using GymX.Domain.Common.Models;

namespace GymX.Domain.Entities.Identity
{
    // Refresh Token của một User — hỗ trợ token rotation để tăng bảo mật
    public class RefreshToken : EntityBase
    {
        public Guid UserId { get; private set; }
        // Chỉ lưu hash của token, không lưu giá trị gốc — tránh lộ token nếu DB bị tấn công
        public Guid SessionId { get; private set; }
        public string TokenHash { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        // Token Rotation: lưu Id của token mới thay thế để audit trail
        public Guid? ReplacedByTokenId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public User User { get; private set; } = null!;

        // Computed properties — tính toán trực tiếp từ dữ liệu, không lưu DB
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsActive => !IsExpired && !IsRevoked;
        protected RefreshToken() { }

        // ExpiresAt truyền vào từ config (ví dụ: DateTime.UtcNow.AddDays(7))
        public static RefreshToken Create(Guid userId, string tokenHash, DateTime expiresAt, Guid sessionId) =>
            new() { UserId = userId, TokenHash = tokenHash, ExpiresAt = expiresAt, SessionId = sessionId };

        // Thu hồi token — ghi lại Id token mới thay thế nếu là rotation
        public void Revoke(Guid? replacedByTokenId = null)
        {
            RevokedAt = DateTime.UtcNow;
            ReplacedByTokenId = replacedByTokenId;
        }
    }
}
