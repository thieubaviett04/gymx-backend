using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Queries.GetPayslipById;

public sealed record GetPayslipByIdQuery(Guid Id) : IRequest<Result<PayslipDetailDto>>;
