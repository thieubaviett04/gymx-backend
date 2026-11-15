using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.RemoveEmployeeFace;

public record RemoveEmployeeFaceCommand(Guid EmployeeId) : IRequest<Result<bool>>;
