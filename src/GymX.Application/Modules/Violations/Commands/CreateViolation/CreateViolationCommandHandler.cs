using GymX.Application.Common.Exceptions;
using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Violations.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Identity;
using GymX.Domain.Entities.Payroll;
using MediatR;

namespace GymX.Application.Modules.Violations.Commands.CreateViolation;

public class CreateViolationCommandHandler(
    IViolationRepository violationRepository,
    IPenaltyRuleRepository penaltyRuleRepository,
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateViolationCommand, Result<ViolationDetailDto>>
{
    private const int MaxRetryCount = 3;

    public async Task<Result<ViolationDetailDto>> Handle(CreateViolationCommand command, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(command.EmployeeId, cancellationToken);
        if (employee == null || employee.IsDeleted)
        {
            return Result.Failure<ViolationDetailDto>(
                Error.NotFound("Violation.EmployeeNotFound", "Nhân viên được chọn không tồn tại trong hệ thống."));
        }

        if (employee.EmploymentStatus != EmployeeStatus.Active)
        {
            return Result.Failure<ViolationDetailDto>(
                Error.Validation("Violation.EmployeeNotActive", "Nhân viên đã nghỉ việc hoặc không ở trạng thái hoạt động."));
        }

        var penaltyRule = await penaltyRuleRepository.GetByIdAsync(command.PenaltyRuleId, cancellationToken);
        if (penaltyRule == null || penaltyRule.IsDeleted)
        {
            return Result.Failure<ViolationDetailDto>(
                Error.NotFound("Violation.PenaltyRuleNotFound", "Quy định phạt không tồn tại trong hệ thống."));
        }

        var todayVn = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
        var isRuleActive = penaltyRule.Status == PenaltyRuleStatus.Active ||
                           (penaltyRule.Status == PenaltyRuleStatus.Pending && penaltyRule.EffectiveDate <= todayVn);

        if (!isRuleActive)
        {
            return Result.Failure<ViolationDetailDto>(
                Error.Validation("Violation.PenaltyRuleNotActive", "Quy định phạt này đã ngừng áp dụng hoặc chưa được kích hoạt."));
        }

        var occurredAtVn = command.OccurredAt.Kind == DateTimeKind.Utc
            ? command.OccurredAt.AddHours(7)
            : command.OccurredAt;
        var occurredDate = DateOnly.FromDateTime(occurredAtVn);

        if (occurredDate < penaltyRule.EffectiveDate)
        {
            return Result.Failure<ViolationDetailDto>(
                Error.Validation("Violation.OccurredBeforeRuleEffectiveDate", "Thời gian vi phạm không được xảy ra trước ngày quy định bắt đầu có hiệu lực."));
        }

        var isTrainer = string.Equals(employee.Position, RoleCode.Trainer, StringComparison.OrdinalIgnoreCase);

        if (string.Equals(penaltyRule.ViolationType, RuleTargetAudience.Member, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<ViolationDetailDto>(
                Error.Validation("Violation.TargetAudienceMismatch", "Quy định này chỉ áp dụng cho hội viên, không áp dụng cho nhân viên."));
        }

        if (isTrainer && string.Equals(penaltyRule.ViolationType, RuleTargetAudience.Staff, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<ViolationDetailDto>(
                Error.Validation("Violation.TargetAudienceMismatch", "Quy định phạt này chỉ áp dụng cho nhân viên lễ tân, không áp dụng cho huấn luyện viên."));
        }

        if (!isTrainer && string.Equals(penaltyRule.ViolationType, RuleTargetAudience.Trainer, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<ViolationDetailDto>(
                Error.Validation("Violation.TargetAudienceMismatch", "Quy định phạt này chỉ áp dụng cho huấn luyện viên, không áp dụng cho nhân viên lễ tân."));
        }

        var isPayrollPeriodClosed = await violationRepository.IsDateInClosedPayrollPeriodAsync(command.OccurredAt, cancellationToken);
        if (isPayrollPeriodClosed)
        {
            return Result.Failure<ViolationDetailDto>(
                Error.Conflict("Violation.PayrollPeriodClosed", "Thời gian vi phạm thuộc kỳ tính lương đã được chốt và duyệt. Không thể tạo biên bản cho kỳ này."));
        }

        for (var attempt = 1; attempt <= MaxRetryCount; attempt++)
        {
            var generatedViolationCode = await violationRepository.GenerateNextViolationCodeAsync(command.OccurredAt, cancellationToken);

            var newViolation = Violation.Create(
                generatedViolationCode,
                command.EmployeeId,
                command.PenaltyRuleId,
                penaltyRule.FineAmount,
                command.OccurredAt,
                command.RecordedBy,
                command.AttendanceRecordId,
                command.EvidenceUrl,
                command.Note
            );

            await violationRepository.AddAsync(newViolation, cancellationToken);

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                var createdViolationDetail = await violationRepository.GetViolationDetailByIdAsync(newViolation.Id, cancellationToken);
                return Result.Success(createdViolationDetail!);
            }
            catch (UniqueConstraintException) when (attempt < MaxRetryCount)
            {
            }
        }

        return Result.Failure<ViolationDetailDto>(
            Error.Conflict("Violation.CodeGenerationFailed", "Không thể sinh mã biên bản vi phạm do xung đột dữ liệu. Vui lòng thử lại."));
    }
}
