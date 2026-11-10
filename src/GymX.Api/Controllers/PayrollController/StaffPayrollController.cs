using GymX.Application.Modules.Payroll.Queries.GetMyPayslips;
using GymX.Application.Modules.Payroll.Queries.GetPayslipById;
using GymX.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.PayrollController;

[ApiController]
[Route("api/v1/staff/payroll")]
[Authorize]
public class StaffPayrollController(ISender sender) : ControllerBase
{
    [HttpGet("my-payslips")]
    public async Task<IActionResult> GetMyPayslips(CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetMyPayslipsQuery(), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("my-payslips/{id:guid}")]
    public async Task<IActionResult> GetMyPayslipById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPayslipByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return NotFound(new { error = result.Error });
    }
}
