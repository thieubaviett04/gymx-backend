using GymX.Application.Modules.PenaltyRules.Commands.CreatePenaltyRule;
using GymX.Application.Modules.PenaltyRules.Commands.DeletePenaltyRule;
using GymX.Application.Modules.PenaltyRules.Commands.DiscontinuePenaltyRule;
using GymX.Application.Modules.PenaltyRules.Commands.UpdatePenaltyRule;
using GymX.Application.Modules.PenaltyRules.DTOs.Request;
using GymX.Application.Modules.PenaltyRules.Queries.GetPenaltyRuleById;
using GymX.Application.Modules.PenaltyRules.Queries.GetPenaltyRules;
using GymX.Application.Modules.PenaltyRules.Queries.GetPenaltyRulesLookup;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.PayrollController;

[ApiController]
[Route("api/v1/admin/penalty-rules")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class PenaltyRulesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPenaltyRules(
        [FromQuery] string? search,
        [FromQuery] string? severity,
        [FromQuery] string? status,
        [FromQuery] string? targetAudience,
        [FromQuery] string? sortBy,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetPenaltyRulesQuery(search, severity, status, targetAudience, sortBy, pageIndex, pageSize),
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> GetPenaltyRulesLookup(
        [FromQuery] string? targetAudience,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPenaltyRulesLookupQuery(targetAudience), cancellationToken);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPenaltyRuleById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPenaltyRuleByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "PenaltyRule.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPost]
    public async Task<IActionResult> CreatePenaltyRule([FromBody] CreatePenaltyRuleRequest request, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(CreatePenaltyRuleCommand.FromRequest(request), cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetPenaltyRuleById),
                new { id = result.Value.Id },
                new { success = true, data = result.Value, message = "Thêm quy định phạt thành công." });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePenaltyRule(
        [FromRoute] Guid id,
        [FromBody] UpdatePenaltyRuleRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(UpdatePenaltyRuleCommand.FromRequest(id, request), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value, message = "Cập nhật quy định phạt thành công." });
        }

        return result.Error.Code switch
        {
            "PenaltyRule.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePenaltyRule([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DeletePenaltyRuleCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Xóa quy định phạt thành công." });
        }

        return result.Error.Code switch
        {
            "PenaltyRule.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPatch("{id:guid}/discontinue")]
    public async Task<IActionResult> DiscontinuePenaltyRule([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DiscontinuePenaltyRuleCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Đã cập nhật trạng thái Ngừng áp dụng cho quy định phạt." });
        }

        return result.Error.Code switch
        {
            "PenaltyRule.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
