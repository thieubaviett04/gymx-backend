using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Entities.Payment;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class InvoiceRepository(ApplicationDbContext context) : IInvoiceRepository
{
    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Invoices.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Invoice?> GetByOrderCodeAsync(long orderCode, CancellationToken cancellationToken = default)
    {
        return await context.Invoices.FirstOrDefaultAsync(x => x.OrderCode == orderCode, cancellationToken);
    }

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await context.Invoices.AddAsync(invoice, cancellationToken);
    }

    public async Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        context.Invoices.Update(invoice);
        await Task.CompletedTask;
    }
}
