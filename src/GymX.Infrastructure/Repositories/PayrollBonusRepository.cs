using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PayrollBonuses.DTOs;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Payroll;
using GymX.Domain.Entities.Payroll;
using GymX.Domain.Errors;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class PayrollBonusRepository(ApplicationDbContext context)
    : GenericRepository<PayrollBonus>(context), IPayrollBonusRepository
{
    public async Task<PayrollBonusPagedResultDto> GetBonusesPagedAsync(
        int? month,
        int? year,
        string? search,
        string? position,
        string? bonusType,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = from b in Context.PayrollBonuses.AsNoTracking()
                    join emp in Context.Employees.AsNoTracking() on b.EmployeeId equals emp.Id
                    join user in Context.Users.AsNoTracking() on emp.UserId equals user.Id
                    select new
                    {
                        Bonus = b,
                        Employee = emp,
                        User = user
                    };

        if (month.HasValue && month.Value >= 1 && month.Value <= 12)
        {
            query = query.Where(x => x.Bonus.Month == month.Value);
        }

        if (year.HasValue && year.Value > 2000)
        {
            query = query.Where(x => x.Bonus.Year == year.Value);
        }

        if (!string.IsNullOrWhiteSpace(position))
        {
            var normalizedPosition = position.Trim().ToUpperInvariant();
            query = query.Where(x => x.Employee.Position == normalizedPosition);
        }

        if (!string.IsNullOrWhiteSpace(bonusType))
        {
            var normalizedBonusType = bonusType.Trim().ToUpperInvariant();
            query = query.Where(x => x.Bonus.BonusType == normalizedBonusType);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(x =>
                x.Employee.EmployeeCode.ToLower().Contains(term) ||
                x.User.FullName.ToLower().Contains(term) ||
                x.Bonus.Reason.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalBonusAmount = await query.SumAsync(x => x.Bonus.Amount, cancellationToken);

        var pagedItems = await query
            .OrderByDescending(x => x.Bonus.CreatedDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var periodIds = pagedItems
            .Where(x => x.Bonus.PayrollPeriodId.HasValue)
            .Select(x => x.Bonus.PayrollPeriodId!.Value)
            .Distinct()
            .ToList();

        var periodStatusMap = new Dictionary<Guid, string>();
        if (periodIds.Count > 0)
        {
            periodStatusMap = await Context.PayrollPeriods.AsNoTracking()
                .Where(p => periodIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Status, cancellationToken);
        }

        var targetMonth = month ?? DateTime.UtcNow.Month;
        var targetYear = year ?? DateTime.UtcNow.Year;
        var mStart = new DateOnly(targetYear, targetMonth, 1);
        var mEnd = mStart.AddMonths(1).AddDays(-1);
        var mStartUtc = mStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);
        var mEndUtc = mEnd.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc).AddHours(-7);

        var empIds = pagedItems.Select(x => x.Employee.Id).Distinct().ToList();
        var trainers = await Context.Trainers.AsNoTracking()
            .Where(t => empIds.Contains(t.EmployeeId))
            .ToDictionaryAsync(t => t.EmployeeId, t => t.Id, cancellationToken);

        var trainerIds = trainers.Values.ToList();
        var ptMap = await Context.TrainingSessions.AsNoTracking()
            .Where(s => trainerIds.Contains(s.TrainerId) &&
                        s.StartAt >= mStartUtc &&
                        s.StartAt <= mEndUtc &&
                        s.Status == TrainingSessionStatus.Completed)
            .GroupBy(s => s.TrainerId)
            .Select(g => new { TrainerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TrainerId, x => x.Count, cancellationToken);

        var attMap = await (from a in Context.EmployeeAttendanceRecords.AsNoTracking()
                            join s in Context.EmployeeWorkSchedules.AsNoTracking() on a.WorkScheduleId equals s.Id
                            where empIds.Contains(s.EmployeeId) && s.WorkDate >= mStart && s.WorkDate <= mEnd && a.CheckInAt != null
                            group a by s.EmployeeId into g
                            select new { EmployeeId = g.Key, Attended = g.Count() })
                            .ToDictionaryAsync(x => x.EmployeeId, x => x.Attended, cancellationToken);

        var dtos = pagedItems.Select(x =>
        {
            var periodStatus = PayrollPeriodStatus.Draft;
            if (x.Bonus.PayrollPeriodId.HasValue && periodStatusMap.TryGetValue(x.Bonus.PayrollPeriodId.Value, out var st))
            {
                periodStatus = st;
            }

            string perf;
            if (string.Equals(x.Employee.Position, "TRAINER", StringComparison.OrdinalIgnoreCase))
            {
                var ptCount = trainers.TryGetValue(x.Employee.Id, out var trId) && ptMap.TryGetValue(trId, out var c) ? c : 0;
                var days = attMap.TryGetValue(x.Employee.Id, out var d) ? d : 0;
                perf = $"{days} ngày công, {ptCount} buổi PT";
            }
            else
            {
                var days = attMap.TryGetValue(x.Employee.Id, out var d) ? d : 0;
                perf = $"{days} ngày công chấm vân tay";
            }

            return new PayrollBonusDto
            {
                Id = x.Bonus.Id,
                EmployeeId = x.Employee.Id,
                EmployeeCode = x.Employee.EmployeeCode,
                EmployeeName = x.User.FullName,
                Position = x.Employee.Position,
                Month = x.Bonus.Month,
                Year = x.Bonus.Year,
                BonusType = x.Bonus.BonusType,
                Amount = x.Bonus.Amount,
                Reason = x.Bonus.Reason,
                MonthlyPerformance = perf,
                PayrollPeriodStatus = periodStatus,
                CreatedAt = x.Bonus.CreatedDate
            };
        }).ToList();

        return new PayrollBonusPagedResultDto
        {
            Items = dtos,
            TotalBonusAmount = totalBonusAmount,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<Result<int>> CreateBonusesAsync(
        int month,
        int year,
        string bonusType,
        string targetType,
        string? position,
        List<Guid>? employeeIds,
        decimal amount,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
        {
            return Result.Failure<int>(Error.Validation("PayrollBonus.InvalidAmount", "Số tiền khen thưởng phải lớn hơn 0."));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Failure<int>(Error.Validation("PayrollBonus.EmptyReason", "Vui lòng nhập nội dung khen thưởng."));
        }

        var periodStart = new DateOnly(year, month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);
        var existingPeriod = await Context.PayrollPeriods
            .FirstOrDefaultAsync(p => p.PeriodStart == periodStart && p.PeriodEnd == periodEnd, cancellationToken);

        if (existingPeriod != null && (existingPeriod.Status == PayrollPeriodStatus.Confirmed || existingPeriod.Status == "LOCKED"))
        {
            return Result.Failure<int>(PayrollErrors.PeriodAlreadyConfirmed);
        }

        var empQuery = Context.Employees.Where(e => !e.IsDeleted && e.EmploymentStatus == EmployeeStatus.Active);
        var normalizedTarget = string.IsNullOrWhiteSpace(targetType) ? PayrollBonusTargetTypes.Individual : targetType.Trim().ToUpperInvariant();

        if (normalizedTarget == PayrollBonusTargetTypes.ByPosition && !string.IsNullOrWhiteSpace(position))
        {
            var pUpper = position.Trim().ToUpperInvariant();
            empQuery = empQuery.Where(e => e.Position == pUpper);
        }
        else if (normalizedTarget == PayrollBonusTargetTypes.Individual && employeeIds != null && employeeIds.Count > 0)
        {
            empQuery = empQuery.Where(e => employeeIds.Contains(e.Id));
        }

        var targetEmployees = await empQuery.ToListAsync(cancellationToken);
        if (targetEmployees.Count == 0)
        {
            return Result.Failure<int>(Error.Validation("PayrollBonus.NoEmployees", "Không tìm thấy nhân viên phù hợp với tiêu chí thưởng."));
        }

        var bonusEntities = new List<PayrollBonus>();
        foreach (var emp in targetEmployees)
        {
            var bonus = PayrollBonus.Create(
                emp.Id,
                month,
                year,
                bonusType,
                amount,
                reason,
                existingPeriod?.Id);
            bonusEntities.Add(bonus);
        }

        await Context.PayrollBonuses.AddRangeAsync(bonusEntities, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);

        if (existingPeriod != null)
        {
            await SyncPayslipsKpiBonusAsync(existingPeriod.Id, month, year, cancellationToken);
        }

        return Result.Success(bonusEntities.Count);
    }

    public async Task<Result<PayrollBonusDto>> UpdateBonusAsync(
        Guid id,
        decimal amount,
        string reason,
        string bonusType,
        CancellationToken cancellationToken = default)
    {
        if (amount < 0)
        {
            return Result.Failure<PayrollBonusDto>(Error.Validation("PayrollBonus.InvalidAmount", "Số tiền khen thưởng không được âm."));
        }

        var bonus = await Context.PayrollBonuses.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (bonus == null)
        {
            return Result.Failure<PayrollBonusDto>(Error.NotFound("PayrollBonus.NotFound", "Không tìm thấy bản ghi khen thưởng."));
        }

        var periodStart = new DateOnly(bonus.Year, bonus.Month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);
        var period = await Context.PayrollPeriods
            .FirstOrDefaultAsync(p => p.PeriodStart == periodStart && p.PeriodEnd == periodEnd, cancellationToken);

        if (period != null && (period.Status == PayrollPeriodStatus.Confirmed || period.Status == "LOCKED"))
        {
            return Result.Failure<PayrollBonusDto>(PayrollErrors.PeriodAlreadyConfirmed);
        }

        bonus.Update(amount, reason, bonusType);
        await Context.SaveChangesAsync(cancellationToken);

        if (period != null)
        {
            await SyncPayslipsKpiBonusAsync(period.Id, bonus.Month, bonus.Year, cancellationToken);
        }

        var emp = await Context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == bonus.EmployeeId, cancellationToken);
        var user = emp != null ? await Context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == emp.UserId, cancellationToken) : null;

        var dto = new PayrollBonusDto
        {
            Id = bonus.Id,
            EmployeeId = bonus.EmployeeId,
            EmployeeCode = emp?.EmployeeCode ?? string.Empty,
            EmployeeName = user?.FullName ?? string.Empty,
            Position = emp?.Position ?? string.Empty,
            Month = bonus.Month,
            Year = bonus.Year,
            BonusType = bonus.BonusType,
            Amount = bonus.Amount,
            Reason = bonus.Reason,
            PayrollPeriodStatus = period?.Status ?? PayrollPeriodStatus.Draft,
            CreatedAt = bonus.CreatedDate
        };

        return Result.Success(dto);
    }

    public async Task<Result<bool>> DeleteBonusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var bonus = await Context.PayrollBonuses.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (bonus == null)
        {
            return Result.Failure<bool>(Error.NotFound("PayrollBonus.NotFound", "Không tìm thấy bản ghi khen thưởng."));
        }

        var periodStart = new DateOnly(bonus.Year, bonus.Month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);
        var period = await Context.PayrollPeriods
            .FirstOrDefaultAsync(p => p.PeriodStart == periodStart && p.PeriodEnd == periodEnd, cancellationToken);

        if (period != null && (period.Status == PayrollPeriodStatus.Confirmed || period.Status == "LOCKED"))
        {
            return Result.Failure<bool>(PayrollErrors.PeriodAlreadyConfirmed);
        }

        Context.PayrollBonuses.Remove(bonus);
        await Context.SaveChangesAsync(cancellationToken);

        if (period != null)
        {
            await SyncPayslipsKpiBonusAsync(period.Id, bonus.Month, bonus.Year, cancellationToken);
        }

        return Result.Success(true);
    }

    public async Task<IReadOnlyCollection<BonusEmployeeLookupDto>> GetEmployeesLookupAsync(
        int? month,
        int? year,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = from emp in Context.Employees.AsNoTracking()
                    where !emp.IsDeleted && emp.EmploymentStatus == EmployeeStatus.Active
                    join user in Context.Users.AsNoTracking() on emp.UserId equals user.Id
                    select new
                    {
                        emp.Id,
                        emp.EmployeeCode,
                        emp.Position,
                        user.FullName
                    };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(x => x.EmployeeCode.ToLower().Contains(term) || x.FullName.ToLower().Contains(term));
        }

        var list = await query.OrderBy(x => x.EmployeeCode).ToListAsync(cancellationToken);

        var targetMonth = month ?? DateTime.UtcNow.Month;
        var targetYear = year ?? DateTime.UtcNow.Year;
        var mStart = new DateOnly(targetYear, targetMonth, 1);
        var mEnd = mStart.AddMonths(1).AddDays(-1);
        var mStartUtc = mStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);
        var mEndUtc = mEnd.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc).AddHours(-7);

        var empIds = list.Select(x => x.Id).ToList();
        var trainers = await Context.Trainers.AsNoTracking()
            .Where(t => empIds.Contains(t.EmployeeId))
            .ToDictionaryAsync(t => t.EmployeeId, t => t.Id, cancellationToken);

        var trainerIds = trainers.Values.ToList();
        var ptMap = await Context.TrainingSessions.AsNoTracking()
            .Where(s => trainerIds.Contains(s.TrainerId) &&
                        s.StartAt >= mStartUtc &&
                        s.StartAt <= mEndUtc &&
                        s.Status == TrainingSessionStatus.Completed)
            .GroupBy(s => s.TrainerId)
            .Select(g => new { TrainerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TrainerId, x => x.Count, cancellationToken);

        var attMap = await (from a in Context.EmployeeAttendanceRecords.AsNoTracking()
                            join s in Context.EmployeeWorkSchedules.AsNoTracking() on a.WorkScheduleId equals s.Id
                            where empIds.Contains(s.EmployeeId) && s.WorkDate >= mStart && s.WorkDate <= mEnd && a.CheckInAt != null
                            group a by s.EmployeeId into g
                            select new { EmployeeId = g.Key, Attended = g.Count() })
                            .ToDictionaryAsync(x => x.EmployeeId, x => x.Attended, cancellationToken);

        return list.Select(x =>
        {
            string perf;
            if (string.Equals(x.Position, "TRAINER", StringComparison.OrdinalIgnoreCase))
            {
                var ptCount = trainers.TryGetValue(x.Id, out var trId) && ptMap.TryGetValue(trId, out var c) ? c : 0;
                var days = attMap.TryGetValue(x.Id, out var d) ? d : 0;
                perf = $"{days} công, {ptCount} buổi PT";
            }
            else
            {
                var days = attMap.TryGetValue(x.Id, out var d) ? d : 0;
                perf = $"{days} công chấm vân tay";
            }

            return new BonusEmployeeLookupDto
            {
                EmployeeId = x.Id,
                EmployeeCode = x.EmployeeCode,
                FullName = x.FullName,
                Position = x.Position,
                MonthlyPerformance = perf
            };
        }).ToList();
    }

    private async Task SyncPayslipsKpiBonusAsync(Guid periodId, int month, int year, CancellationToken cancellationToken)
    {
        var payslips = await Context.Payslips
            .Where(ps => ps.PayrollPeriodId == periodId)
            .ToListAsync(cancellationToken);

        if (payslips.Count == 0) return;

        var bonuses = await Context.PayrollBonuses.AsNoTracking()
            .Where(b => b.Month == month && b.Year == year)
            .ToListAsync(cancellationToken);

        var bonusSumMap = bonuses
            .GroupBy(b => b.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.Amount));

        foreach (var slip in payslips)
        {
            bonusSumMap.TryGetValue(slip.EmployeeId, out var totalBonus);
            slip.AdjustKpiBonus(totalBonus, null);
        }

        await Context.SaveChangesAsync(cancellationToken);
    }
}
