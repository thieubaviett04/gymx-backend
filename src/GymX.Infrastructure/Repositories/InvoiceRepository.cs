using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Invoices.DTOs.Response;
using GymX.Domain.Common.Models;
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

    public async Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await context.Payments.AddAsync(payment, cancellationToken);
    }

    public async Task<string> GenerateNextInvoiceNumberAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var codePrefix = $"INV-{date:yyyyMMdd}-";

        var latestInvoiceNumber = await context.Invoices
            .AsNoTracking()
            .Where(x => x.InvoiceNumber.StartsWith(codePrefix))
            .OrderByDescending(x => x.InvoiceNumber)
            .Select(x => x.InvoiceNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var maxSequenceNumber = 0;
        if (latestInvoiceNumber != null)
        {
            var parts = latestInvoiceNumber.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out var sequence))
            {
                maxSequenceNumber = sequence;
            }
        }

        return $"{codePrefix}{(maxSequenceNumber + 1):D4}";
    }

    public async Task<InvoiceSummaryDto?> GetInvoiceSummaryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoiceQuery = from invoice in context.Invoices.AsNoTracking()
                           where invoice.Id == id
                           join member in context.Members.AsNoTracking() on invoice.MemberId equals member.Id into memberGroup
                           from member in memberGroup.DefaultIfEmpty()
                           join memberUser in context.Users.AsNoTracking() on member.UserId equals memberUser.Id into memberUserGroup
                           from memberUser in memberUserGroup.DefaultIfEmpty()
                           join creator in context.Users.AsNoTracking() on invoice.CreatorId equals creator.Id into creatorGroup
                           from creator in creatorGroup.DefaultIfEmpty()
                           join payment in context.Payments.AsNoTracking() on invoice.Id equals payment.InvoiceId into paymentGroup
                           from payment in paymentGroup.DefaultIfEmpty()
                           select new InvoiceSummaryDto(
                               invoice.Id,
                               invoice.InvoiceNumber,
                               invoice.MemberId,
                               member != null ? member.MemberCode : null,
                               !string.IsNullOrWhiteSpace(invoice.CustomerName)
                                   ? invoice.CustomerName
                                   : (memberUser != null ? memberUser.FullName : null),
                               !string.IsNullOrWhiteSpace(invoice.CustomerPhone)
                                   ? invoice.CustomerPhone
                                   : (memberUser != null ? memberUser.PhoneNumber : null),
                               invoice.ReferenceType,
                               invoice.ReferenceId,
                               invoice.Subtotal,
                               invoice.TotalAmount,
                               invoice.Status,
                               invoice.IssuedAt,
                               invoice.PaidAt,
                               payment != null ? payment.Method : null,
                               creator != null ? creator.FullName : null);

        return await invoiceQuery.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid?> GetMemberIdByCodeAsync(string memberCode, CancellationToken cancellationToken = default)
    {
        var normalized = memberCode.Trim().ToUpperInvariant();
        return await context.Members
            .AsNoTracking()
            .Where(m => m.MemberCode == normalized)
            .Select(m => (Guid?)m.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid?> GetMemberIdByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        var cleanPhone = phone.Trim().Replace(" ", "").Replace(".", "").Replace("-", "");
        var query = from m in context.Members.AsNoTracking()
                    join u in context.Users.AsNoTracking() on m.UserId equals u.Id
                    where u.PhoneNumber != null && u.PhoneNumber.Replace(" ", "").Replace(".", "").Replace("-", "") == cleanPhone
                    select (Guid?)m.Id;

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<InvoiceSummaryDto>> GetInvoicesPagedAsync(
        string? search,
        string? status,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? sortBy,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var invoiceQuery = from invoice in context.Invoices.AsNoTracking()
                           join member in context.Members.AsNoTracking() on invoice.MemberId equals member.Id into memberGroup
                           from member in memberGroup.DefaultIfEmpty()
                           join memberUser in context.Users.AsNoTracking() on member.UserId equals memberUser.Id into memberUserGroup
                           from memberUser in memberUserGroup.DefaultIfEmpty()
                           join creator in context.Users.AsNoTracking() on invoice.CreatorId equals creator.Id into creatorGroup
                           from creator in creatorGroup.DefaultIfEmpty()
                           join payment in context.Payments.AsNoTracking() on invoice.Id equals payment.InvoiceId into paymentGroup
                           from payment in paymentGroup.DefaultIfEmpty()
                           select new
                           {
                               invoice.Id,
                               invoice.InvoiceNumber,
                               invoice.MemberId,
                               MemberCode = member != null ? member.MemberCode : null,
                               CustomerName = !string.IsNullOrWhiteSpace(invoice.CustomerName)
                                   ? invoice.CustomerName
                                   : (memberUser != null ? memberUser.FullName : null),
                               CustomerPhone = !string.IsNullOrWhiteSpace(invoice.CustomerPhone)
                                   ? invoice.CustomerPhone
                                   : (memberUser != null ? memberUser.PhoneNumber : null),
                               invoice.ReferenceType,
                               invoice.ReferenceId,
                               invoice.Subtotal,
                               invoice.TotalAmount,
                               invoice.Status,
                               invoice.IssuedAt,
                               invoice.PaidAt,
                               PaymentMethod = payment != null ? payment.Method : null,
                               CreatorName = creator != null ? creator.FullName : null
                           };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();
            invoiceQuery = invoiceQuery.Where(x =>
                x.InvoiceNumber.ToLower().Contains(normalizedSearch) ||
                (x.CustomerName != null && x.CustomerName.ToLower().Contains(normalizedSearch)) ||
                (x.CustomerPhone != null && x.CustomerPhone.Contains(normalizedSearch)) ||
                (x.MemberCode != null && x.MemberCode.ToLower().Contains(normalizedSearch)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            invoiceQuery = invoiceQuery.Where(x => x.Status == normalizedStatus);
        }

        if (fromDate.HasValue)
        {
            var fromDateTime = fromDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            invoiceQuery = invoiceQuery.Where(x => x.IssuedAt >= fromDateTime);
        }

        if (toDate.HasValue)
        {
            var toDateTime = toDate.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            invoiceQuery = invoiceQuery.Where(x => x.IssuedAt <= toDateTime);
        }

        invoiceQuery = sortBy?.Trim().ToLowerInvariant() switch
        {
            "date_asc" => invoiceQuery.OrderBy(x => x.IssuedAt),
            "amount_desc" => invoiceQuery.OrderByDescending(x => x.TotalAmount),
            "amount_asc" => invoiceQuery.OrderBy(x => x.TotalAmount),
            _ => invoiceQuery.OrderByDescending(x => x.IssuedAt)
        };

        var totalCount = await invoiceQuery.CountAsync(cancellationToken);

        var rawItems = await invoiceQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(x => new InvoiceSummaryDto(
            x.Id,
            x.InvoiceNumber,
            x.MemberId,
            x.MemberCode,
            x.CustomerName,
            x.CustomerPhone,
            x.ReferenceType,
            x.ReferenceId,
            x.Subtotal,
            x.TotalAmount,
            x.Status,
            x.IssuedAt,
            x.PaidAt,
            x.PaymentMethod,
            x.CreatorName)).ToList();

        return new PagedResult<InvoiceSummaryDto>(items, totalCount, pageIndex, pageSize);
    }
}
