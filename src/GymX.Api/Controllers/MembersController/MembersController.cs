using GymX.Application.Modules.Members.Commands.UpdateMyMemberProfile;
using GymX.Application.Modules.Members.Queries.GetMyMemberProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.MembersController
{
    [ApiController]
    [Route("api/v1/members")]
    [Authorize]
    public class MembersController(ISender sender) : ControllerBase
    {
        [HttpGet("me")]
        public async Task<IActionResult> GetMyMemberProfile(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetMyMemberProfileQuery(), cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new { success = true, data = result.Value });
            }

            return result.Error.Code switch
            {
                "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
                "Member.NotFound" => NotFound(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }

        [HttpPatch("me")]
        public async Task<IActionResult> UpdateMyMemberProfile([FromBody] UpdateMyMemberProfileCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(new { success = true, data = result.Value, message = "Cập nhật hồ sơ hội viên thành công." });
            }

            return result.Error.Code switch
            {
                "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
                "Member.NotFound" => NotFound(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }
    }
}
