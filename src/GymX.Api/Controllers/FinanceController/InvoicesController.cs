using GymX.Application.Modules.Invoices.Commands.CreateInvoice;
using GymX.Application.Modules.Invoices.DTOs.Request;
using GymX.Application.Modules.Invoices.Queries.GetInvoicesPaged;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.FinanceController;

[ApiController]
[Route("api/v1/admin/invoices")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class InvoicesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetInvoices(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] string? sortBy,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetInvoicesPagedQuery(
            search,
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

    [HttpPost]
    public async Task<IActionResult> CreateInvoice(
        [FromBody] CreateInvoiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new CreateInvoiceCommand(
            request.CustomerName,
            request.CustomerPhone,
            request.MemberCode,
            request.Amount,
            request.PaymentMethod,
            request.ReferenceType,
            request.IssuedDate,
            request.Note), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }
}
