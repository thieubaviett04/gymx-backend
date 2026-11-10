namespace GymX.Application.Modules.Expenses.DTOs.Request;

public sealed record CreateExpenseRequest(
    string Category,
    decimal Amount,
    DateOnly ExpenseDate,
    string Description);
