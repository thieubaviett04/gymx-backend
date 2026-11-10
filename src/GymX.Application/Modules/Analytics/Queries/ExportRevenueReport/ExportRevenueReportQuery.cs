using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Analytics.Queries.ExportRevenueReport;

public sealed record ExportRevenueReportQuery(
    string Period = "today",
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    string Format = "excel") : IRequest<Result<byte[]>>;
