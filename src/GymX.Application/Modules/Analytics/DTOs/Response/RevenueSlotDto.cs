namespace GymX.Application.Modules.Analytics.DTOs.Response;

public sealed class RevenueSlotDto
{
    public string Label { get; init; } = string.Empty;
    public decimal Revenue { get; init; }
    public decimal Expense { get; init; }
    public decimal Profit { get; init; }
    public int Transactions { get; init; }
    public double Percentage { get; init; }
}
