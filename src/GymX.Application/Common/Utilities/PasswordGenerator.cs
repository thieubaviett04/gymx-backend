using System.Security.Cryptography;

namespace GymX.Application.Common.Utilities;

public static class PasswordGenerator
{
    private const string Upper = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
    private const string Lower = "abcdefghijkmnopqrstuvwxyz";
    private const string Digits = "0123456789";
    private const string Specials = "!@$?_-";

    public static string Generate(int length = 8)
    {
        var randomBytes = new byte[length];
        RandomNumberGenerator.Fill(randomBytes);

        var chars = new char[length];
        chars[0] = Upper[randomBytes[0] % Upper.Length];
        chars[1] = Lower[randomBytes[1] % Lower.Length];
        chars[2] = Digits[randomBytes[2] % Digits.Length];
        chars[3] = Specials[randomBytes[3] % Specials.Length];

        const string allChars = Upper + Lower + Digits + Specials;
        for (var i = 4; i < length; i++)
        {
            chars[i] = allChars[randomBytes[i] % allChars.Length];
        }

        var shuffled = chars.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray();
        return new string(shuffled);
    }
}
