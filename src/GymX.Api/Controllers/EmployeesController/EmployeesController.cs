using GymX.Application.Modules.Employees.Commands.UpdateMyEmployeeProfile;
using GymX.Application.Modules.Employees.Queries.GetMyEmployeeProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.EmployeesController
{
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
        public async Task<IActionResult> UpdateMyEmployeeProfile([FromBody] UpdateMyEmployeeProfileCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

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
    }
}
