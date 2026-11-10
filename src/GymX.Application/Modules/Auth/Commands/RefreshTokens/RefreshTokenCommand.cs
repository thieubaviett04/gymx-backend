using GymX.Application.Common.Models.Authentication;
using GymX.Application.Modules.Auth.DTOs.Request;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.RefreshTokens;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthTokenDto>>
{
    public static RefreshTokenCommand FromRequest(RefreshTokenRequest req) => new(req.RefreshToken.Trim());
}
