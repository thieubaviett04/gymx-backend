namespace GymX.Application.Modules.Analytics.DTOs.Response;

public sealed class RevenueAnalyticsDto
{
    public bool HasData { get; init; }
    public FinancialSummaryDto Summary { get; init; } = new();
    public IReadOnlyCollection<RevenueSlotDto> ChartData { get; init; } = [];
    public IReadOnlyCollection<CategoryBreakdownDto> CategoryBreakdown { get; init; } = [];
}
