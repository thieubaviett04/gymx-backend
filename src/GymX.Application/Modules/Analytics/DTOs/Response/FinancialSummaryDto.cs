namespace GymX.Application.Modules.Analytics.DTOs.Response;

public sealed class FinancialSummaryDto
{
    public decimal TotalRevenue { get; init; }
    public decimal TotalExpense { get; init; }
    public decimal NetProfit { get; init; }
    public int TotalTransactions { get; init; }
    public double RevenueGrowthRate { get; init; }
    public decimal AverageOrderValue { get; init; }
}
