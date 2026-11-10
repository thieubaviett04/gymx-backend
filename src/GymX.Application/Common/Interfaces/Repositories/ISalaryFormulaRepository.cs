using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payroll;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface ISalaryFormulaRepository : IGenericRepository<SalaryFormula>
{
    Task<bool> ExistsNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<PagedResult<SalaryFormulaDto>> GetFormulasPagedAsync(
        string? search,
        string? targetRole,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<SalaryFormulaDto?> GetFormulaDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SalaryFormula?> GetActiveFormulaForRoleAsync(string role, DateOnly effectiveDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SalaryFormulaLookupDto>> GetLookupAsync(string? targetRole, CancellationToken cancellationToken = default);
    Task<bool> IsFormulaInUseAsync(Guid formulaId, CancellationToken cancellationToken = default);
}
