using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Payroll;
using GymX.Domain.Entities.Payroll;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class SalaryFormulaRepository(ApplicationDbContext context)
    : GenericRepository<SalaryFormula>(context), ISalaryFormulaRepository
{
    public async Task<bool> ExistsNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        var query = Context.SalaryFormulas.AsNoTracking()
            .Where(f => EF.Functions.ILike(f.Name, normalizedName));

        if (excludeId.HasValue && excludeId.Value != Guid.Empty)
        {
            query = query.Where(f => f.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<PagedResult<SalaryFormulaDto>> GetFormulasPagedAsync(
        string? search,
        string? targetRole,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var formulaQuery = from formula in Context.SalaryFormulas.AsNoTracking()
                           join user in Context.Users.AsNoTracking() on formula.CreatorId equals user.Id
                           select new
                           {
                               Formula = formula,
                               Creator = user
                           };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchKeyword = search.Trim();
            formulaQuery = formulaQuery.Where(item =>
                EF.Functions.ILike(item.Formula.Name, $"%{searchKeyword}%") ||
                (item.Formula.Description != null && EF.Functions.ILike(item.Formula.Description, $"%{searchKeyword}%")));
        }

        if (!string.IsNullOrWhiteSpace(targetRole))
        {
            var normalizedRole = targetRole.Trim().ToUpperInvariant();
            formulaQuery = formulaQuery.Where(item => item.Formula.TargetRole == normalizedRole);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            formulaQuery = formulaQuery.Where(item => item.Formula.Status == normalizedStatus);
        }

        var orderedQuery = formulaQuery.OrderByDescending(item => item.Formula.EffectiveDate)
            .ThenByDescending(item => item.Formula.CreatedDate);

        var totalCount = await orderedQuery.CountAsync(cancellationToken);

        var items = await orderedQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new SalaryFormulaDto
            {
                Id = item.Formula.Id,
                Name = item.Formula.Name,
                TargetRole = item.Formula.TargetRole,
                BaseRateType = item.Formula.BaseRateType,
                BaseRate = item.Formula.BaseRate,
                StandardWorkDays = item.Formula.StandardWorkDays,
                PricePerPtSession = item.Formula.PricePerPtSession,
                SalesCommissionPercent = item.Formula.SalesCommissionPercent,
                EffectiveDate = item.Formula.EffectiveDate,
                Status = item.Formula.Status,
                Description = item.Formula.Description,
                CreatorName = item.Creator.FullName,
                CreatedAt = item.Formula.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SalaryFormulaDto>(items, totalCount, pageIndex, pageSize);
    }

    public async Task<SalaryFormulaDto?> GetFormulaDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var formulaQuery = from formula in Context.SalaryFormulas.AsNoTracking()
                           where formula.Id == id
                           join user in Context.Users.AsNoTracking() on formula.CreatorId equals user.Id
                           select new SalaryFormulaDto
                           {
                               Id = formula.Id,
                               Name = formula.Name,
                               TargetRole = formula.TargetRole,
                               BaseRateType = formula.BaseRateType,
                               BaseRate = formula.BaseRate,
                               StandardWorkDays = formula.StandardWorkDays,
                               PricePerPtSession = formula.PricePerPtSession,
                               SalesCommissionPercent = formula.SalesCommissionPercent,
                               EffectiveDate = formula.EffectiveDate,
                               Status = formula.Status,
                               Description = formula.Description,
                               CreatorName = user.FullName,
                               CreatedAt = formula.CreatedDate
                           };

        return await formulaQuery.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SalaryFormula?> GetActiveFormulaForRoleAsync(string role, DateOnly effectiveDate, CancellationToken cancellationToken = default)
    {
        var normalizedRole = role.Trim().ToUpperInvariant();

        var formula = await Context.SalaryFormulas
            .Where(f => (f.TargetRole == normalizedRole || f.TargetRole == "ALL") &&
                        f.Status == EmployeeStatus.Active &&
                        f.EffectiveDate <= effectiveDate)
            .OrderByDescending(f => f.TargetRole == normalizedRole ? 1 : 0)
            .ThenByDescending(f => f.EffectiveDate)
            .FirstOrDefaultAsync(cancellationToken);

        return formula;
    }

    public async Task<IReadOnlyCollection<SalaryFormulaLookupDto>> GetLookupAsync(string? targetRole, CancellationToken cancellationToken = default)
    {
        var query = Context.SalaryFormulas.AsNoTracking()
            .Where(f => f.Status == EmployeeStatus.Active);

        if (!string.IsNullOrWhiteSpace(targetRole))
        {
            var normalizedRole = targetRole.Trim().ToUpperInvariant();
            query = query.Where(f => f.TargetRole == normalizedRole || f.TargetRole == "ALL");
        }

        return await query
            .OrderBy(f => f.Name)
            .Select(f => new SalaryFormulaLookupDto
            {
                Id = f.Id,
                Name = f.Name,
                TargetRole = f.TargetRole,
                BaseRate = f.BaseRate,
                EffectiveDate = f.EffectiveDate
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsFormulaInUseAsync(Guid formulaId, CancellationToken cancellationToken = default)
    {
        return await Context.PayrollPeriods.AsNoTracking()
            .AnyAsync(p => p.SalaryFormulaId == formulaId && p.Status == PayrollPeriodStatus.Confirmed, cancellationToken);
    }
}
