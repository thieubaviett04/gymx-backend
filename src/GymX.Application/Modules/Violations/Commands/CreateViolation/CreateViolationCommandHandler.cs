using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Violations.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
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

        if (penaltyRule.Status != PenaltyRuleStatus.Applied)
        {
            return Result.Failure<ViolationDetailDto>(
                Error.Validation("Violation.PenaltyRuleNotActive", "Quy định phạt này đã ngừng áp dụng hoặc chưa được kích hoạt."));
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
            catch (Exception exception) when (IsUniqueConstraintViolation(exception) && attempt < MaxRetryCount)
            {
            }
        }

        return Result.Failure<ViolationDetailDto>(
            Error.Conflict("Violation.CodeGenerationFailed", "Không thể sinh mã biên bản vi phạm do xung đột dữ liệu. Vui lòng thử lại."));
    }

    private static bool IsUniqueConstraintViolation(Exception exception)
    {
        var exceptionMessage = exception.ToString();
        return exceptionMessage.Contains("23505") ||
               exceptionMessage.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
               exceptionMessage.Contains("unique constraint", StringComparison.OrdinalIgnoreCase);
    }
}
