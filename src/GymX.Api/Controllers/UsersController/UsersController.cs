using GymX.Application.Features.Users.Commands.ChangePassword;
using GymX.Application.Features.Users.Commands.UpdateMyProfile;
using GymX.Application.Features.Users.Commands.UploadAvatar;
using GymX.Application.Features.Users.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.UsersController
{
    [ApiController]
    [Route("api/v1/users")]
    [Authorize]
    public class UsersController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Lấy thông tin hồ sơ của người dùng hiện tại (dùng chung cho mọi actor đã đăng nhập)
        /// </summary>
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

        /// <summary>
        /// Cập nhật thông tin hồ sơ cá nhân của người dùng hiện tại
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

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

        /// <summary>
        /// Tải lên và cập nhật ảnh đại diện mới cho người dùng hiện tại
        /// </summary>
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

        /// <summary>
        /// Đổi mật khẩu cho người dùng hiện tại
        /// </summary>
        [HttpPut("me/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

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
}
