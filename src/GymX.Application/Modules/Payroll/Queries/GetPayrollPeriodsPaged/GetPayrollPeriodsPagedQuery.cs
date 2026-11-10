using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Queries.GetPayrollPeriodsPaged;

public sealed record GetPayrollPeriodsPagedQuery(
    int? Year,
    string? Status,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<PayrollPeriodSummaryDto>>>;
