using GymX.Application.Modules.Invoices.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Finance;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default);
    Task<string> GenerateNextInvoiceNumberAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<InvoiceSummaryDto?> GetInvoiceSummaryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid?> GetMemberIdByCodeAsync(string memberCode, CancellationToken cancellationToken = default);
    Task<Guid?> GetMemberIdByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<PagedResult<InvoiceSummaryDto>> GetInvoicesPagedAsync(
        string? search,
        string? status,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? sortBy,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}
