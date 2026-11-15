using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Identity;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.RemoveEmployeeFace;

public class RemoveEmployeeFaceCommandHandler(
    IEmployeeRepository employeeRepository,
    IFaceDataRepository faceDataRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveEmployeeFaceCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        RemoveEmployeeFaceCommand command,
        CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(command.EmployeeId, cancellationToken);
        if (employee is null || employee.IsDeleted)
        {
            return Result.Failure<bool>(EmployeeFaceErrors.EmployeeNotFound);
        }

        var faceData = await faceDataRepository.GetByUserIdAsync(employee.UserId, cancellationToken);
        if (faceData is null || faceData.Status == FaceDataStatus.Inactive)
        {
            return Result.Failure<bool>(EmployeeFaceErrors.FaceDataNotFound);
        }

        faceData.Deactivate();
        faceDataRepository.Update(faceData);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
