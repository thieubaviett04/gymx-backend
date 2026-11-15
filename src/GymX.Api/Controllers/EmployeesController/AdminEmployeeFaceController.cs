using GymX.Application.Modules.Employees.Commands.RegisterEmployeeFace;
using GymX.Application.Modules.Employees.Commands.RemoveEmployeeFace;
using GymX.Application.Modules.Employees.Queries.GetEmployeeFaceByCode;
using GymX.Application.Modules.Employees.Queries.GetEmployeeFaceDetail;
using GymX.Domain.Constants;
using GymX.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.EmployeesController;

[ApiController]
[Route("api/v1/admin/employees/{employeeId:guid}/face")]
[Authorize(Roles = $"{RoleCode.Admin},{RoleCode.Receptionist}")]
public class AdminEmployeeFaceController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> RegisterFace(
        [FromRoute] Guid employeeId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { error = EmployeeFaceErrors.FileEmpty });
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        var imageBytes = memoryStream.ToArray();

        var command = new RegisterEmployeeFaceCommand(
            employeeId,
            imageBytes,
            file.FileName,
            file.ContentType,
            file.Length);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value, message = "Đăng ký khuôn mặt nhân viên thành công." });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "Employee.NotFound" => NotFound(new { error = result.Error }),
            "FaceData.NotFound" => NotFound(new { error = result.Error }),
            "FaceRecognition.NoFaceFound" => BadRequest(new { error = result.Error }),
            "FaceRecognition.MultipleFacesDetected" => BadRequest(new { error = result.Error }),
            "FaceRegistration.InvalidFileFormat" => BadRequest(new { error = result.Error }),
            "FaceRegistration.FileTooLarge" => BadRequest(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetFaceDetail(
        [FromRoute] Guid employeeId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetEmployeeFaceDetailQuery(employeeId), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "Employee.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpGet("/api/v1/admin/employees/code/{employeeCode}/face")]
    public async Task<IActionResult> GetFaceDetailByCode(
        [FromRoute] string employeeCode,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetEmployeeFaceByCodeQuery(employeeCode), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "Employee.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveFace(
        [FromRoute] Guid employeeId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveEmployeeFaceCommand(employeeId), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Hủy đăng ký khuôn mặt nhân viên thành công." });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "Employee.NotFound" => NotFound(new { error = result.Error }),
            "FaceData.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
