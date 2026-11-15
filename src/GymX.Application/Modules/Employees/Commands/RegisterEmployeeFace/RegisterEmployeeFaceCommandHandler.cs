using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Identity;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.RegisterEmployeeFace;

public class RegisterEmployeeFaceCommandHandler(
    IEmployeeRepository employeeRepository,
    IUserRepository userRepository,
    IFaceDataRepository faceDataRepository,
    IFaceRecognitionService faceRecognitionService,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterEmployeeFaceCommand, Result<EmployeeFaceDetailDto>>
{
    public async Task<Result<EmployeeFaceDetailDto>> Handle(
        RegisterEmployeeFaceCommand command,
        CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(command.EmployeeId, cancellationToken);
        if (employee is null || employee.IsDeleted)
        {
            return Result.Failure<EmployeeFaceDetailDto>(EmployeeFaceErrors.EmployeeNotFound);
        }

        var user = await userRepository.GetByIdAsync(employee.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<EmployeeFaceDetailDto>(EmployeeFaceErrors.EmployeeNotFound);
        }

        var faceResult = await faceRecognitionService.RegisterFaceAsync(command.ImageBytes, cancellationToken);
        if (faceResult.IsFailure)
        {
            return Result.Failure<EmployeeFaceDetailDto>(faceResult.Error);
        }

        var faceToken = faceResult.Value;

        var otherActiveFaces = (await faceDataRepository.GetAllAsync(cancellationToken))
            .Where(f => f.Status == "ACTIVE" && f.UserId != employee.UserId && !string.IsNullOrWhiteSpace(f.ExternalFaceId))
            .ToList();

        foreach (var otherFace in otherActiveFaces)
        {
            var compareResult = await faceRecognitionService.CompareFacesAsync(faceToken, otherFace.ExternalFaceId!, cancellationToken);
            if (compareResult.IsSuccess && compareResult.Value >= 75.0)
            {
                var conflictingEmployee = await employeeRepository.GetByUserIdAsync(otherFace.UserId, cancellationToken);
                var conflictingUser = await userRepository.GetByIdAsync(otherFace.UserId, cancellationToken);
                var empInfo = conflictingEmployee is not null
                    ? $"{conflictingEmployee.EmployeeCode} - {conflictingUser?.FullName ?? ""}"
                    : conflictingUser?.FullName ?? "khác";

                return Result.Failure<EmployeeFaceDetailDto>(Error.Conflict(
                    "FaceRecognition.FaceAlreadyEnrolledToAnother",
                    $"Khuôn mặt đã đăng ký cho {empInfo}."));
            }
        }

        Guid? enrolledBy = Guid.TryParse(currentUserService.UserId, out var adminId) ? adminId : null;

        var faceData = await faceDataRepository.GetByUserIdAsync(employee.UserId, cancellationToken);
        if (faceData is not null)
        {
            faceData.Reenroll(faceToken, faceToken, enrolledBy);
            faceDataRepository.Update(faceData);
        }
        else
        {
            faceData = FaceData.Create(employee.UserId, faceToken, faceToken, enrolledBy);
            await faceDataRepository.AddAsync(faceData, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new EmployeeFaceDetailDto
        {
            EmployeeId = employee.Id,
            UserId = employee.UserId,
            EmployeeCode = employee.EmployeeCode,
            FullName = user.FullName,
            Position = employee.Position,
            HasFaceEnrolled = true,
            Status = faceData.Status,
            Provider = faceData.Provider,
            ExternalFaceId = faceData.ExternalFaceId,
            EnrolledAt = faceData.EnrolledAt,
            EnrolledBy = faceData.EnrolledBy
        };

        return Result.Success(dto);
    }
}
