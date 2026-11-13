using System.Security.Claims;
using GymX.Application.Modules.Kpis.Queries.GetMyKpi;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.PayrollController;

[ApiController]
[Route("api/v1/employees/my-kpi")]
[Authorize]
public class MyKpiController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyKpi(
        [FromQuery] int? month,
        [FromQuery] int? year,
        CancellationToken cancellationToken = default)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var currentUserId))
        {
            return Unauthorized();
        }

        var today = DateTime.UtcNow.AddHours(7);
        var queryMonth = month.GetValueOrDefault(today.Month);
        var queryYear = year.GetValueOrDefault(today.Year);

        var result = await sender.Send(new GetMyKpiQuery(currentUserId, queryMonth, queryYear), cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }
        return BadRequest(new { error = result.Error });
    }
}

