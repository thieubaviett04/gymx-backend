using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(Guid EmployeeId) : IRequest<Result<bool>>;
