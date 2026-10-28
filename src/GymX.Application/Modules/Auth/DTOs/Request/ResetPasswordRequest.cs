namespace GymX.Application.Modules.Auth.DTOs.Request;

public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
