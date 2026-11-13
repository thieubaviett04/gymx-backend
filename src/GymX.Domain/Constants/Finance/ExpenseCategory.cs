namespace GymX.Domain.Constants.Finance;

public static class ExpenseCategory
{
    public const string Rent = "RENT";
    public const string Utilities = "UTILITIES";
    public const string Maintenance = "MAINTENANCE";
    public const string Equipment = "EQUIPMENT";
    public const string Other = "OTHER";

    public static readonly IReadOnlyCollection<string> All =
    [
        Rent,
        Utilities,
        Maintenance,
        Equipment,
        Other
    ];

    public static bool IsValid(string category) => All.Contains(category);
}
