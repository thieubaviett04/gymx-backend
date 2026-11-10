namespace GymX.Application.Modules.Expenses.DTOs.Response;

public sealed class ExpenseDetailDto
{
    public Guid Id { get; init; }
    public string ExpenseCode { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateOnly ExpenseDate { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public Guid RecordedById { get; init; }
    public string RecordedByName { get; init; } = string.Empty;
    public Guid? CancelledById { get; init; }
    public string? CancelledByName { get; init; }
    public DateTime? CancelledAt { get; init; }
    public string? CancellationReason { get; init; }
    public DateTime CreatedAt { get; init; }
}
