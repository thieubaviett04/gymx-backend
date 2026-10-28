using GymX.Application.Modules.Employees.Commands.CreateEmployee;
using GymX.Application.Modules.Employees.Commands.DeleteEmployee;
using GymX.Application.Modules.Employees.Commands.UpdateEmployee;
using GymX.Application.Modules.Employees.Commands.UpdateMyEmployeeProfile;
using GymX.Application.Modules.Employees.DTOs.Request;
using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Application.Modules.Employees.Queries.GetEmployeeById;
using GymX.Application.Modules.Employees.Queries.GetEmployees;
using GymX.Application.Modules.Employees.Queries.GetMyEmployeeProfile;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.EmployeesController;

[ApiController]
[Route("api/v1/employees")]
[Authorize]
public class EmployeesController(ISender sender) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyEmployeeProfile(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyEmployeeProfileQuery(), cancellationToken);

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

    [HttpPatch("me")]
    public async Task<IActionResult> UpdateMyEmployeeProfile([FromBody] UpdateMyEmployeeProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(UpdateMyEmployeeProfileCommand.FromRequest(request), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value, message = "Cập nhật hồ sơ nhân viên thành công." });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "Employee.NotFound" => NotFound(new { error = result.Error }),
            "Employee.NationalIdExists" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpGet]
    [Authorize(Policy = PolicyNames.RequireAdmin)]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] string? search,
        [FromQuery] string? position,
        [FromQuery] string? status,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetEmployeesQuery(search, position, status, pageIndex, pageSize),
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PolicyNames.RequireAdmin)]
    public async Task<IActionResult> GetEmployeeById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetEmployeeByIdQuery(id), cancellationToken);

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

    [HttpPost]
    [Authorize(Policy = PolicyNames.RequireAdmin)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(CreateEmployeeCommand.FromRequest(request), cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetEmployeeById),
                new { id = result.Value.Id },
                new { success = true, data = result.Value, message = "Thêm nhân viên thành công." });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "User.EmailExists" => Conflict(new { error = result.Error }),
            "User.PhoneExists" => Conflict(new { error = result.Error }),
            "Employee.NationalIdExists" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyNames.RequireAdmin)]
    public async Task<IActionResult> UpdateEmployee([FromRoute] Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(UpdateEmployeeCommand.FromRequest(id, request), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value, message = "Cập nhật thông tin nhân viên thành công." });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "Employee.NotFound" => NotFound(new { error = result.Error }),
            "User.NotFound" => NotFound(new { error = result.Error }),
            "User.EmailExists" => Conflict(new { error = result.Error }),
            "User.PhoneExists" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyNames.RequireAdmin)]
    public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DeleteEmployeeCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Xóa nhân viên thành công." });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "Employee.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
