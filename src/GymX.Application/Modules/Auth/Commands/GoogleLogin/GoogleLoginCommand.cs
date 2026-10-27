using GymX.Application.Common.Models.Authentication;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.GoogleLogin;

public record GoogleLoginCommand(string IdToken) : IRequest<Result<AuthTokenDto>>;
