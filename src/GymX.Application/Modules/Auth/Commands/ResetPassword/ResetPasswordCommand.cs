using GymX.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymX.Application.Modules.Auth.Commands.ResetPassword
{
    public record ResetPasswordCommand(
        string Token,
        string NewPassword) : IRequest<Result<ResetPasswordResponse>>;

    public record ResetPasswordResponse
    {
        public string Email { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }

}
