
namespace GymX.Domain.Entities.Identity
{
    // Bảng trung gian User ↔ Role — một User có nhiều Role và ngược lại
    public class UserRole
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }
        public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;

        // Navigation properties — EF Core dùng để JOIN bảng, không cần query thủ công
        public User User { get; private set; } = null!;
        public Role Role { get; private set; } = null!;

        // Protected constructor — bắt buộc có để EF Core có thể tạo instance khi load từ DB
        protected UserRole() { }

        // Factory method — cách tạo object an toàn, thay thế cho constructor public
        public static UserRole Create(Guid userId, Guid roleId) =>
            new() { UserId = userId, RoleId = roleId };
    }
}
