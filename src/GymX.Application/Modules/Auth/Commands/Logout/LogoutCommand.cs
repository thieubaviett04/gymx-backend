using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.Logout
{
   public record LogoutCommand (string RefreshToken) : IRequest<Result<string>>;
}
