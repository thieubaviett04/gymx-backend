namespace GymX.Application.Modules.Analytics.DTOs.Response;

public sealed class CategoryBreakdownDto
{
    public string CategoryName { get; init; } = string.Empty;
    public decimal Revenue { get; init; }
    public double Percentage { get; init; }
}
