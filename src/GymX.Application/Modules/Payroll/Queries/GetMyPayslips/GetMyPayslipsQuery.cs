using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Queries.GetMyPayslips;

public sealed record GetMyPayslipsQuery : IRequest<Result<IReadOnlyCollection<PayslipSummaryDto>>>;
