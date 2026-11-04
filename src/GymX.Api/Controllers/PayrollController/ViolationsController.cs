using System.Security.Claims;
using GymX.Application.Modules.Violations.Commands.CancelViolation;
using GymX.Application.Modules.Violations.Commands.CreateViolation;
using GymX.Application.Modules.Violations.DTOs.Request;
using GymX.Application.Modules.Violations.Queries.GetEmployeesForLookup;
using GymX.Application.Modules.Violations.Queries.GetViolationById;
using GymX.Application.Modules.Violations.Queries.GetViolations;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.PayrollController;

[ApiController]
[Route("api/v1/admin/violations")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class ViolationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetViolations(
        [FromQuery] string? search,
        [FromQuery] Guid? employeeId,
        [FromQuery] Guid? penaltyRuleId,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? sortBy,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetViolationsQuery(
            search, employeeId, penaltyRuleId, status, fromDate, toDate, sortBy, pageIndex, pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("employees-lookup")]
    public async Task<IActionResult> GetEmployeesLookup([FromQuery] string? search, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetEmployeesForLookupQuery(search), cancellationToken);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetViolationById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetViolationByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "Violation.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPost]
    public async Task<IActionResult> CreateViolation([FromBody] CreateViolationRequest request, CancellationToken cancellationToken = default)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var recordedBy))
        {
            return Unauthorized();
        }

        var command = new CreateViolationCommand(
            request.EmployeeId,
            request.PenaltyRuleId,
            request.OccurredAt,
            recordedBy,
            request.AttendanceRecordId,
            request.EvidenceUrl,
            request.Note);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetViolationById), new { id = result.Value.Id }, new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "Violation.EmployeeNotFound" or "Violation.PenaltyRuleNotFound" => NotFound(new { error = result.Error }),
            "Violation.PayrollPeriodClosed" or "Violation.CodeGenerationFailed" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> CancelViolation([FromRoute] Guid id, [FromBody] CancelViolationRequest request, CancellationToken cancellationToken = default)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var cancelledBy))
        {
            return Unauthorized();
        }

        var result = await sender.Send(new CancelViolationCommand(id, request.CancellationReason, cancelledBy), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Biên bản vi phạm đã được hủy thành công." });
        }

        return result.Error.Code switch
        {
            "Violation.NotFound" => NotFound(new { error = result.Error }),
            "Violation.PayrollPeriodClosed" or "Violation.AlreadyCancelled" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}

