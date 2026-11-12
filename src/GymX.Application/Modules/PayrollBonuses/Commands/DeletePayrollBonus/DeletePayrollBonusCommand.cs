using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PayrollBonuses.Commands.DeletePayrollBonus;

public sealed record DeletePayrollBonusCommand(Guid Id) : IRequest<Result<bool>>;
