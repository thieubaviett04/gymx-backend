using GymX.Domain.Common.Models;
using GymX.Application.Common.Models.Authentication;
using MediatR;

namespace GymX.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<AuthTokenDto>>;
}
