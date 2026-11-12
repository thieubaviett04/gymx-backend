using GymX.Application.Modules.PayrollBonuses.Commands.CreatePayrollBonus;
using GymX.Application.Modules.PayrollBonuses.Commands.DeletePayrollBonus;
using GymX.Application.Modules.PayrollBonuses.Commands.UpdatePayrollBonus;
using GymX.Application.Modules.PayrollBonuses.DTOs;
using GymX.Application.Modules.PayrollBonuses.Queries.GetBonusEmployeesLookup;
using GymX.Application.Modules.PayrollBonuses.Queries.GetPayrollBonusesPaged;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.PayrollController;

[ApiController]
[Route("api/v1/admin/payroll/bonuses")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class PayrollBonusesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBonuses(
        [FromQuery] int? month,
        [FromQuery] int? year,
        [FromQuery] string? search,
        [FromQuery] string? position,
        [FromQuery] string? bonusType,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPayrollBonusesPagedQuery(
            month,
            year,
            search,
            position,
            bonusType,
            pageIndex,
            pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("employees-lookup")]
    public async Task<IActionResult> GetEmployeesLookup(
        [FromQuery] int? month,
        [FromQuery] int? year,
        [FromQuery] string? search,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetBonusEmployeesLookupQuery(month, year, search), cancellationToken);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost]
    public async Task<IActionResult> CreateBonus(
        [FromBody] CreatePayrollBonusRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new CreatePayrollBonusCommand(
            request.Month,
            request.Year,
            request.BonusType,
            request.TargetType,
            request.Position,
            request.EmployeeIds,
            request.Amount,
            request.Reason), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = new { createdCount = result.Value } });
        }

        return result.Error.Code switch
        {
            "Payroll.PeriodAlreadyConfirmed" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBonus(
        [FromRoute] Guid id,
        [FromBody] UpdatePayrollBonusRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new UpdatePayrollBonusCommand(
            id,
            request.Amount,
            request.Reason,
            request.BonusType), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "PayrollBonus.NotFound" => NotFound(new { error = result.Error }),
            "Payroll.PeriodAlreadyConfirmed" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBonus(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DeletePayrollBonusCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "PayrollBonus.NotFound" => NotFound(new { error = result.Error }),
            "Payroll.PeriodAlreadyConfirmed" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}

