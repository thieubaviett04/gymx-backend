using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Payroll;
using MediatR;

namespace GymX.Application.Modules.Violations.Commands.CancelViolation;

public class CancelViolationCommandHandler(
    IViolationRepository violationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelViolationCommand, Result>
{
    public async Task<Result> Handle(CancelViolationCommand command, CancellationToken cancellationToken)
    {
        var violation = await violationRepository.GetByIdAsync(command.ViolationId, cancellationToken);
        if (violation == null)
        {
            return Result.Failure(
                Error.NotFound("Violation.NotFound", "Biên bản vi phạm không tồn tại trong hệ thống."));
        }

        if (violation.Status == ViolationStatus.Cancelled)
        {
            return Result.Failure(
                Error.Conflict("Violation.AlreadyCancelled", "Biên bản vi phạm này đã bị hủy trước đó."));
        }

        var isPayrollPeriodClosed = await violationRepository.IsDateInClosedPayrollPeriodAsync(violation.OccurredAt, cancellationToken);
        if (isPayrollPeriodClosed)
        {
            return Result.Failure(
                Error.Conflict("Violation.PayrollPeriodClosed", "Thời gian vi phạm thuộc kỳ tính lương đã được chốt và duyệt. Không thể xóa biên bản này."));
        }

        var reason = string.IsNullOrWhiteSpace(command.Reason)
            ? "Hủy bởi quản trị viên"
            : command.Reason.Trim();

        violation.Cancel(reason, command.CancelledBy);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
