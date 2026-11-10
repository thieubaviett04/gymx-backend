using GymX.Application.Modules.Analytics.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Analytics.Queries.GetRevenueAnalytics;

public sealed record GetRevenueAnalyticsQuery(
    string Period = "today",
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    string? Metric = "revenue") : IRequest<Result<RevenueAnalyticsDto>>;
