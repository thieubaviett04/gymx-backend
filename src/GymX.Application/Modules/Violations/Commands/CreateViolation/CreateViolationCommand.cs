using GymX.Application.Modules.Violations.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Violations.Commands.CreateViolation;

public record CreateViolationCommand(
    Guid EmployeeId,
    Guid PenaltyRuleId,
    DateTime OccurredAt,
    Guid RecordedBy,
    Guid? AttendanceRecordId = null,
    string? EvidenceUrl = null,
    string? Note = null) : IRequest<Result<ViolationDetailDto>>;

