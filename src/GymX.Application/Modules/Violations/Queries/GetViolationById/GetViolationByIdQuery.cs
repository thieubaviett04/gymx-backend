using GymX.Application.Modules.Violations.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Violations.Queries.GetViolationById;

public record GetViolationByIdQuery(Guid Id) : IRequest<Result<ViolationDetailDto>>;

