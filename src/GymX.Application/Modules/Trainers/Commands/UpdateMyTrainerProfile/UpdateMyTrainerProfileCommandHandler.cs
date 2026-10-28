using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Trainers.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Trainers.Commands.UpdateMyTrainerProfile;

public class UpdateMyTrainerProfileCommandHandler(
    ICurrentUserService currentUserService,
    IEmployeeRepository employeeRepository,
    ITrainerRepository trainerRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateMyTrainerProfileCommand, Result<TrainerProfileDto>>
{
    public async Task<Result<TrainerProfileDto>> Handle(UpdateMyTrainerProfileCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Result.Failure<TrainerProfileDto>(Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để thực hiện thao tác này."));
        }

        var employee = await employeeRepository.GetByUserIdAsync(userId, cancellationToken);
        if (employee is null)
        {
            return Result.Failure<TrainerProfileDto>(Error.NotFound("Employee.NotFound", "Không tìm thấy hồ sơ nhân viên của bạn."));
        }

        var trainer = await trainerRepository.GetByEmployeeIdAsync(employee.Id, cancellationToken);
        if (trainer is null)
        {
            return Result.Failure<TrainerProfileDto>(Error.NotFound("Trainer.NotFound", "Không tìm thấy hồ sơ huấn luyện viên của bạn."));
        }

        if (!string.IsNullOrWhiteSpace(request.NationalId))
        {
            var trimmedNationalId = request.NationalId.Trim();
            var existingWithNationalId = await employeeRepository.GetByNationalIdAsync(trimmedNationalId, cancellationToken);
            if (existingWithNationalId is not null && existingWithNationalId.Id != employee.Id)
            {
                return Result.Failure<TrainerProfileDto>(Error.Conflict("Employee.NationalIdExists", "Số CCCD/CMND này đã được sử dụng bởi một nhân viên khác."));
            }

            employee.UpdateNationalId(trimmedNationalId);
            employeeRepository.Update(employee);
        }

        if (request.Description is not null)
        {
            trainer.UpdateDescription(request.Description.Trim());
            trainerRepository.Update(trainer);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var fullProfile = await userRepository.GetFullProfileAsync(userId, cancellationToken);
        if (fullProfile?.TrainerInfo is null)
        {
            return Result.Failure<TrainerProfileDto>(Error.NotFound("Trainer.NotFound", "Không thể tải lại hồ sơ huấn luyện viên."));
        }

        return Result.Success(fullProfile.TrainerInfo);
    }
}
