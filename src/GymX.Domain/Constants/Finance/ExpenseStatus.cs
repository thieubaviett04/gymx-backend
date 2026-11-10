namespace GymX.Domain.Constants.Finance;

public static class ExpenseStatus
{
    public const string Confirmed = "CONFIRMED";
    public const string Cancelled = "CANCELLED";

    public static readonly IReadOnlyCollection<string> All =
    [
        Confirmed,
        Cancelled
    ];

    public static bool IsValid(string status) => All.Contains(status);
}
