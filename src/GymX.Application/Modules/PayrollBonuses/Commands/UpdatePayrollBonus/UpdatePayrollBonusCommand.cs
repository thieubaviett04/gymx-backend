using GymX.Application.Modules.PayrollBonuses.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PayrollBonuses.Commands.UpdatePayrollBonus;

public sealed record UpdatePayrollBonusCommand(
    Guid Id,
    decimal Amount,
    string Reason,
    string BonusType) : IRequest<Result<PayrollBonusDto>>;
