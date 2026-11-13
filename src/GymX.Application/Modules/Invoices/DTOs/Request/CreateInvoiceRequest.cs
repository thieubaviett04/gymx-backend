namespace GymX.Application.Modules.Invoices.DTOs.Request;

public sealed record CreateInvoiceRequest(
    string CustomerName,
    string? CustomerPhone,
    string? MemberCode,
    decimal Amount,
    string PaymentMethod,
    string ReferenceType,
    DateOnly IssuedDate,
    string? Note);

