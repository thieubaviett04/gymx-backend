using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Violations.Commands.CancelViolation;

public record CancelViolationCommand(Guid ViolationId, string Reason, Guid CancelledBy) : IRequest<Result>;

