using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Commands.AdjustPayslipKpiBonus;

public sealed record AdjustPayslipKpiBonusCommand(
    Guid PayslipId,
    decimal KpiBonusAmount,
    string? Note) : IRequest<Result<PayslipDetailDto>>;
