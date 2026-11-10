using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Commands.CalculatePayroll;

public sealed record CalculatePayrollCommand(
    int Month,
    int Year) : IRequest<Result<PayrollPeriodDetailDto>>;
