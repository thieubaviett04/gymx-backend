using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Finance;
using GymX.Domain.Constants.Payroll;
using GymX.Domain.Entities.Payroll;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class EmployeeKpiRepository(ApplicationDbContext context)
    : GenericRepository<EmployeeKpi>(context), IEmployeeKpiRepository
{
    public Task<bool> ExistsAssignmentAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(k => k.EmployeeId == employeeId && k.Month == month && k.Year == year, cancellationToken);

    public async Task<List<Guid>> GetAssignedEmployeeIdsAsync(IEnumerable<Guid> employeeIds, int month, int year, CancellationToken cancellationToken = default)
    {
        var ids = employeeIds.ToList();
        return await DbSet
            .Where(k => ids.Contains(k.EmployeeId) && k.Month == month && k.Year == year)
            .Select(k => k.EmployeeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EmployeeKpi>> GetActiveByTemplateIdAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        var confirmedPeriods = await Context.PayrollPeriods.AsNoTracking()
            .Where(p => p.Status == PayrollPeriodStatus.Confirmed)
            .Select(p => new { Month = p.PeriodStart.Month, Year = p.PeriodStart.Year })
            .ToListAsync(cancellationToken);

        var allKpis = await DbSet
            .Where(k => k.KpiTemplateId == templateId)
            .ToListAsync(cancellationToken);

        return allKpis
            .Where(k => !confirmedPeriods.Any(p => p.Month == k.Month && p.Year == k.Year))
            .ToList();
    }

    public async Task<PagedResult<EmployeeKpiProgressSummaryDto>> GetKpiProgressPagedAsync(
        int month,
        int year,
        string? search,
        string? position,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var periodStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = periodStart.AddMonths(1).AddTicks(-1);

        var query = from kpi in DbSet.AsNoTracking()
                    join emp in Context.Employees.AsNoTracking() on kpi.EmployeeId equals emp.Id
                    join usr in Context.Users.AsNoTracking() on emp.UserId equals usr.Id
                    join tmpl in Context.KpiTemplates.AsNoTracking() on kpi.KpiTemplateId equals tmpl.Id
                    where kpi.Month == month && kpi.Year == year && !emp.IsDeleted
                    select new
                    {
                        Kpi = kpi,
                        Employee = emp,
                        User = usr,
                        Template = tmpl
                    };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchKw = search.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Employee.EmployeeCode, $"%{searchKw}%") ||
                                     EF.Functions.ILike(x.User.FullName, $"%{searchKw}%"));
        }

        if (!string.IsNullOrWhiteSpace(position) && position.Trim().ToUpperInvariant() != "ALL")
        {
            var filterPos = position.Trim().ToUpperInvariant();
            query = query.Where(x => x.Employee.Position == filterPos);
        }

        var totalRecords = await query.CountAsync(cancellationToken);

        var pagedItems = await query
            .OrderBy(x => x.Employee.EmployeeCode)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var empIds = pagedItems.Select(x => x.Employee.Id).ToList();
        var userIds = pagedItems.Select(x => x.User.Id).Distinct().ToList();

        var trainersMap = await Context.Trainers
            .AsNoTracking()
            .Where(t => empIds.Contains(t.EmployeeId))
            .ToDictionaryAsync(t => t.EmployeeId, t => t.Id, cancellationToken);

        var trainerIds = trainersMap.Values.ToList();
        var sessionsCountMap = await Context.TrainingSessions
            .AsNoTracking()
            .Where(s => trainerIds.Contains(s.TrainerId) &&
                        s.StartAt >= periodStart &&
                        s.StartAt <= periodEnd &&
                        s.Status == TrainingSessionStatus.Completed)
            .GroupBy(s => s.TrainerId)
            .Select(g => new { TrainerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TrainerId, x => x.Count, cancellationToken);

        var revenueMap = await Context.Invoices
            .AsNoTracking()
            .Where(i => i.CreatorId != null &&
                        userIds.Contains(i.CreatorId.Value) &&
                        i.Status == InvoiceStatus.Paid &&
                        i.IssuedAt >= periodStart &&
                        i.IssuedAt <= periodEnd)
            .GroupBy(i => i.CreatorId!.Value)
            .Select(g => new { UserId = g.Key, Total = g.Sum(i => i.TotalAmount) })
            .ToDictionaryAsync(x => x.UserId, x => x.Total, cancellationToken);

        var resultList = new List<EmployeeKpiProgressSummaryDto>();
        foreach (var item in pagedItems)
        {
            trainersMap.TryGetValue(item.Employee.Id, out var trainerId);
            sessionsCountMap.TryGetValue(trainerId, out var actualSessions);
            revenueMap.TryGetValue(item.User.Id, out var actualRevenue);

            var (sessionProgress, revenueProgress, memberProgress, overallProgress) = CalculateProgress(
                item.Kpi.TargetPtSessions, actualSessions,
                item.Kpi.TargetRevenue, actualRevenue,
                item.Kpi.TargetNewMembers, 0,
                item.Employee.Position);

            var calculatedStatus = overallProgress >= 100m ? KpiStatus.Achieved : KpiStatus.InProgress;

            resultList.Add(new EmployeeKpiProgressSummaryDto(
                item.Kpi.Id,
                item.Employee.Id,
                item.Employee.EmployeeCode,
                item.User.FullName,
                item.Employee.Position,
                item.Template.Name,
                item.Kpi.Month,
                item.Kpi.Year,
                item.Kpi.TargetPtSessions,
                actualSessions,
                sessionProgress,
                item.Kpi.TargetRevenue,
                actualRevenue,
                revenueProgress,
                item.Kpi.TargetNewMembers,
                0,
                overallProgress,
                item.Kpi.TargetBonusAmount,
                calculatedStatus,
                item.Kpi.Note,
                item.Kpi.AssignedAt
            ));
        }

        if (!string.IsNullOrWhiteSpace(status) && status.Trim().ToUpperInvariant() != "ALL")
        {
            var filterStatus = status.Trim().ToUpperInvariant();
            resultList = resultList.Where(x => x.Status == filterStatus).ToList();
        }

        return new PagedResult<EmployeeKpiProgressSummaryDto>(resultList, totalRecords, pageIndex, pageSize);
    }

    public async Task<EmployeeKpiDetailDto?> GetKpiDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await (from kpi in DbSet.AsNoTracking()
                          join emp in Context.Employees.AsNoTracking() on kpi.EmployeeId equals emp.Id
                          join usr in Context.Users.AsNoTracking() on emp.UserId equals usr.Id
                          join tmpl in Context.KpiTemplates.AsNoTracking() on kpi.KpiTemplateId equals tmpl.Id
                          where kpi.Id == id
                          select new { kpi, emp, usr, tmpl })
                          .FirstOrDefaultAsync(cancellationToken);

        if (item == null) return null;

        var periodStart = new DateTime(item.kpi.Year, item.kpi.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = periodStart.AddMonths(1).AddTicks(-1);

        var trainer = await Context.Trainers.AsNoTracking().FirstOrDefaultAsync(t => t.EmployeeId == item.emp.Id, cancellationToken);
        var actualSessions = 0;
        if (trainer != null)
        {
            actualSessions = await Context.TrainingSessions.AsNoTracking()
                .CountAsync(s => s.TrainerId == trainer.Id && s.StartAt >= periodStart && s.StartAt <= periodEnd && s.Status == TrainingSessionStatus.Completed, cancellationToken);
        }

        var actualRevenue = await Context.Invoices.AsNoTracking()
            .Where(i => i.CreatorId == item.usr.Id && i.Status == InvoiceStatus.Paid && i.IssuedAt >= periodStart && i.IssuedAt <= periodEnd)
            .SumAsync(i => (decimal?)i.TotalAmount, cancellationToken) ?? 0m;

        var (sessionProgress, revenueProgress, memberProgress, overallProgress) = CalculateProgress(
            item.kpi.TargetPtSessions, actualSessions,
            item.kpi.TargetRevenue, actualRevenue,
            item.kpi.TargetNewMembers, 0,
            item.emp.Position);

        return new EmployeeKpiDetailDto(
            item.kpi.Id,
            item.emp.Id,
            item.emp.EmployeeCode,
            item.usr.FullName,
            item.emp.Position,
            item.tmpl.Id,
            item.tmpl.Name,
            item.kpi.Month,
            item.kpi.Year,
            item.kpi.TargetPtSessions,
            actualSessions,
            sessionProgress,
            item.kpi.TargetRevenue,
            actualRevenue,
            revenueProgress,
            item.kpi.TargetNewMembers,
            0,
            overallProgress,
            item.kpi.TargetBonusAmount,
            overallProgress >= 100m ? KpiStatus.Achieved : KpiStatus.InProgress,
            item.kpi.Note,
            item.kpi.AssignedAt
        );
    }

    public async Task<MyKpiDto?> GetEmployeeKpiByPeriodAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default)
    {
        var item = await (from kpi in DbSet.AsNoTracking()
                          join emp in Context.Employees.AsNoTracking() on kpi.EmployeeId equals emp.Id
                          join usr in Context.Users.AsNoTracking() on emp.UserId equals usr.Id
                          join tmpl in Context.KpiTemplates.AsNoTracking() on kpi.KpiTemplateId equals tmpl.Id
                          where kpi.EmployeeId == employeeId && kpi.Month == month && kpi.Year == year
                          select new { kpi, emp, usr, tmpl })
                          .FirstOrDefaultAsync(cancellationToken);

        if (item == null) return null;

        var periodStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = periodStart.AddMonths(1).AddTicks(-1);

        var trainer = await Context.Trainers.AsNoTracking().FirstOrDefaultAsync(t => t.EmployeeId == item.emp.Id, cancellationToken);
        var actualSessions = 0;
        if (trainer != null)
        {
            actualSessions = await Context.TrainingSessions.AsNoTracking()
                .CountAsync(s => s.TrainerId == trainer.Id && s.StartAt >= periodStart && s.StartAt <= periodEnd && s.Status == TrainingSessionStatus.Completed, cancellationToken);
        }

        var actualRevenue = await Context.Invoices.AsNoTracking()
            .Where(i => i.CreatorId == item.usr.Id && i.Status == InvoiceStatus.Paid && i.IssuedAt >= periodStart && i.IssuedAt <= periodEnd)
            .SumAsync(i => (decimal?)i.TotalAmount, cancellationToken) ?? 0m;

        var (sessionProgress, revenueProgress, memberProgress, overallProgress) = CalculateProgress(
            item.kpi.TargetPtSessions, actualSessions,
            item.kpi.TargetRevenue, actualRevenue,
            item.kpi.TargetNewMembers, 0,
            item.emp.Position);

        return new MyKpiDto(
            item.kpi.Id,
            item.tmpl.Name,
            month,
            year,
            item.kpi.TargetPtSessions,
            actualSessions,
            sessionProgress,
            item.kpi.TargetRevenue,
            actualRevenue,
            revenueProgress,
            item.kpi.TargetBonusAmount,
            overallProgress,
            overallProgress >= 100m ? KpiStatus.Achieved : KpiStatus.InProgress,
            item.kpi.Note
        );
    }

    private static (decimal SessionProgress, decimal RevenueProgress, decimal MemberProgress, decimal OverallProgress) CalculateProgress(
        int? targetSessions, int actualSessions,
        decimal targetRevenue, decimal actualRevenue,
        int? targetMembers, int actualMembers,
        string position)
    {
        decimal sessionProgress = targetSessions.HasValue && targetSessions.Value > 0
            ? Math.Round((decimal)actualSessions / targetSessions.Value * 100m, 1)
            : 0m;

        decimal revenueProgress = targetRevenue > 0
            ? Math.Round(actualRevenue / targetRevenue * 100m, 1)
            : 0m;

        decimal memberProgress = targetMembers.HasValue && targetMembers.Value > 0
            ? Math.Round((decimal)actualMembers / targetMembers.Value * 100m, 1)
            : 0m;

        var activeMetrics = new List<decimal>();
        if (targetSessions.HasValue && targetSessions.Value > 0) activeMetrics.Add(sessionProgress);
        if (targetRevenue > 0) activeMetrics.Add(revenueProgress);
        if (targetMembers.HasValue && targetMembers.Value > 0) activeMetrics.Add(memberProgress);

        decimal overallProgress = activeMetrics.Count > 0
            ? Math.Round(activeMetrics.Average(), 1)
            : 0m;

        return (sessionProgress, revenueProgress, memberProgress, overallProgress);
    }
}

