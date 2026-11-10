namespace GymX.Application.Modules.Expenses.DTOs.Request;

public sealed record CancelExpenseRequest(
    string CancellationReason);
