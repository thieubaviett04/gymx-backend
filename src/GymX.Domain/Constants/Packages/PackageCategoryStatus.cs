namespace GymX.Domain.Constants.Packages;

public static class PackageCategoryStatus
{
    public const string Active = "ACTIVE";
    public const string Inactive = "INACTIVE";

    public static readonly string[] All = [Active, Inactive];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) &&
        All.Contains(status.Trim().ToUpperInvariant());
}

