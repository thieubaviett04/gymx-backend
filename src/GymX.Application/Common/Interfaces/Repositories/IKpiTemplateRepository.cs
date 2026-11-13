using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payroll;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IKpiTemplateRepository : IGenericRepository<KpiTemplate>
{
    Task<bool> ExistsNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<List<KpiTemplateLookupDto>> GetLookupAsync(string? targetRole = null, CancellationToken cancellationToken = default);
    Task<PagedResult<KpiTemplateSummaryDto>> GetTemplatesPagedAsync(string? search, string? targetRole, string? status, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<KpiTemplateDto?> GetTemplateDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

