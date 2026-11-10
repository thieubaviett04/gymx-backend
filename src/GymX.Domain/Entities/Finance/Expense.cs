using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Finance;

namespace GymX.Domain.Entities.Finance;

public class Expense : EntityAuditBase
{
    public string ExpenseCode { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public DateOnly ExpenseDate { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Status { get; private set; } = ExpenseStatus.Confirmed;
    public Guid RecordedBy { get; private set; }
    public Guid? CancelledBy { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    protected Expense() { }

    public static Expense Create(
        string expenseCode,
        string category,
        decimal amount,
        DateOnly expenseDate,
        string description,
        Guid recordedBy)
    {
        return new Expense
        {
            ExpenseCode = expenseCode.Trim().ToUpperInvariant(),
            Category = category.Trim().ToUpperInvariant(),
            Amount = amount,
            ExpenseDate = expenseDate,
            Description = description.Trim(),
            Status = ExpenseStatus.Confirmed,
            RecordedBy = recordedBy
        };
    }

    public void Cancel(Guid cancelledBy, string cancellationReason)
    {
        Status = ExpenseStatus.Cancelled;
        CancelledBy = cancelledBy;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = cancellationReason.Trim();
    }
}
