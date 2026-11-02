namespace GymX.Domain.Constants;

public static class PenaltyRuleStatus
{
    public const string Pending = "PENDING";
    public const string Applied = "APPLIED";
    public const string Discontinued = "DISCONTINUED";

    public static readonly string[] All = [Pending, Applied, Discontinued];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) &&
        All.Contains(status.Trim().ToUpperInvariant());
}
