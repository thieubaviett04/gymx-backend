using GymX.Application.Features.Auth.Commands.GoogleLogin;
using GymX.Application.Common.Interfaces;
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
            {
                return Ok(result.Value);
            }

            return result.Error.Code switch
            {
                "Auth.GoogleLogin" => Unauthorized(new {error = result.Error}),
                "Auth.EmailExists" => Conflict(new {error = result.Error}),
                _ => BadRequest(new {error = result.Error})
            };
        }
    }
}
