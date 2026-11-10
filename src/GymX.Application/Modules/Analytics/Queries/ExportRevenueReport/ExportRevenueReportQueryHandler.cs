using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Analytics.Queries.ExportRevenueReport;

public sealed class ExportRevenueReportQueryHandler(
    IFinancialAnalyticsRepository financialAnalyticsRepository) : IRequestHandler<ExportRevenueReportQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(ExportRevenueReportQuery request, CancellationToken cancellationToken)
    {
        var period = string.IsNullOrWhiteSpace(request.Period) ? "today" : request.Period.Trim().ToLowerInvariant();
        var fileBytes = await financialAnalyticsRepository.GenerateRevenueReportCsvAsync(
            period,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        return Result.Success(fileBytes);
    }
}
