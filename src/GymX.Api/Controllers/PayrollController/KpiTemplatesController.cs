using GymX.Application.Modules.Kpis.Commands.CreateKpiTemplate;
using GymX.Application.Modules.Kpis.Commands.DeleteKpiTemplate;
using GymX.Application.Modules.Kpis.Commands.UpdateKpiTemplate;
using GymX.Application.Modules.Kpis.DTOs.Request;
using GymX.Application.Modules.Kpis.Queries.GetKpiTemplateById;
using GymX.Application.Modules.Kpis.Queries.GetKpiTemplates;
using GymX.Application.Modules.Kpis.Queries.GetKpiTemplatesLookup;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.PayrollController;

[ApiController]
[Route("api/v1/admin/kpi-templates")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class KpiTemplatesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTemplates(
        [FromQuery] string? search,
        [FromQuery] string? targetRole,
        [FromQuery] string? status,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetKpiTemplatesQuery(search, targetRole, status, pageIndex, pageSize), cancellationToken);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup(
        [FromQuery] string? targetRole,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetKpiTemplatesLookupQuery(targetRole), cancellationToken);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTemplateById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetKpiTemplateByIdQuery(id), cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }
        return NotFound(new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateKpiTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new CreateKpiTemplateCommand(
            request.Name, request.TargetRole, request.TargetRevenue, request.TargetBonusAmount,
            request.TargetPtSessions, request.TargetNewMembers, request.Description, request.Status), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value, message = "Tạo mẫu KPI thành công." });
        }
        return result.Error.Code == "KpiTemplate.NameExists"
            ? Conflict(new { error = result.Error })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTemplate([FromRoute] Guid id, [FromBody] UpdateKpiTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new UpdateKpiTemplateCommand(
            id, request.Name, request.TargetRole, request.TargetRevenue, request.TargetBonusAmount,
            request.TargetPtSessions, request.TargetNewMembers, request.Description, request.Status), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value, message = "Cập nhật mẫu KPI thành công." });
        }
        return result.Error.Code switch
        {
            "KpiTemplate.NotFound" => NotFound(new { error = result.Error }),
            "KpiTemplate.NameExists" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTemplate([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DeleteKpiTemplateCommand(id), cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Xóa mẫu KPI thành công." });
        }
        return result.Error.Code == "KpiTemplate.NotFound"
            ? NotFound(new { error = result.Error })
            : BadRequest(new { error = result.Error });
    }
}

