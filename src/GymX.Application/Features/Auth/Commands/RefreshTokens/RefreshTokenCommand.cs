using GymX.Application.Common.Models.Authentication;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Features.Auth.Commands.RefreshTokens;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthTokenDto>>;
