using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Common.Templates;
using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Entities.Payroll;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Kpis.Commands.AssignEmployeeKpis;

public sealed class AssignEmployeeKpisCommandHandler(
    IKpiTemplateRepository templateRepository,
    IEmployeeKpiRepository employeeKpiRepository,
    IEmployeeRepository employeeRepository,
    IUserRepository userRepository,
    INotificationService notificationService,
    IEmailService emailService,
    IUnitOfWork unitOfWork) : IRequestHandler<AssignEmployeeKpisCommand, Result<AssignKpiResultDto>>
{
    public async Task<Result<AssignKpiResultDto>> Handle(AssignEmployeeKpisCommand command, CancellationToken cancellationToken)
    {
        var template = await templateRepository.GetByIdAsync(command.KpiTemplateId, cancellationToken);
        if (template == null || template.IsDeleted)
        {
            return Result.Failure<AssignKpiResultDto>(KpiErrors.TemplateNotFound);
        }

        var distinctEmployeeIds = command.EmployeeIds.Distinct().ToList();
        if (distinctEmployeeIds.Count == 0)
        {
            return Result.Failure<AssignKpiResultDto>(KpiErrors.EmptyEmployees);
        }

        var existingAssignedIds = await employeeKpiRepository.GetAssignedEmployeeIdsAsync(
            distinctEmployeeIds, command.Month, command.Year, cancellationToken);

        var validEmployeeIds = distinctEmployeeIds.Except(existingAssignedIds).ToList();

        if (validEmployeeIds.Count == 0)
        {
            return Result.Success(new AssignKpiResultDto(0, existingAssignedIds.Count, "Tất cả nhân viên đã có KPI trong chu kỳ này."));
        }

        var validEmployees = await employeeRepository.GetActiveByIdsAsync(validEmployeeIds, cancellationToken);

        var matchedEmployees = validEmployees.Where(e =>
            string.Equals(e.Position, template.TargetRole, StringComparison.OrdinalIgnoreCase)).ToList();

        if (matchedEmployees.Count == 0)
        {
            return Result.Failure<AssignKpiResultDto>(KpiErrors.RoleMismatch);
        }

        var newKpis = matchedEmployees.Select(e => EmployeeKpi.Create(
            e.Id,
            template.Id,
            command.Month,
            command.Year,
            template.TargetRevenue,
            template.TargetBonusAmount,
            command.CurrentUserId,
            template.TargetPtSessions,
            template.TargetNewMembers,
            command.Note
        )).ToList();

        try
        {
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await employeeKpiRepository.AddRangeAsync(newKpis, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var recipientUserIds = matchedEmployees.Select(e => e.UserId).Distinct().ToList();
                var notifyTitle = $"🎯 Giao chỉ tiêu KPI Tháng {command.Month}/{command.Year}";
                var notifyContent = $"Mẫu áp dụng: {template.Name}. Chỉ tiêu: {template.TargetPtSessions?.ToString() ?? "0"} buổi PT, {template.TargetRevenue:N0} ₫ doanh số. Thưởng đạt mốc: {template.TargetBonusAmount:N0} ₫.";

                await notificationService.SendBatchNotificationsAsync(
                    recipientUserIds,
                    notifyTitle,
                    notifyContent,
                    NotificationType.System,
                    cancellationToken);
            }, cancellationToken);

            _ = Task.Run(async () =>
            {
                foreach (var emp in matchedEmployees)
                {
                    try
                    {
                        var user = await userRepository.GetByIdAsync(emp.UserId);
                        if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                        {
                            var emailSubject = $"[GymFitness] Thông báo Giao chỉ tiêu KPI Tháng {command.Month}/{command.Year}";
                            var emailHtml = EmailTemplates.BuildKpiAssignmentEmail(
                                user.FullName,
                                command.Month,
                                command.Year,
                                template.Name,
                                template.TargetPtSessions,
                                template.TargetRevenue,
                                template.TargetBonusAmount,
                                command.Note);

                            await emailService.SendEmailAsync(user.Email, emailSubject, emailHtml);
                        }
                    }
                    catch
                    {
                    }
                }
            });

            return Result.Success(new AssignKpiResultDto(
                matchedEmployees.Count,
                existingAssignedIds.Count,
                $"Giao KPI thành công cho {matchedEmployees.Count} nhân viên. Bỏ qua {existingAssignedIds.Count} nhân viên đã có phiếu trước đó."));
        }
        catch
        {
            return Result.Failure<AssignKpiResultDto>(KpiErrors.AssignFailed);
        }
    }
}

