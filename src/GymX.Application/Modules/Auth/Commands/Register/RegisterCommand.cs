using GymX.Application.Modules.Auth.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.Register;

public record RegisterCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password) : IRequest<Result<RegisterResponse>>
{
    public static RegisterCommand FromRequest(RegisterRequest req) => new(
        req.FullName.Trim(),
        req.Email.Trim().ToLowerInvariant(),
        req.PhoneNumber.Trim(),
        req.Password);
}
