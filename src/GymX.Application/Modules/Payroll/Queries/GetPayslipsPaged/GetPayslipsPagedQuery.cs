using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Queries.GetPayslipsPaged;

public sealed record GetPayslipsPagedQuery(
    Guid? PeriodId,
    Guid? EmployeeId,
    string? Status,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<PayslipSummaryDto>>>;
