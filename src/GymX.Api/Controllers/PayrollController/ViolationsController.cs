using System.Security.Claims;
using GymX.Application.Common.Interfaces;
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

    [HttpPost("upload-evidence")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadEvidence(
        IFormFile file,
        [FromServices] IFileStorageService fileStorageService,
        CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { error = new { code = "Violation.EvidenceFileEmpty", description = "Vui lòng chọn file bằng chứng để tải lên." } });
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(new { error = new { code = "Violation.InvalidFileType", description = "Chỉ chấp nhận các tệp định dạng JPG, PNG, WEBP hoặc PDF." } });
        }

        if (file.Length > 10 * 1024 * 1024)
        {
            return BadRequest(new { error = new { code = "Violation.FileTooLarge", description = "Dung lượng tệp không được vượt quá 10MB." } });
        }

        await using var stream = file.OpenReadStream();
        var evidenceUrl = await fileStorageService.SaveFileAsync(stream, file.FileName, "violations", cancellationToken);

        return Ok(new { success = true, data = new { evidenceUrl }, message = "Tải lên tệp bằng chứng thành công." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteViolation(
        [FromRoute] Guid id,
        [FromQuery] string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var cancelledBy))
        {
            return Unauthorized();
        }

        var cancellationReason = !string.IsNullOrWhiteSpace(reason) ? reason.Trim() : "Hủy bởi quản trị viên";

        var result = await sender.Send(new CancelViolationCommand(id, cancellationReason, cancelledBy), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Xóa biên bản vi phạm thành công." });
        }

        return result.Error.Code switch
        {
            "Violation.NotFound" => NotFound(new { error = result.Error }),
            "Violation.PayrollPeriodClosed" or "Violation.AlreadyCancelled" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
