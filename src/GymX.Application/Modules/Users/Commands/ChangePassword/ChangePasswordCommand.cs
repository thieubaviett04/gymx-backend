using GymX.Application.Modules.Users.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Users.Commands.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
) : IRequest<Result<string>>
{
    public static ChangePasswordCommand FromRequest(ChangePasswordRequest req) => new(
        req.CurrentPassword,
        req.NewPassword,
        req.ConfirmPassword);
}
