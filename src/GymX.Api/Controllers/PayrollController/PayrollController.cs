using GymX.Application.Modules.Payroll.Commands.AdjustPayslipKpiBonus;
using GymX.Application.Modules.Payroll.Commands.CalculatePayroll;
using GymX.Application.Modules.Payroll.Commands.LockPayrollPeriod;
using GymX.Application.Modules.Payroll.DTOs.Request;
using GymX.Application.Modules.Payroll.Queries.GetPayrollPeriodDetailById;
using GymX.Application.Modules.Payroll.Queries.GetPayrollPeriodsPaged;
using GymX.Application.Modules.Payroll.Queries.GetPayslipById;
using GymX.Application.Modules.Payroll.Queries.GetPayslipsPaged;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.PayrollController;

[ApiController]
[Route("api/v1/admin/payroll")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class PayrollController(ISender sender) : ControllerBase
{
    [HttpPost("periods/calculate")]
    public async Task<IActionResult> CalculatePayroll(
        [FromBody] CalculatePayrollRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new CalculatePayrollCommand(request.Month, request.Year), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpPost("periods/{id:guid}/lock")]
    public async Task<IActionResult> LockPayrollPeriod(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new LockPayrollPeriodCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("periods")]
    public async Task<IActionResult> GetPayrollPeriods(
        [FromQuery] int? year,
        [FromQuery] string? status,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPayrollPeriodsPagedQuery(
            year,
            status,
            pageIndex,
            pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("periods/{id:guid}")]
    public async Task<IActionResult> GetPayrollPeriodById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPayrollPeriodDetailByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return NotFound(new { error = result.Error });
    }

    [HttpGet("payslips")]
    public async Task<IActionResult> GetPayslips(
        [FromQuery] Guid? periodId,
        [FromQuery] Guid? employeeId,
        [FromQuery] string? status,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPayslipsPagedQuery(
            periodId,
            employeeId,
            status,
            pageIndex,
            pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("payslips/{id:guid}")]
    public async Task<IActionResult> GetPayslipById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPayslipByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return NotFound(new { error = result.Error });
    }

    [HttpPatch("payslips/{id:guid}/kpi-bonus")]
    public async Task<IActionResult> AdjustKpiBonus(
        [FromRoute] Guid id,
        [FromBody] AdjustPayslipKpiBonusRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new AdjustPayslipKpiBonusCommand(id, request.KpiBonusAmount, request.Note), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "Payroll.PayslipNotFound" => NotFound(new { error = result.Error }),
            "Payroll.PeriodAlreadyConfirmed" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
