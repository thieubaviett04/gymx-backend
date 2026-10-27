using System.Security.Claims;
using GymX.Application.Modules.Auth.Commands.ForgotPassword;
using GymX.Application.Modules.Auth.Commands.ResetPassword;
using GymX.Application.Modules.Auth.Commands.GoogleLogin;
using GymX.Application.Modules.Auth.Commands.Login;
using GymX.Application.Modules.Auth.Commands.Register;
using GymX.Application.Modules.Auth.Commands.VerifyEmail;
using GymX.Application.Modules.Auth.Commands.VerifyOTP;
using GymX.Application.Modules.Auth.Commands.RefreshTokens;
using GymX.Application.Modules.Auth.Commands.Logout;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.AuthController
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController(ISender sender) : ControllerBase
    {
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess) return Ok(new { success = true, data = result.Value });

            return result.Error.Code switch
            {
                "Auth.GoogleLogin" => Unauthorized(new { error = result.Error }),
                "Auth.EmailExists" => Conflict(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return result.Error.Code switch
            {
                "Auth.EmailExists" => Conflict(new { error = result.Error }),
                "Auth.PhoneExists" => Conflict(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            if (result.IsSuccess) return Ok(new { success = true, data = result.Value });

            return result.Error.Code switch
            {
                "Auth.InvalidOtp" => BadRequest(new { error = result.Error }),
                "Auth.EmailExists" => Conflict(new { error = result.Error }),
                "Auth.PhoneExists" => Conflict(new { error = result.Error }),
                "Auth.RoleNotFound" => NotFound(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            if (result.IsSuccess) return Ok(new { success = true, data = result.Value });

            return result.Error.Code switch
            {
                "Auth.InvalidCredentials" => Unauthorized(new { error = result.Error }),
                "Auth.UnverifiedEmail" => StatusCode(403, new { error = result.Error }),
                "Auth.AccountLocked" => StatusCode(403, new { error = result.Error }),
                "Auth.AccountInactive" => StatusCode(403, new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            if (result.IsSuccess) return Ok(new { message = result.Value });

            return BadRequest(new { error = result.Error });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            if (result.IsSuccess) return Ok(result.Value);

            return result.Error.Code switch
            {
                "Auth.InvalidToken" => BadRequest(new { error = result.Error }),
                "Auth.UserNotFound" => NotFound(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            if (result.IsSuccess) return Ok(result.Value);

            return BadRequest(new { error = result.Error });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            if (result.IsSuccess) return Ok(new { success = true, data = result.Value });

            return result.Error.Code switch
            {
                "Auth.InvalidRefreshToken" => Unauthorized(new { error = result.Error }),
                "Auth.CompromisedToken" => Unauthorized(new { error = result.Error }),
                "Auth.ExpiredToken" => Unauthorized(new { error = result.Error }),
                "Auth.UnverifiedEmail" => StatusCode(403, new { error = result.Error }),
                "Auth.AccountLocked" => StatusCode(403, new { error = result.Error }),
                "Auth.AccountInactive" => StatusCode(403, new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            if (result.IsSuccess) return Ok(new { message = result.Value });

            return BadRequest(new { error = result.Error });
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var fullName = User.FindFirst("fullName")?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            return Ok(new
            {
                userId,
                email,
                fullName,
                roles
            });
        }

        [Authorize(Policy = PolicyNames.RequireAdmin)]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok(new { message = "Xin chào Admin! Bạn có toàn quyền truy cập hệ thống GymX." });
        }
    }
}
