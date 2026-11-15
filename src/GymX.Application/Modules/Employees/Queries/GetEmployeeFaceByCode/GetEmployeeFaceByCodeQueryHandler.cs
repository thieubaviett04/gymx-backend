using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Identity;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Employees.Queries.GetEmployeeFaceByCode;

public class GetEmployeeFaceByCodeQueryHandler(
    IEmployeeRepository employeeRepository,
    IUserRepository userRepository,
    IFaceDataRepository faceDataRepository)
    : IRequestHandler<GetEmployeeFaceByCodeQuery, Result<EmployeeFaceDetailDto>>
{
    public async Task<Result<EmployeeFaceDetailDto>> Handle(
        GetEmployeeFaceByCodeQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.EmployeeCode))
        {
            return Result.Failure<EmployeeFaceDetailDto>(EmployeeFaceErrors.EmployeeNotFound);
        }

        var normalizedCode = request.EmployeeCode.Trim().ToUpperInvariant();
        var employee = await employeeRepository.GetByEmployeeCodeAsync(normalizedCode, cancellationToken);
        if (employee is null || employee.IsDeleted)
        {
            return Result.Failure<EmployeeFaceDetailDto>(EmployeeFaceErrors.EmployeeNotFound);
        }

        var user = await userRepository.GetByIdAsync(employee.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<EmployeeFaceDetailDto>(EmployeeFaceErrors.EmployeeNotFound);
        }

        var faceData = await faceDataRepository.GetByUserIdAsync(employee.UserId, cancellationToken);
        var hasFace = faceData is not null && faceData.Status == FaceDataStatus.Active;

        var dto = new EmployeeFaceDetailDto
        {
            EmployeeId = employee.Id,
            UserId = employee.UserId,
            EmployeeCode = employee.EmployeeCode,
            FullName = user.FullName,
            Position = employee.Position,
            HasFaceEnrolled = hasFace,
            Status = faceData?.Status ?? "NOT_ENROLLED",
            Provider = faceData?.Provider ?? FaceDataProvider.FacePlusPlus,
            ExternalFaceId = faceData?.ExternalFaceId,
            EnrolledAt = faceData?.EnrolledAt,
            EnrolledBy = faceData?.EnrolledBy
        };

        return Result.Success(dto);
    }
}

