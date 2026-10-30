namespace GymX.Domain.Constants;

public static class PackageStatus
{
    public const string Inactive = "INACTIVE";         // Chưa hoạt động
    public const string Active = "ACTIVE";             // Hoạt động
    public const string Discontinued = "DISCONTINUED"; // Ngừng kinh doanh

    public static readonly string[] All = [Inactive, Active, Discontinued];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) &&
        All.Contains(status.Trim().ToUpperInvariant());
}
