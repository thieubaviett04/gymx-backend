namespace GymX.Application.Modules.Auth.DTOs.Response;

public record PendingRegistrationCacheItem(
    string FullName,
    string Email,
    string PhoneNumber,
    string PasswordHash,
    string OtpCode);
