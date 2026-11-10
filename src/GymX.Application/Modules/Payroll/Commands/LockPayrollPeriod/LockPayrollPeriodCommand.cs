using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Commands.LockPayrollPeriod;

public sealed record LockPayrollPeriodCommand(Guid PeriodId) : IRequest<Result<PayrollPeriodDetailDto>>;
