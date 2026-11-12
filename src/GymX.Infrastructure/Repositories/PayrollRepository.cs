using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Payroll;
using GymX.Domain.Entities.Payroll;
using GymX.Domain.Errors;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class PayrollRepository(ApplicationDbContext context)
    : GenericRepository<PayrollPeriod>(context), IPayrollRepository
{
    public async Task<Result<PayrollPeriodDetailDto>> CalculatePayrollAsync(int month, int year, CancellationToken cancellationToken = default)
    {
        var periodStart = new DateOnly(year, month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);

        var existingPeriod = await GetPeriodByDateRangeAsync(periodStart, periodEnd, cancellationToken);
        if (existingPeriod != null && existingPeriod.Status == PayrollPeriodStatus.Confirmed)
        {
            return Result.Failure<PayrollPeriodDetailDto>(PayrollErrors.PeriodAlreadyConfirmed);
        }

        var activeEmployees = await Context.Employees
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.EmploymentStatus == EmployeeStatus.Active)
            .ToListAsync(cancellationToken);

        if (activeEmployees.Count == 0)
        {
            return Result.Failure<PayrollPeriodDetailDto>(PayrollErrors.NoActiveEmployeesFound);
        }

        var defaultFormula = await Context.SalaryFormulas
            .Where(f => f.TargetRole == "ALL" &&
                        f.Status == EmployeeStatus.Active &&
                        f.EffectiveDate <= periodEnd)
            .OrderByDescending(f => f.EffectiveDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (defaultFormula == null)
        {
            defaultFormula = await Context.SalaryFormulas
                .Where(f => f.Status == EmployeeStatus.Active && f.EffectiveDate <= periodEnd)
                .OrderByDescending(f => f.EffectiveDate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (defaultFormula == null)
        {
            return Result.Failure<PayrollPeriodDetailDto>(PayrollErrors.NoActiveFormulaFound("Tất cả nhân viên"));
        }

        PayrollPeriod period;
        if (existingPeriod == null)
        {
            period = PayrollPeriod.Create(periodStart, periodEnd, defaultFormula.Id);
            await Context.PayrollPeriods.AddAsync(period, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            period = existingPeriod;
            period.UpdateSalaryFormula(defaultFormula.Id);
        }

        var periodStartUtc = periodStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);
        var periodEndUtc = periodEnd.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc).AddHours(-7);

        var existingPayslips = await Context.Payslips
            .Where(ps => ps.PayrollPeriodId == period.Id)
            .ToListAsync(cancellationToken);

        var existingPayslipsMap = existingPayslips.ToDictionary(ps => ps.EmployeeId);

        var schedulesCountMap = await Context.EmployeeWorkSchedules
            .AsNoTracking()
            .Where(s => s.WorkDate >= periodStart && s.WorkDate <= periodEnd && s.Status == ScheduleStatus.Approved)
            .GroupBy(s => s.EmployeeId)
            .Select(g => new { EmployeeId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EmployeeId, x => x.Count, cancellationToken);

        var attendanceRecords = await (from att in Context.EmployeeAttendanceRecords.AsNoTracking()
                                       join sch in Context.EmployeeWorkSchedules.AsNoTracking() on att.WorkScheduleId equals sch.Id
                                       join shift in Context.ShiftDefinitions.AsNoTracking() on sch.ShiftId equals shift.Id into shifts
                                       from shift in shifts.DefaultIfEmpty()
                                       where sch.WorkDate >= periodStart && sch.WorkDate <= periodEnd
                                       select new
                                       {
                                           sch.EmployeeId,
                                           att.CheckInAt,
                                           att.CheckOutAt,
                                           att.LateMinutes,
                                           att.EarlyLeaveMinutes,
                                           ShiftStartTime = shift != null ? (TimeOnly?)shift.StartTime : null,
                                           ShiftEndTime = shift != null ? (TimeOnly?)shift.EndTime : null
                                       }).ToListAsync(cancellationToken);

        var attendanceStatsMap = attendanceRecords
            .GroupBy(a => a.EmployeeId)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var attended = g.Where(x => x.CheckInAt != null).ToList();
                    var totalHours = attended.Sum(x =>
                    {
                        if (x.CheckInAt != null && x.CheckOutAt != null && x.CheckOutAt > x.CheckInAt)
                        {
                            return Math.Round((decimal)(x.CheckOutAt.Value - x.CheckInAt.Value).TotalHours, 2);
                        }
                        if (x.ShiftStartTime != null && x.ShiftEndTime != null)
                        {
                            if (x.ShiftEndTime > x.ShiftStartTime)
                            {
                                return (decimal)(x.ShiftEndTime.Value - x.ShiftStartTime.Value).TotalHours;
                            }
                            return (decimal)(24 - (x.ShiftStartTime.Value - x.ShiftEndTime.Value).TotalHours);
                        }
                        return 4.0m;
                    });

                    return new
                    {
                        LateCount = g.Count(x => x.LateMinutes > 0),
                        EarlyLeaveCount = g.Count(x => x.EarlyLeaveMinutes > 0),
                        AttendedCount = attended.Count,
                        TotalWorkedHours = totalHours
                    };
                });

        var employeeIds = activeEmployees.Select(e => e.Id).ToList();
        var trainersMap = await Context.Trainers
            .AsNoTracking()
            .Where(t => employeeIds.Contains(t.EmployeeId))
            .ToDictionaryAsync(t => t.EmployeeId, t => t.Id, cancellationToken);

        var trainerIds = trainersMap.Values.ToList();
        var completedSessionsMap = await Context.TrainingSessions
            .AsNoTracking()
            .Where(s => trainerIds.Contains(s.TrainerId) &&
                        s.StartAt >= periodStartUtc &&
                        s.StartAt <= periodEndUtc &&
                        s.Status == TrainingSessionStatus.Completed)
            .GroupBy(s => s.TrainerId)
            .Select(g => new { TrainerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TrainerId, x => x.Count, cancellationToken);

        var violationsMap = await Context.Violations
            .AsNoTracking()
            .Where(v => v.Status == ViolationStatus.Confirmed &&
                        v.OccurredAt >= periodStartUtc &&
                        v.OccurredAt <= periodEndUtc)
            .GroupBy(v => v.EmployeeId)
            .Select(g => new { EmployeeId = g.Key, TotalFine = g.Sum(v => v.FineAmount) })
            .ToDictionaryAsync(x => x.EmployeeId, x => x.TotalFine, cancellationToken);

        var employeeUserIds = activeEmployees.Select(e => e.UserId).Distinct().ToList();
        var salesRevenueMap = await Context.Invoices
            .AsNoTracking()
            .Where(i => i.CreatorId != null &&
                        employeeUserIds.Contains(i.CreatorId.Value) &&
                        i.Status == InvoiceStatus.Paid &&
                        i.IssuedAt >= periodStartUtc &&
                        i.IssuedAt <= periodEndUtc)
            .GroupBy(i => i.CreatorId!.Value)
            .Select(g => new { UserId = g.Key, TotalRevenue = g.Sum(i => i.TotalAmount) })
            .ToDictionaryAsync(x => x.UserId, x => x.TotalRevenue, cancellationToken);

        var roleFormulas = await Context.SalaryFormulas
            .AsNoTracking()
            .Where(f => f.Status == EmployeeStatus.Active && f.EffectiveDate <= periodEnd)
            .OrderByDescending(f => f.EffectiveDate)
            .ToListAsync(cancellationToken);

        foreach (var employee in activeEmployees)
        {
            var roleFormula = roleFormulas.FirstOrDefault(f =>
                string.Equals(f.TargetRole, employee.Position, StringComparison.OrdinalIgnoreCase));
            var formula = roleFormula ?? defaultFormula;

            schedulesCountMap.TryGetValue(employee.Id, out var scheduledShifts);
            attendanceStatsMap.TryGetValue(employee.Id, out var attendanceStats);

            var hasAttendanceRecords = attendanceStats != null;
            var lateCount = attendanceStats?.LateCount ?? 0;
            var earlyLeaveCount = attendanceStats?.EarlyLeaveCount ?? 0;

            var isShiftRate = string.Equals(formula.BaseRateType, SalaryBaseRateType.PerShift, StringComparison.OrdinalIgnoreCase);
            var isHourlyRate = string.Equals(formula.BaseRateType, SalaryBaseRateType.Hourly, StringComparison.OrdinalIgnoreCase);
            var unitName = isShiftRate ? "ca" : (isHourlyRate ? "giờ" : "ngày");

            int workedShifts;
            decimal workedHours;

            if (hasAttendanceRecords)
            {
                workedShifts = attendanceStats!.AttendedCount;
                workedHours = attendanceStats.TotalWorkedHours;
            }
            else if (scheduledShifts > 0)
            {
                workedShifts = scheduledShifts;
                workedHours = scheduledShifts * 4m;
            }
            else
            {
                if (isShiftRate || isHourlyRate)
                {
                    workedShifts = 0;
                    workedHours = 0m;
                }
                else
                {
                    var standardDays = formula.StandardWorkDays > 0 ? formula.StandardWorkDays : 26;
                    workedShifts = standardDays;
                    workedHours = standardDays * 8m;
                }
            }

            var workedUnits = isHourlyRate ? (int)Math.Round(workedHours) : workedShifts;

            decimal baseSalaryRate;
            if (isHourlyRate)
            {
                baseSalaryRate = (employee.BaseSalaryRate > 0 && employee.BaseSalaryRate <= 500000m)
                    ? employee.BaseSalaryRate
                    : formula.BaseRate;
            }
            else if (isShiftRate)
            {
                baseSalaryRate = (employee.BaseSalaryRate > 0 && employee.BaseSalaryRate <= 1000000m)
                    ? employee.BaseSalaryRate
                    : formula.BaseRate;
            }
            else
            {
                baseSalaryRate = employee.BaseSalaryRate > 0 ? employee.BaseSalaryRate : formula.BaseRate;
            }

            decimal calculatedBaseSalary;
            if (isShiftRate)
            {
                calculatedBaseSalary = baseSalaryRate * workedShifts;
            }
            else if (isHourlyRate)
            {
                calculatedBaseSalary = Math.Round(baseSalaryRate * workedHours, 2);
            }
            else
            {
                var standardDays = formula.StandardWorkDays > 0 ? formula.StandardWorkDays : 26;
                calculatedBaseSalary = Math.Round((baseSalaryRate / standardDays) * workedShifts, 2);
            }

            var ptSessionsCount = 0;
            if (trainersMap.TryGetValue(employee.Id, out var trainerId))
            {
                completedSessionsMap.TryGetValue(trainerId, out ptSessionsCount);
            }

            var teachingAmount = ptSessionsCount * formula.PricePerPtSession;

            salesRevenueMap.TryGetValue(employee.UserId, out var salesRevenue);
            var commissionAmount = formula.SalesCommissionPercent > 0
                ? Math.Round(salesRevenue * (formula.SalesCommissionPercent / 100m), 2)
                : 0m;

            var bonusAmount = teachingAmount + commissionAmount;

            violationsMap.TryGetValue(employee.Id, out var penaltyAmount);

            var note = $"Công thức: {formula.Name}. Lương cứng: {calculatedBaseSalary:N0} ₫ ({workedUnits}/{formula.StandardWorkDays} {unitName}). Tiền dạy PT: {teachingAmount:N0} ₫ ({ptSessionsCount} buổi). Hoa hồng bán: {commissionAmount:N0} ₫ ({formula.SalesCommissionPercent}% của {salesRevenue:N0} ₫). Phạt: -{penaltyAmount:N0} ₫.";

            if (existingPayslipsMap.TryGetValue(employee.Id, out var existingPayslip))
            {
                existingPayslip.UpdateCalculatedSalary(
                    calculatedBaseSalary,
                    workedUnits,
                    lateCount,
                    earlyLeaveCount,
                    bonusAmount,
                    penaltyAmount,
                    ptSessionsCount,
                    teachingAmount,
                    salesRevenue,
                    commissionAmount,
                    existingPayslip.KpiBonusAmount,
                    note);
            }
            else
            {
                var newPayslip = Payslip.Create(
                    period.Id,
                    employee.Id,
                    calculatedBaseSalary,
                    workedUnits,
                    lateCount,
                    earlyLeaveCount,
                    bonusAmount,
                    penaltyAmount,
                    ptSessionsCount,
                    teachingAmount,
                    salesRevenue,
                    commissionAmount,
                    0m,
                    note);

                await Context.Payslips.AddAsync(newPayslip, cancellationToken);
            }
        }

        period.MarkAsCalculated();

        await Context.SaveChangesAsync(cancellationToken);

        var periodDetail = await GetPayrollPeriodDetailByIdAsync(period.Id, cancellationToken);
        if (periodDetail == null)
        {
            return Result.Failure<PayrollPeriodDetailDto>(PayrollErrors.PeriodNotFound(period.Id));
        }

        return Result.Success(periodDetail);
    }

    public async Task<PayrollPeriod?> GetPeriodByDateRangeAsync(DateOnly periodStart, DateOnly periodEnd, CancellationToken cancellationToken = default)
    {
        return await Context.PayrollPeriods
            .FirstOrDefaultAsync(p => p.PeriodStart == periodStart && p.PeriodEnd == periodEnd, cancellationToken);
    }

    public async Task<PagedResult<PayrollPeriodSummaryDto>> GetPayrollPeriodsPagedAsync(
        int? year,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var periodQuery = from period in Context.PayrollPeriods.AsNoTracking()
                          join formula in Context.SalaryFormulas.AsNoTracking() on period.SalaryFormulaId equals formula.Id
                          join confirmedUser in Context.Users.AsNoTracking() on period.ConfirmedBy equals confirmedUser.Id into confirmedUsers
                          from confirmedUser in confirmedUsers.DefaultIfEmpty()
                          select new
                          {
                              Period = period,
                              Formula = formula,
                              ConfirmedUserName = confirmedUser != null ? confirmedUser.FullName : null
                          };

        if (year.HasValue && year.Value > 2000)
        {
            periodQuery = periodQuery.Where(item => item.Period.PeriodStart.Year == year.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            periodQuery = periodQuery.Where(item => item.Period.Status == normalizedStatus);
        }

        var orderedQuery = periodQuery.OrderByDescending(item => item.Period.PeriodStart);

        var totalCount = await orderedQuery.CountAsync(cancellationToken);

        var pagedItems = await orderedQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var periodIds = pagedItems.Select(x => x.Period.Id).ToList();

        var payslipStats = await Context.Payslips.AsNoTracking()
            .Where(ps => periodIds.Contains(ps.PayrollPeriodId))
            .GroupBy(ps => ps.PayrollPeriodId)
            .Select(g => new
            {
                PeriodId = g.Key,
                TotalSalary = g.Sum(ps => ps.TotalSalary),
                TotalBaseSalary = g.Sum(ps => ps.BaseSalary),
                TotalPtAmount = g.Sum(ps => ps.PtTeachingAmount),
                TotalCommissionAmount = g.Sum(ps => ps.CommissionAmount),
                TotalKpiBonusAmount = g.Sum(ps => ps.KpiBonusAmount),
                TotalPenaltyAmount = g.Sum(ps => ps.PenaltyAmount),
                TotalEmployees = g.Count()
            })
            .ToDictionaryAsync(x => x.PeriodId, cancellationToken);

        var items = pagedItems.Select(item =>
        {
            payslipStats.TryGetValue(item.Period.Id, out var stats);
            return new PayrollPeriodSummaryDto
            {
                Id = item.Period.Id,
                PeriodStart = item.Period.PeriodStart,
                PeriodEnd = item.Period.PeriodEnd,
                SalaryFormulaId = item.Period.SalaryFormulaId,
                SalaryFormulaName = item.Formula.Name,
                Status = item.Period.Status,
                TotalSalary = stats?.TotalSalary ?? 0m,
                TotalBaseSalary = stats?.TotalBaseSalary ?? 0m,
                TotalPtAmount = stats?.TotalPtAmount ?? 0m,
                TotalCommissionAmount = stats?.TotalCommissionAmount ?? 0m,
                TotalKpiBonusAmount = stats?.TotalKpiBonusAmount ?? 0m,
                TotalPenaltyAmount = stats?.TotalPenaltyAmount ?? 0m,
                TotalEmployees = stats?.TotalEmployees ?? 0,
                CalculatedAt = item.Period.CalculatedAt,
                ConfirmedAt = item.Period.ConfirmedAt,
                ConfirmedByName = item.ConfirmedUserName
            };
        }).ToList();

        return new PagedResult<PayrollPeriodSummaryDto>(items, totalCount, pageIndex, pageSize);
    }

    public async Task<PayrollPeriodDetailDto?> GetPayrollPeriodDetailByIdAsync(Guid periodId, CancellationToken cancellationToken = default)
    {
        var periodQuery = from period in Context.PayrollPeriods.AsNoTracking()
                          where period.Id == periodId
                          join formula in Context.SalaryFormulas.AsNoTracking() on period.SalaryFormulaId equals formula.Id
                          join confirmedUser in Context.Users.AsNoTracking() on period.ConfirmedBy equals confirmedUser.Id into confirmedUsers
                          from confirmedUser in confirmedUsers.DefaultIfEmpty()
                          select new
                          {
                              Period = period,
                              Formula = formula,
                              ConfirmedUserName = confirmedUser != null ? confirmedUser.FullName : null
                          };

        var periodRecord = await periodQuery.FirstOrDefaultAsync(cancellationToken);
        if (periodRecord == null)
        {
            return null;
        }

        var payslips = await (from ps in Context.Payslips.AsNoTracking()
                              where ps.PayrollPeriodId == periodId
                              join emp in Context.Employees.AsNoTracking() on ps.EmployeeId equals emp.Id
                              join user in Context.Users.AsNoTracking() on emp.UserId equals user.Id
                              orderby user.FullName
                              select new PayslipSummaryDto
                              {
                                  Id = ps.Id,
                                  PayrollPeriodId = ps.PayrollPeriodId,
                                  EmployeeId = ps.EmployeeId,
                                  EmployeeCode = emp.EmployeeCode,
                                  EmployeeName = user.FullName,
                                  Position = emp.Position,
                                  BaseSalary = ps.BaseSalary,
                                  WorkedShifts = ps.WorkedShifts,
                                  BonusAmount = ps.BonusAmount,
                                  PenaltyAmount = ps.PenaltyAmount,
                                  TotalSalary = ps.TotalSalary,
                                  Status = ps.Status,
                                  GeneratedAt = ps.GeneratedAt,
                                  PtSessionsCount = ps.PtSessionsCount,
                                  PtTeachingAmount = ps.PtTeachingAmount,
                                  SalesRevenue = ps.SalesRevenue,
                                  CommissionAmount = ps.CommissionAmount,
                                  KpiBonusAmount = ps.KpiBonusAmount
                              }).ToListAsync(cancellationToken);

        return new PayrollPeriodDetailDto
        {
            Id = periodRecord.Period.Id,
            PeriodStart = periodRecord.Period.PeriodStart,
            PeriodEnd = periodRecord.Period.PeriodEnd,
            SalaryFormulaId = periodRecord.Period.SalaryFormulaId,
            SalaryFormulaName = periodRecord.Formula.Name,
            Status = periodRecord.Period.Status,
            TotalSalary = payslips.Sum(ps => ps.TotalSalary),
            TotalBaseSalary = payslips.Sum(ps => ps.BaseSalary),
            TotalPtAmount = payslips.Sum(ps => ps.PtTeachingAmount),
            TotalCommissionAmount = payslips.Sum(ps => ps.CommissionAmount),
            TotalKpiBonusAmount = payslips.Sum(ps => ps.KpiBonusAmount),
            TotalPenaltyAmount = payslips.Sum(ps => ps.PenaltyAmount),
            TotalEmployees = payslips.Count,
            CalculatedAt = periodRecord.Period.CalculatedAt,
            ConfirmedAt = periodRecord.Period.ConfirmedAt,
            ConfirmedByName = periodRecord.ConfirmedUserName,
            Payslips = payslips
        };
    }

    public async Task<PagedResult<PayslipSummaryDto>> GetPayslipsPagedAsync(
        Guid? periodId,
        Guid? employeeId,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var payslipQuery = from ps in Context.Payslips.AsNoTracking()
                           join emp in Context.Employees.AsNoTracking() on ps.EmployeeId equals emp.Id
                           join user in Context.Users.AsNoTracking() on emp.UserId equals user.Id
                           select new
                           {
                               Payslip = ps,
                               Employee = emp,
                               User = user
                           };

        if (periodId.HasValue && periodId.Value != Guid.Empty)
        {
            payslipQuery = payslipQuery.Where(item => item.Payslip.PayrollPeriodId == periodId.Value);
        }

        if (employeeId.HasValue && employeeId.Value != Guid.Empty)
        {
            payslipQuery = payslipQuery.Where(item => item.Payslip.EmployeeId == employeeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            payslipQuery = payslipQuery.Where(item => item.Payslip.Status == normalizedStatus);
        }

        var orderedQuery = payslipQuery.OrderBy(item => item.User.FullName);

        var totalCount = await orderedQuery.CountAsync(cancellationToken);

        var items = await orderedQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new PayslipSummaryDto
            {
                Id = item.Payslip.Id,
                PayrollPeriodId = item.Payslip.PayrollPeriodId,
                EmployeeId = item.Payslip.EmployeeId,
                EmployeeCode = item.Employee.EmployeeCode,
                EmployeeName = item.User.FullName,
                Position = item.Employee.Position,
                BaseSalary = item.Payslip.BaseSalary,
                WorkedShifts = item.Payslip.WorkedShifts,
                BonusAmount = item.Payslip.BonusAmount,
                PenaltyAmount = item.Payslip.PenaltyAmount,
                TotalSalary = item.Payslip.TotalSalary,
                Status = item.Payslip.Status,
                GeneratedAt = item.Payslip.GeneratedAt,
                PtSessionsCount = item.Payslip.PtSessionsCount,
                PtTeachingAmount = item.Payslip.PtTeachingAmount,
                SalesRevenue = item.Payslip.SalesRevenue,
                CommissionAmount = item.Payslip.CommissionAmount,
                KpiBonusAmount = item.Payslip.KpiBonusAmount
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PayslipSummaryDto>(items, totalCount, pageIndex, pageSize);
    }

    public async Task<PayslipDetailDto?> GetPayslipDetailByIdAsync(Guid payslipId, CancellationToken cancellationToken = default)
    {
        var payslipQuery = from ps in Context.Payslips.AsNoTracking()
                           where ps.Id == payslipId
                           join emp in Context.Employees.AsNoTracking() on ps.EmployeeId equals emp.Id
                           join user in Context.Users.AsNoTracking() on emp.UserId equals user.Id
                           join period in Context.PayrollPeriods.AsNoTracking() on ps.PayrollPeriodId equals period.Id
                           select new PayslipDetailDto
                           {
                               Id = ps.Id,
                               PayrollPeriodId = ps.PayrollPeriodId,
                               PeriodMonth = period.PeriodStart.Month,
                               PeriodYear = period.PeriodStart.Year,
                               EmployeeId = ps.EmployeeId,
                               EmployeeCode = emp.EmployeeCode,
                               EmployeeName = user.FullName,
                               Position = emp.Position,
                               BaseSalary = ps.BaseSalary,
                               WorkedShifts = ps.WorkedShifts,
                               LateCount = ps.LateCount,
                               EarlyLeaveCount = ps.EarlyLeaveCount,
                               BonusAmount = ps.BonusAmount,
                               PenaltyAmount = ps.PenaltyAmount,
                               TotalSalary = ps.TotalSalary,
                               Status = ps.Status,
                               Note = ps.Note,
                               GeneratedAt = ps.GeneratedAt,
                               PtSessionsCount = ps.PtSessionsCount,
                               PtTeachingAmount = ps.PtTeachingAmount,
                               SalesRevenue = ps.SalesRevenue,
                               CommissionAmount = ps.CommissionAmount,
                               KpiBonusAmount = ps.KpiBonusAmount
                           };

        return await payslipQuery.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PayslipSummaryDto>> GetMyPayslipsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var employee = await Context.Employees.AsNoTracking()
            .FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted, cancellationToken);

        if (employee == null)
        {
            return [];
        }

        var payslips = await (from ps in Context.Payslips.AsNoTracking()
                              where ps.EmployeeId == employee.Id
                              join period in Context.PayrollPeriods.AsNoTracking() on ps.PayrollPeriodId equals period.Id
                              join user in Context.Users.AsNoTracking() on employee.UserId equals user.Id
                              orderby period.PeriodStart descending
                              select new PayslipSummaryDto
                              {
                                  Id = ps.Id,
                                  PayrollPeriodId = ps.PayrollPeriodId,
                                  EmployeeId = ps.EmployeeId,
                                  EmployeeCode = employee.EmployeeCode,
                                  EmployeeName = user.FullName,
                                  Position = employee.Position,
                                  BaseSalary = ps.BaseSalary,
                                  WorkedShifts = ps.WorkedShifts,
                                  BonusAmount = ps.BonusAmount,
                                  PenaltyAmount = ps.PenaltyAmount,
                                  TotalSalary = ps.TotalSalary,
                                  Status = ps.Status,
                                  GeneratedAt = ps.GeneratedAt,
                                  PtSessionsCount = ps.PtSessionsCount,
                                  PtTeachingAmount = ps.PtTeachingAmount,
                                  SalesRevenue = ps.SalesRevenue,
                                  CommissionAmount = ps.CommissionAmount,
                                  KpiBonusAmount = ps.KpiBonusAmount
                              }).ToListAsync(cancellationToken);

        return payslips;
    }

    public async Task<List<Payslip>> GetPayslipsByPeriodIdAsync(Guid periodId, CancellationToken cancellationToken = default)
    {
        return await Context.Payslips
            .Where(ps => ps.PayrollPeriodId == periodId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<PayslipDetailDto>> AdjustPayslipKpiBonusAsync(
        Guid payslipId,
        decimal kpiBonusAmount,
        string? note,
        CancellationToken cancellationToken = default)
    {
        var payslip = await Context.Payslips
            .FirstOrDefaultAsync(ps => ps.Id == payslipId, cancellationToken);

        if (payslip == null)
        {
            return Result.Failure<PayslipDetailDto>(PayrollErrors.PayslipNotFound(payslipId));
        }

        var period = await Context.PayrollPeriods
            .FirstOrDefaultAsync(p => p.Id == payslip.PayrollPeriodId, cancellationToken);

        if (period == null)
        {
            return Result.Failure<PayslipDetailDto>(PayrollErrors.PeriodNotFound(payslip.PayrollPeriodId));
        }

        if (period.Status == PayrollPeriodStatus.Confirmed)
        {
            return Result.Failure<PayslipDetailDto>(PayrollErrors.PeriodAlreadyConfirmed);
        }

        payslip.AdjustKpiBonus(kpiBonusAmount, note);
        await Context.SaveChangesAsync(cancellationToken);

        var detail = await GetPayslipDetailByIdAsync(payslipId, cancellationToken);
        if (detail == null)
        {
            return Result.Failure<PayslipDetailDto>(PayrollErrors.PayslipNotFound(payslipId));
        }

        return Result.Success(detail);
    }
}
