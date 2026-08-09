using GymX.Domain.Common.Models;

namespace GymX.Domain.Entities.Identity
{
    // Vai trò hệ thống — liên kết với User qua bảng trung gian UserRole
    public class Role : EntityBase
    {
        public string Code { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Navigation: 1 Role gán cho nhiều UserRole
        public ICollection<UserRole> UserRoles { get; private set; } = [];

        protected Role() { }

        // Dùng factory method thay constructor public để kiểm soát cách tạo object
        public static Role Create(string code, string name) =>
            new() { Code = code, Name = name };
    }
}
