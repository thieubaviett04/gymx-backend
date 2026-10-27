using GymX.Application.Modules.Trainers.Commands.UpdateMyTrainerProfile;
using GymX.Application.Modules.Trainers.Queries.GetMyTrainerProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.TrainersController
{
    [ApiController]
    [Route("api/v1/trainers")]
    [Authorize]
    public class TrainersController(ISender sender) : ControllerBase
    {
        [HttpGet("me")]
        public async Task<IActionResult> GetMyTrainerProfile(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetMyTrainerProfileQuery(), cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new { success = true, data = result.Value });
            }

            return result.Error.Code switch
            {
                "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
                "Trainer.NotFound" => NotFound(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }

        [HttpPatch("me")]
        public async Task<IActionResult> UpdateMyTrainerProfile([FromBody] UpdateMyTrainerProfileCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new { success = true, data = result.Value, message = "Cập nhật hồ sơ huấn luyện viên thành công." });
            }

            return result.Error.Code switch
            {
                "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
                "Employee.NotFound" => NotFound(new { error = result.Error }),
                "Trainer.NotFound" => NotFound(new { error = result.Error }),
                "Employee.NationalIdExists" => Conflict(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }
    }
}
