using GymX.Application.Modules.Analytics.Queries.ExportRevenueReport;
using GymX.Application.Modules.Analytics.Queries.GetRevenueAnalytics;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.FinanceController;

[ApiController]
[Route("api/v1/admin/analytics")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class AnalyticsController(ISender sender) : ControllerBase
{
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueAnalytics(
        [FromQuery] string period = "today",
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] string? metric = "revenue",
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetRevenueAnalyticsQuery(
            period,
            fromDate,
            toDate,
            metric), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportRevenueReport(
        [FromQuery] string period = "today",
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] string format = "excel",
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new ExportRevenueReportQuery(
            period,
            fromDate,
            toDate,
            format), cancellationToken);

        if (result.IsSuccess)
        {
            var fileName = $"Bao_Cao_Doanh_Thu_{DateTime.UtcNow.AddHours(7):yyyyMMdd_HHmmss}.csv";
            return File(result.Value, "text/csv; charset=utf-8", fileName);
        }

        return BadRequest(new { error = result.Error });
    }
}
