using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Features.Users.Commands.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
) : IRequest<Result<string>>;
