using GymX.Application.Features.Auth.Commands.GoogleLogin;
using GymX.Application.Features.Auth.Commands.Register;
using GymX.Application.Features.Auth.Commands.VerifyEmail;
using MediatR;
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

            if (result.IsSuccess)
                return Ok(result.Value);

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
                return Ok(result.Value); // Value is RegisterResponse

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

            if (result.IsSuccess)
                return Ok(new { message = result.Value });

            return result.Error.Code switch
            {
                "Auth.InvalidOtp" => BadRequest(new { error = result.Error }),
                "Auth.AlreadyVerified" => Conflict(new { error = result.Error }),
                "Auth.UserNotFound" => NotFound(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }
    }
}
