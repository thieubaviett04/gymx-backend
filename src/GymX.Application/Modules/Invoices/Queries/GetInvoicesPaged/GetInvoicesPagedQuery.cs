using GymX.Application.Modules.Invoices.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Invoices.Queries.GetInvoicesPaged;

public sealed record GetInvoicesPagedQuery(
    string? Search,
    string? Status,
    DateOnly? FromDate,
    DateOnly? ToDate,
    string? SortBy,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<InvoiceSummaryDto>>>;

