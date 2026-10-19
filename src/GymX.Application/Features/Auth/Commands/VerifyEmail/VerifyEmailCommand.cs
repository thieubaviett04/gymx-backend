using GymX.Application.Common.Models.Authentication;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Features.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(string Email, string OtpCode) : IRequest<Result<AuthTokenDto>>;
