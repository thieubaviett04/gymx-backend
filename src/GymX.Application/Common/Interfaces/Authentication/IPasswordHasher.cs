
namespace GymX.Application.Common.Interfaces.Authencation
{
    // Abstraction để băm mật khẩu (chống phụ thuộc trực tiếp vào BCrypt ở tầng Application)
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
