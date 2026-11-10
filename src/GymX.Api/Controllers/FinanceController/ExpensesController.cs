using GymX.Application.Modules.Expenses.Commands.CancelExpense;
using GymX.Application.Modules.Expenses.Commands.CreateExpense;
using GymX.Application.Modules.Expenses.DTOs.Request;
using GymX.Application.Modules.Expenses.Queries.GetExpenseById;
using GymX.Application.Modules.Expenses.Queries.GetExpensesPaged;
using GymX.Domain.Constants.Finance;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.FinanceController;

[ApiController]
[Route("api/v1/admin/expenses")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class ExpensesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetExpenses(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] string? status,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] string? sortBy,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetExpensesPagedQuery(
            search,
            category,
            status,
            fromDate,
            toDate,
            sortBy,
            pageIndex,
            pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        return Ok(new { success = true, data = ExpenseCategory.All });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetExpenseById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetExpenseByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return NotFound(new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense(
        [FromBody] CreateExpenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new CreateExpenseCommand(
            request.Category,
            request.Amount,
            request.ExpenseDate,
            request.Description), cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetExpenseById),
                new { id = result.Value.Id },
                new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelExpense(
        [FromRoute] Guid id,
        [FromBody] CancelExpenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new CancelExpenseCommand(
            id,
            request.CancellationReason), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }
}
