using GymX.Application.Modules.Users.Commands.ChangePassword;
using GymX.Application.Modules.Users.Commands.UpdateMyProfile;
using GymX.Application.Modules.Users.Commands.UploadAvatar;
using GymX.Application.Modules.Users.DTOs.Request;
using GymX.Application.Modules.Users.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.UsersController;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyProfileQuery(), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "User.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPatch("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(UpdateMyProfileCommand.FromRequest(request), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value, message = "Cập nhật thông tin thành công." });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "User.NotFound" => NotFound(new { error = result.Error }),
            "User.PhoneExists" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPut("me/avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { error = new { code = "Avatar.Empty", description = "Vui lòng chọn file ảnh để tải lên." } });
        }

        await using var stream = file.OpenReadStream();
        var command = new UploadAvatarCommand(stream, file.FileName, file.ContentType, file.Length);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = new { avatarUrl = result.Value }, message = "Cập nhật ảnh đại diện thành công." });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "User.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPut("me/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(ChangePasswordCommand.FromRequest(request), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = result.Value });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "User.NotFound" => NotFound(new { error = result.Error }),
            "Auth.WrongPassword" => BadRequest(new { error = result.Error }),
            "Auth.GoogleAccount" => BadRequest(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
