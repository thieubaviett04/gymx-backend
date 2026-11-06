namespace GymX.Domain.Constants;

public static class PackageStatus
{
    public const string Inactive = "INACTIVE";
    public const string Active = "ACTIVE";
    public const string Discontinued = "DISCONTINUED";

    public static readonly string[] All = [Inactive, Active, Discontinued];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) &&
        All.Contains(status.Trim().ToUpperInvariant());
}
