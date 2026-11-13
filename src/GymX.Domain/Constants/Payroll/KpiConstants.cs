namespace GymX.Domain.Constants.Payroll;

public static class KpiStatus
{
    public const string InProgress = "IN_PROGRESS";
    public const string Achieved = "ACHIEVED";
    public const string Failed = "FAILED";
    public const string Closed = "CLOSED";

    public static readonly string[] All = [InProgress, Achieved, Failed, Closed];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) &&
        All.Contains(status.Trim().ToUpperInvariant());
}

public static class KpiTemplateStatus
{
    public const string Active = "ACTIVE";
    public const string Inactive = "INACTIVE";

    public static readonly string[] All = [Active, Inactive];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) &&
        All.Contains(status.Trim().ToUpperInvariant());
}

