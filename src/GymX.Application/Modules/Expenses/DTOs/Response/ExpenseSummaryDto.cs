namespace GymX.Application.Modules.Expenses.DTOs.Response;

public sealed class ExpenseSummaryDto
{
    public Guid Id { get; init; }
    public string ExpenseCode { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateOnly ExpenseDate { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string RecordedByName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
