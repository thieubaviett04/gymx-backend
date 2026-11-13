namespace GymX.Application.Modules.Invoices.DTOs.Response;

public sealed record InvoiceSummaryDto(
    Guid Id,
    string InvoiceNumber,
    Guid? MemberId,
    string? MemberCode,
    string? CustomerName,
    string? CustomerPhone,
    string ReferenceType,
    Guid ReferenceId,
    decimal Subtotal,
    decimal TotalAmount,
    string Status,
    DateTime IssuedAt,
    DateTime? PaidAt,
    string? PaymentMethod,
    string? CreatorName);

