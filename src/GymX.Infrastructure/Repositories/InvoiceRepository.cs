using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Entities.Finance;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class InvoiceRepository(ApplicationDbContext context) : IInvoiceRepository
{
    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Invoices.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await context.Invoices.FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber, cancellationToken);
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
