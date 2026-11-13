using GymX.Application.Modules.Invoices.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Invoices.Commands.CreateInvoice;

public sealed record CreateInvoiceCommand(
    string CustomerName,
    string? CustomerPhone,
    string? MemberCode,
    decimal Amount,
    string PaymentMethod,
    string ReferenceType,
    DateOnly IssuedDate,
    string? Note) : IRequest<Result<InvoiceSummaryDto>>;

