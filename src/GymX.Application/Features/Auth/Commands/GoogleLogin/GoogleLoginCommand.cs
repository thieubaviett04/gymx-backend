using GymX.Application.Common.Models.Authentication;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Features.Auth.Commands.GoogleLogin;

public record GoogleLoginCommand(string IdToken) : IRequest<Result<AuthTokenDto>>;
