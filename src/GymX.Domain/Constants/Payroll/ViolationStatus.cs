namespace GymX.Domain.Constants.Payroll;

public static class ViolationStatus
{
    public const string Confirmed = "CONFIRMED";
    public const string Cancelled = "CANCELLED";

    public static readonly string[] All = [Confirmed, Cancelled];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) &&
        All.Contains(status.Trim().ToUpperInvariant());
}

