using GymX.Application.Common.Interfaces.Authencation;

namespace GymX.Infrastructure.Services.Authentication;

/// <summary>
/// "Thợ băm mật khẩu" — dùng BCrypt (thuật toán băm chậm, chống brute force).
/// Tầng Application chỉ biết IPasswordHasher, không biết bên trong dùng BCrypt hay SHA256.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
