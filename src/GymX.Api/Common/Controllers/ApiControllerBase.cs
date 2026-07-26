using GymX.Domain.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Common.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        private ISender? _sender;
        protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);
            return MapErrorToResponse(result.Error);
        }
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return Ok();
            return MapErrorToResponse(result.Error);
        }
        private IActionResult MapErrorToResponse(Error error)
        {
            var response = new
            {
                code = error.Code,
                messageKey = error.ResolvedMessageKey,
                description = error.Description
            };
            return error.Type switch
            {
                ErrorType.NotFound => NotFound(response),
                ErrorType.Validation => BadRequest(response),
                ErrorType.Unauthorized => Unauthorized(response),
                ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, response),
                ErrorType.Conflict => Conflict(response),
                _ => BadRequest(response)
            };
        }
    }
}
