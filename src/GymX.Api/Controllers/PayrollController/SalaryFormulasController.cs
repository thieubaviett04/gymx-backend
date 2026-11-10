using GymX.Application.Modules.SalaryFormulas.Commands.CreateSalaryFormula;
using GymX.Application.Modules.SalaryFormulas.Commands.ToggleSalaryFormulaStatus;
using GymX.Application.Modules.SalaryFormulas.Commands.UpdateSalaryFormula;
using GymX.Application.Modules.SalaryFormulas.DTOs.Request;
using GymX.Application.Modules.SalaryFormulas.Queries.GetSalaryFormulaById;
using GymX.Application.Modules.SalaryFormulas.Queries.GetSalaryFormulasLookup;
using GymX.Application.Modules.SalaryFormulas.Queries.GetSalaryFormulasPaged;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.PayrollController;

[ApiController]
[Route("api/v1/admin/payroll/formulas")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class SalaryFormulasController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFormulas(
        [FromQuery] string? search,
        [FromQuery] string? targetRole,
        [FromQuery] string? status,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetSalaryFormulasPagedQuery(
            search,
            targetRole,
            status,
            pageIndex,
            pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup(
        [FromQuery] string? targetRole,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetSalaryFormulasLookupQuery(targetRole), cancellationToken);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFormulaById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetSalaryFormulaByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return NotFound(new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> CreateFormula(
        [FromBody] CreateSalaryFormulaRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new CreateSalaryFormulaCommand(
            request.Name,
            request.TargetRole,
            request.BaseRateType,
            request.BaseRate,
            request.StandardWorkDays,
            request.PricePerPtSession,
            request.SalesCommissionPercent,
            request.EffectiveDate,
            request.Description), cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetFormulaById),
                new { id = result.Value.Id },
                new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateFormula(
        [FromRoute] Guid id,
        [FromBody] UpdateSalaryFormulaRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new UpdateSalaryFormulaCommand(
            id,
            request.Name,
            request.TargetRole,
            request.BaseRateType,
            request.BaseRate,
            request.StandardWorkDays,
            request.PricePerPtSession,
            request.SalesCommissionPercent,
            request.EffectiveDate,
            request.Description), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ToggleStatus(
        [FromRoute] Guid id,
        [FromBody] ToggleSalaryFormulaStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new ToggleSalaryFormulaStatusCommand(
            id,
            request.IsActive), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }
}
