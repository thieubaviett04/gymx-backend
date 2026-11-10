using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Entities.Payroll;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class PenaltyRuleRepository(ApplicationDbContext context)
    : GenericRepository<PenaltyRule>(context), IPenaltyRuleRepository
{
    public Task<PenaltyRule?> GetByCodeAsync(string ruleCode, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(penaltyRule => penaltyRule.RuleCode == ruleCode && !penaltyRule.IsDeleted, cancellationToken);

    public Task<bool> ExistsCodeAsync(string ruleCode, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(penaltyRule => penaltyRule.RuleCode == ruleCode && !penaltyRule.IsDeleted, cancellationToken);

    public async Task<bool> IsRuleAppliedAsync(Guid ruleId, CancellationToken cancellationToken = default)
    {
        return await Context.Violations.AnyAsync(violation =>
            violation.PenaltyRuleId == ruleId,
            cancellationToken);
    }

    public async Task<int> CountViolationsAsync(Guid ruleId, CancellationToken cancellationToken = default)
    {
        return await Context.Violations.CountAsync(violation =>
            violation.PenaltyRuleId == ruleId,
            cancellationToken);
    }

    public async Task<PagedResult<PenaltyRuleSummaryDto>> GetPenaltyRulesPagedAsync(
        string? search,
        string? severity,
        string? status,
        string? targetAudience,
        string? sortBy,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var penaltyRulesQuery = DbSet.AsNoTracking().Where(penaltyRule => !penaltyRule.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchKeyword = search.Trim();
            penaltyRulesQuery = penaltyRulesQuery.Where(penaltyRule =>
                EF.Functions.ILike(penaltyRule.RuleCode, $"%{searchKeyword}%") ||
                EF.Functions.ILike(penaltyRule.Name, $"%{searchKeyword}%"));
        }

        if (!string.IsNullOrWhiteSpace(severity) && severity.Trim().ToUpperInvariant() != "ALL")
        {
            var filterSeverity = severity.Trim().ToUpperInvariant();
            penaltyRulesQuery = penaltyRulesQuery.Where(penaltyRule => penaltyRule.SeverityLevel == filterSeverity);
        }

        if (!string.IsNullOrWhiteSpace(status) && status.Trim().ToUpperInvariant() != "ALL")
        {
            var filterStatus = status.Trim().ToUpperInvariant();
            penaltyRulesQuery = penaltyRulesQuery.Where(penaltyRule => penaltyRule.Status == filterStatus);
        }

        if (!string.IsNullOrWhiteSpace(targetAudience) && targetAudience.Trim().ToUpperInvariant() != "ALL")
        {
            var filterAudience = targetAudience.Trim().ToUpperInvariant();
            penaltyRulesQuery = penaltyRulesQuery.Where(penaltyRule => penaltyRule.ViolationType == filterAudience);
        }

        penaltyRulesQuery = sortBy?.Trim().ToLower() switch
        {
            "fine_asc" or "fine-asc" => penaltyRulesQuery.OrderBy(penaltyRule => penaltyRule.FineAmount),
            "fine_desc" or "fine-desc" => penaltyRulesQuery.OrderByDescending(penaltyRule => penaltyRule.FineAmount),
            "date_asc" or "date-asc" => penaltyRulesQuery.OrderBy(penaltyRule => penaltyRule.EffectiveFrom),
            "date_desc" or "date-desc" => penaltyRulesQuery.OrderByDescending(penaltyRule => penaltyRule.EffectiveFrom),
            _ => penaltyRulesQuery.OrderByDescending(penaltyRule => penaltyRule.CreatedDate)
        };

        var totalCount = await penaltyRulesQuery.LongCountAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));

        var penaltyRuleSummaries = await penaltyRulesQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(penaltyRule => new PenaltyRuleSummaryDto
            {
                Id = penaltyRule.Id,
                RuleCode = penaltyRule.RuleCode,
                Name = penaltyRule.Name,
                ViolationType = penaltyRule.ViolationType,
                FineAmount = penaltyRule.FineAmount,
                SeverityLevel = penaltyRule.SeverityLevel,
                Status = penaltyRule.Status == PenaltyRuleStatus.Pending && penaltyRule.EffectiveDate <= today
                    ? PenaltyRuleStatus.Active
                    : penaltyRule.Status,
                EffectiveDate = penaltyRule.EffectiveDate,
                EffectiveFrom = penaltyRule.EffectiveFrom,
                Description = penaltyRule.Description,
                ViolationsCount = Context.Violations.Count(violation => violation.PenaltyRuleId == penaltyRule.Id),
                CreatedDate = penaltyRule.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PenaltyRuleSummaryDto>(penaltyRuleSummaries, totalCount, pageIndex, pageSize);
    }

    public async Task<PenaltyRuleDetailDto?> GetPenaltyRuleDetailByIdAsync(Guid ruleId, CancellationToken cancellationToken = default)
    {
        var penaltyRule = await DbSet.AsNoTracking()
            .Include(rule => rule.PreviousRule)
            .FirstOrDefaultAsync(rule => rule.Id == ruleId && !rule.IsDeleted, cancellationToken);

        if (penaltyRule is null)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
        var appliedViolationsCount = await Context.Violations.CountAsync(violation => violation.PenaltyRuleId == penaltyRule.Id, cancellationToken);

        return new PenaltyRuleDetailDto
        {
            Id = penaltyRule.Id,
            RuleCode = penaltyRule.RuleCode,
            Name = penaltyRule.Name,
            ViolationType = penaltyRule.ViolationType,
            FineAmount = penaltyRule.FineAmount,
            SeverityLevel = penaltyRule.SeverityLevel,
            Status = penaltyRule.Status == PenaltyRuleStatus.Pending && penaltyRule.EffectiveDate <= today
                ? PenaltyRuleStatus.Active
                : penaltyRule.Status,
            EffectiveDate = penaltyRule.EffectiveDate,
            EffectiveFrom = penaltyRule.EffectiveFrom,
            Description = penaltyRule.Description,
            PreviousRuleId = penaltyRule.PreviousRuleId,
            PreviousRuleCode = penaltyRule.PreviousRule?.RuleCode,
            ViolationsCount = appliedViolationsCount,
            CreatedDate = penaltyRule.CreatedDate,
            LastModifiedDate = penaltyRule.LastModifiedDate
        };
    }

    public async Task<List<PenaltyRuleLookupDto>> GetLookupAsync(string? targetAudience = null, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));

        var query = DbSet
            .AsNoTracking()
            .Where(rule => !rule.IsDeleted &&
                (rule.Status == PenaltyRuleStatus.Active ||
                (rule.Status == PenaltyRuleStatus.Pending && rule.EffectiveDate <= today)));

        if (!string.IsNullOrWhiteSpace(targetAudience) && targetAudience.Trim().ToUpperInvariant() != "ALL")
        {
            var filterAudience = targetAudience.Trim().ToUpperInvariant();
            query = query.Where(rule => rule.ViolationType == filterAudience || rule.ViolationType == RuleTargetAudience.General);
        }

        return await query
            .OrderBy(rule => rule.Name)
            .Select(rule => new PenaltyRuleLookupDto(
                rule.Id,
                rule.RuleCode,
                rule.Name,
                rule.FineAmount,
                rule.SeverityLevel,
                rule.ViolationType,
                rule.EffectiveDate,
                rule.Description
            ))
            .ToListAsync(cancellationToken);
    }
}
