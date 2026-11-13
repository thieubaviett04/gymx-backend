namespace GymX.Domain.Constants.Finance;

public static class InvoiceReferenceType
{
    public const string Package = "PACKAGE";
    public const string PtSession = "PT_SESSION";
    public const string DayTicket = "DAY_TICKET";
    public const string Retail = "RETAIL";
    public const string ServiceFee = "SERVICE_FEE";
    public const string Other = "OTHER";

    public static readonly IReadOnlyCollection<string> All =
    [
        Package,
        PtSession,
        DayTicket,
        Retail,
        ServiceFee,
        Other
    ];

    public static bool IsValid(string type) => All.Contains(type);
}

