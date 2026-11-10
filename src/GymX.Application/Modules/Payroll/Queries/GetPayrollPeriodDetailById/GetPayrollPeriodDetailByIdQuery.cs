using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Queries.GetPayrollPeriodDetailById;

public sealed record GetPayrollPeriodDetailByIdQuery(Guid Id) : IRequest<Result<PayrollPeriodDetailDto>>;
