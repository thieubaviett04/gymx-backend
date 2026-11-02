using GymX.Application.Modules.PenaltyRules.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payroll;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IPenaltyRuleRepository : IGenericRepository<PenaltyRule>
{
    Task<PenaltyRule?> GetByCodeAsync(string ruleCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsCodeAsync(string ruleCode, CancellationToken cancellationToken = default);
    Task<bool> IsRuleAppliedAsync(Guid ruleId, CancellationToken cancellationToken = default);
    Task<int> CountViolationsAsync(Guid ruleId, CancellationToken cancellationToken = default);
    Task<PagedResult<PenaltyRuleSummaryDto>> GetPenaltyRulesPagedAsync(
        string? search,
        string? severity,
        string? status,
        string? sortBy,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<PenaltyRuleDetailDto?> GetPenaltyRuleDetailByIdAsync(Guid ruleId, CancellationToken cancellationToken = default);
}
