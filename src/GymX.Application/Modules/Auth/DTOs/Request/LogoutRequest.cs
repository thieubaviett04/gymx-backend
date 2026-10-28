namespace GymX.Application.Modules.Auth.DTOs.Request;

public class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
