namespace GymX.Application.Modules.Auth.DTOs.Request;

public class VerifyOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}
