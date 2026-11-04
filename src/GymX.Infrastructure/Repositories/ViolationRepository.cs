using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Violations.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Entities.Payroll;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class ViolationRepository(ApplicationDbContext context)
    : GenericRepository<Violation>(context), IViolationRepository
{
    public async Task<string> GenerateNextViolationCodeAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var codePrefix = $"VP-{date:yyyyMMdd}-";

        var latestViolationCode = await Context.Violations
            .AsNoTracking()
            .Where(violation => violation.ViolationCode.StartsWith(codePrefix))
            .OrderByDescending(violation => violation.ViolationCode)
            .Select(violation => violation.ViolationCode)
            .FirstOrDefaultAsync(cancellationToken);

        var maxSequenceNumber = 0;
        if (latestViolationCode != null)
        {
            var codeParts = latestViolationCode.Split('-');
            if (codeParts.Length == 3 && int.TryParse(codeParts[2], out var sequence))
            {
                maxSequenceNumber = sequence;
            }
        }

        return $"{codePrefix}{(maxSequenceNumber + 1):D4}";
    }

    public async Task<bool> ExistsCodeAsync(string violationCode, CancellationToken cancellationToken = default)
    {
        var normalizedCode = violationCode.Trim().ToUpperInvariant();
        return await Context.Violations.AnyAsync(
            violation => violation.ViolationCode == normalizedCode, 
            cancellationToken);
    }

    public async Task<bool> IsDateInClosedPayrollPeriodAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var targetDate = DateOnly.FromDateTime(date);
        return await Context.PayrollPeriods.AnyAsync(period =>
            period.PeriodStart <= targetDate &&
            period.PeriodEnd >= targetDate &&
            period.Status == "CONFIRMED",
            cancellationToken);
    }

    public async Task<PagedResult<ViolationSummaryDto>> GetViolationsPagedAsync(
        string? search,
        Guid? employeeId,
        Guid? penaltyRuleId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        string? sortBy,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var violationQuery = from violation in Context.Violations.AsNoTracking()
                             join employee in Context.Employees.AsNoTracking() on violation.EmployeeId equals employee.Id
                             join user in Context.Users.AsNoTracking() on employee.UserId equals user.Id
                             join penaltyRule in Context.PenaltyRules.AsNoTracking() on violation.PenaltyRuleId equals penaltyRule.Id
                             select new
                             {
                                 Violation = violation,
                                 Employee = employee,
                                 User = user,
                                 PenaltyRule = penaltyRule
                             };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchKeyword = search.Trim();
            violationQuery = violationQuery.Where(item =>
                EF.Functions.ILike(item.Violation.ViolationCode, $"%{searchKeyword}%") ||
                EF.Functions.ILike(item.Employee.EmployeeCode, $"%{searchKeyword}%") ||
                EF.Functions.ILike(item.User.FullName, $"%{searchKeyword}%") ||
                EF.Functions.ILike(item.PenaltyRule.Name, $"%{searchKeyword}%"));
        }

        if (employeeId.HasValue && employeeId.Value != Guid.Empty)
        {
            violationQuery = violationQuery.Where(item => item.Violation.EmployeeId == employeeId.Value);
        }

        if (penaltyRuleId.HasValue && penaltyRuleId.Value != Guid.Empty)
        {
            violationQuery = violationQuery.Where(item => item.Violation.PenaltyRuleId == penaltyRuleId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            violationQuery = violationQuery.Where(item => item.Violation.Status == normalizedStatus);
        }

        if (fromDate.HasValue)
        {
            var fromDateUtc = fromDate.Value.ToUniversalTime();
            violationQuery = violationQuery.Where(item => item.Violation.OccurredAt >= fromDateUtc);
        }

        if (toDate.HasValue)
        {
            var toDateUtc = toDate.Value.ToUniversalTime();
            violationQuery = violationQuery.Where(item => item.Violation.OccurredAt <= toDateUtc);
        }

        violationQuery = sortBy?.ToLowerInvariant() switch
        {
            "occurredat_asc" => violationQuery.OrderBy(item => item.Violation.OccurredAt),
            "fineamount_desc" => violationQuery.OrderByDescending(item => item.Violation.FineAmount),
            "fineamount_asc" => violationQuery.OrderBy(item => item.Violation.FineAmount),
            "createddate_asc" => violationQuery.OrderBy(item => item.Violation.CreatedDate),
            _ => violationQuery.OrderByDescending(item => item.Violation.OccurredAt).ThenByDescending(item => item.Violation.CreatedDate)
        };

        var totalRecords = await violationQuery.CountAsync(cancellationToken);

        var violationSummaries = await violationQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new ViolationSummaryDto(
                item.Violation.Id,
                item.Violation.ViolationCode,
                item.Violation.EmployeeId,
                item.Employee.EmployeeCode,
                item.User.FullName,
                item.Employee.Position,
                item.Violation.PenaltyRuleId,
                item.PenaltyRule.RuleCode,
                item.PenaltyRule.Name,
                item.Violation.FineAmount,
                item.Violation.OccurredAt,
                item.Violation.Status,
                item.Violation.EvidenceUrl,
                item.Violation.CreatedDate
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<ViolationSummaryDto>(violationSummaries, totalRecords, pageIndex, pageSize);
    }

    public async Task<ViolationDetailDto?> GetViolationDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var violationRecord = await (from violation in Context.Violations.AsNoTracking()
                                     join employee in Context.Employees.AsNoTracking() on violation.EmployeeId equals employee.Id
                                     join user in Context.Users.AsNoTracking() on employee.UserId equals user.Id
                                     join penaltyRule in Context.PenaltyRules.AsNoTracking() on violation.PenaltyRuleId equals penaltyRule.Id
                                     join recorder in Context.Users.AsNoTracking() on violation.RecordedBy equals recorder.Id into recorders
                                     from recorder in recorders.DefaultIfEmpty()
                                     join canceller in Context.Users.AsNoTracking() on violation.CancelledBy equals canceller.Id into cancellers
                                     from canceller in cancellers.DefaultIfEmpty()
                                     where violation.Id == id
                                     select new
                                     {
                                         Violation = violation,
                                         Employee = employee,
                                         User = user,
                                         PenaltyRule = penaltyRule,
                                         RecordedByName = recorder != null ? recorder.FullName : "Hệ thống",
                                         CancelledByName = canceller != null ? canceller.FullName : null
                                     }).FirstOrDefaultAsync(cancellationToken);

        if (violationRecord == null) return null;

        return new ViolationDetailDto(
            violationRecord.Violation.Id,
            violationRecord.Violation.ViolationCode,
            violationRecord.Violation.EmployeeId,
            violationRecord.Employee.EmployeeCode,
            violationRecord.User.FullName,
            violationRecord.Employee.Position,
            violationRecord.Violation.PenaltyRuleId,
            violationRecord.PenaltyRule.RuleCode,
            violationRecord.PenaltyRule.Name,
            violationRecord.PenaltyRule.ViolationType,
            violationRecord.Violation.FineAmount,
            violationRecord.Violation.OccurredAt,
            violationRecord.Violation.EvidenceUrl,
            violationRecord.Violation.Note,
            violationRecord.Violation.Status,
            violationRecord.Violation.CancellationReason,
            violationRecord.Violation.CancelledAt,
            violationRecord.CancelledByName,
            violationRecord.Violation.RecordedBy,
            violationRecord.RecordedByName,
            violationRecord.Violation.AttendanceRecordId,
            violationRecord.Violation.CreatedDate
        );
    }

    public async Task<List<EmployeeLookupDto>> GetEmployeesForViolationLookupAsync(string? search, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var employeeQuery = from employee in Context.Employees.AsNoTracking()
                             join user in Context.Users.AsNoTracking() on employee.UserId equals user.Id
                             where !employee.IsDeleted && employee.EmploymentStatus == EmployeeStatus.Active
                             select new
                             {
                                 employee.Id,
                                 employee.EmployeeCode,
                                 user.FullName,
                                 employee.Position,
                                 user.PhoneNumber,
                                 HasShiftToday = Context.EmployeeWorkSchedules.Any(schedule =>
                                     schedule.EmployeeId == employee.Id &&
                                     schedule.WorkDate == today &&
                                     schedule.Status != "CANCELLED")
                             };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchKeyword = search.Trim();
            employeeQuery = employeeQuery.Where(item =>
                EF.Functions.ILike(item.EmployeeCode, $"%{searchKeyword}%") ||
                EF.Functions.ILike(item.FullName, $"%{searchKeyword}%") ||
                EF.Functions.ILike(item.PhoneNumber ?? "", $"%{searchKeyword}%"));
        }

        return await employeeQuery
            .OrderByDescending(item => item.HasShiftToday)
            .ThenBy(item => item.FullName)
            .Take(50)
            .Select(item => new EmployeeLookupDto(
                item.Id,
                item.EmployeeCode,
                item.FullName,
                item.Position,
                item.PhoneNumber,
                item.HasShiftToday
            ))
            .ToListAsync(cancellationToken);
    }
}
