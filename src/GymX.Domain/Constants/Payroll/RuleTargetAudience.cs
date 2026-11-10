namespace GymX.Domain.Constants;

public static class RuleTargetAudience
{
    public const string General = "GENERAL";
    public const string Member = "MEMBER";
    public const string Staff = "STAFF";
    public const string Trainer = "TRAINER";

    public static readonly string[] All = [General, Member, Staff, Trainer];

    public static bool IsValid(string? audience) =>
        !string.IsNullOrWhiteSpace(audience) &&
        All.Contains(audience.Trim().ToUpperInvariant());
}

