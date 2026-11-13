using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Payroll;
using GymX.Domain.Entities.Payroll;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class KpiTemplateRepository(ApplicationDbContext context)
    : GenericRepository<KpiTemplate>(context), IKpiTemplateRepository
{
    public Task<bool> ExistsNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        var query = DbSet.Where(t => !t.IsDeleted && t.Name == normalizedName);
        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }
        return query.AnyAsync(cancellationToken);
    }

    public async Task<List<KpiTemplateLookupDto>> GetLookupAsync(string? targetRole = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(t => !t.IsDeleted && t.Status == KpiTemplateStatus.Active);

        if (!string.IsNullOrWhiteSpace(targetRole) && targetRole.Trim().ToUpperInvariant() != "ALL")
        {
            var filterRole = targetRole.Trim().ToUpperInvariant();
            query = query.Where(t => t.TargetRole == filterRole);
        }

        return await query
            .OrderBy(t => t.Name)
            .Select(t => new KpiTemplateLookupDto(
                t.Id,
                t.Name,
                t.TargetRole,
                t.TargetPtSessions,
                t.TargetRevenue,
                t.TargetNewMembers,
                t.TargetBonusAmount))
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<KpiTemplateSummaryDto>> GetTemplatesPagedAsync(
        string? search,
        string? targetRole,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchKw = search.Trim();
            query = query.Where(t => EF.Functions.ILike(t.Name, $"%{searchKw}%"));
        }

        if (!string.IsNullOrWhiteSpace(targetRole) && targetRole.Trim().ToUpperInvariant() != "ALL")
        {
            var filterRole = targetRole.Trim().ToUpperInvariant();
            query = query.Where(t => t.TargetRole == filterRole);
        }

        if (!string.IsNullOrWhiteSpace(status) && status.Trim().ToUpperInvariant() != "ALL")
        {
            var filterStatus = status.Trim().ToUpperInvariant();
            query = query.Where(t => t.Status == filterStatus);
        }

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreatedDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new KpiTemplateSummaryDto(
                t.Id,
                t.Name,
                t.TargetRole,
                t.TargetPtSessions,
                t.TargetRevenue,
                t.TargetNewMembers,
                t.TargetBonusAmount,
                t.Description,
                t.Status,
                t.CreatedDate))
            .ToListAsync(cancellationToken);

        return new PagedResult<KpiTemplateSummaryDto>(items, totalRecords, pageIndex, pageSize);
    }

    public async Task<KpiTemplateDto?> GetTemplateDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await DbSet.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        if (template == null) return null;

        return new KpiTemplateDto(
            template.Id,
            template.Name,
            template.TargetRole,
            template.TargetPtSessions,
            template.TargetRevenue,
            template.TargetNewMembers,
            template.TargetBonusAmount,
            template.Description,
            template.Status,
            template.CreatedDate,
            template.LastModifiedDate);
    }
}

