using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Invoices.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Invoices.Queries.GetInvoicesPaged;

public sealed class GetInvoicesPagedQueryHandler(
    IInvoiceRepository invoiceRepository) : IRequestHandler<GetInvoicesPagedQuery, Result<PagedResult<InvoiceSummaryDto>>>
{
    public async Task<Result<PagedResult<InvoiceSummaryDto>>> Handle(GetInvoicesPagedQuery request, CancellationToken cancellationToken)
    {
        var result = await invoiceRepository.GetInvoicesPagedAsync(
            request.Search,
            request.Status,
            request.FromDate,
            request.ToDate,
            request.SortBy,
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        return Result.Success(result);
    }
}

