using GymX.Application.Common.Models.Authentication;
using GymX.Application.Modules.Auth.DTOs.Request;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthTokenDto>>
{
    public static LoginCommand FromRequest(LoginRequest req) => new(
        req.Email.Trim().ToLowerInvariant(),
        req.Password);
}
