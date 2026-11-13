using System.Security.Claims;
using GymX.Application.Modules.Kpis.Commands.AssignEmployeeKpis;
using GymX.Application.Modules.Kpis.DTOs.Request;
using GymX.Application.Modules.Kpis.Queries.GetEmployeeKpiDetail;
using GymX.Application.Modules.Kpis.Queries.GetEmployeeKpisPaged;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.PayrollController;

[ApiController]
[Route("api/v1/admin/employee-kpis")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class EmployeeKpisController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetKpiProgress(
        [FromQuery] int month,
        [FromQuery] int year,
        [FromQuery] string? search,
        [FromQuery] string? position,
        [FromQuery] string? status,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (month <= 0 || month > 12) month = DateTime.UtcNow.AddHours(7).Month;
        if (year <= 2000) year = DateTime.UtcNow.AddHours(7).Year;

        var result = await sender.Send(new GetEmployeeKpisPagedQuery(month, year, search, position, status, pageIndex, pageSize), cancellationToken);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetKpiDetail([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetEmployeeKpiDetailQuery(id), cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }
        return NotFound(new { error = result.Error });
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignKpi([FromBody] AssignEmployeeKpiRequest request, CancellationToken cancellationToken = default)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _ = Guid.TryParse(userIdStr, out var currentUserId);

        var result = await sender.Send(new AssignEmployeeKpisCommand(
            request.KpiTemplateId, request.Month, request.Year, request.EmployeeIds, request.Note, currentUserId), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value, message = result.Value.Message });
        }
        return BadRequest(new { error = result.Error });
    }
}
