using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.RegisterEmployeeFace;

public record RegisterEmployeeFaceCommand(
    Guid EmployeeId,
    byte[] ImageBytes,
    string FileName,
    string ContentType,
    long FileSize
) : IRequest<Result<EmployeeFaceDetailDto>>;
