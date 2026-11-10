using GymX.Application.Modules.Analytics.DTOs.Response;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IFinancialAnalyticsRepository
{
    Task<RevenueAnalyticsDto> GetRevenueAnalyticsAsync(
        string period,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? metric,
        CancellationToken cancellationToken = default);

    Task<byte[]> GenerateRevenueReportCsvAsync(
        string period,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default);
}
