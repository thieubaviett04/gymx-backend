namespace GymX.Application.Modules.Auth.DTOs;

public class VerifyOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}
