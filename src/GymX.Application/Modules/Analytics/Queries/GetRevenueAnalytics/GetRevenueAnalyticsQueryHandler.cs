using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Analytics.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Analytics.Queries.GetRevenueAnalytics;

public sealed class GetRevenueAnalyticsQueryHandler(
    IFinancialAnalyticsRepository financialAnalyticsRepository) : IRequestHandler<GetRevenueAnalyticsQuery, Result<RevenueAnalyticsDto>>
{
    public async Task<Result<RevenueAnalyticsDto>> Handle(GetRevenueAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var period = string.IsNullOrWhiteSpace(request.Period) ? "today" : request.Period.Trim().ToLowerInvariant();
        var analytics = await financialAnalyticsRepository.GetRevenueAnalyticsAsync(
            period,
            request.FromDate,
            request.ToDate,
            request.Metric,
            cancellationToken);

        return Result.Success(analytics);
    }
}
