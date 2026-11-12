using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PayrollBonuses.Commands.CreatePayrollBonus;

public sealed record CreatePayrollBonusCommand(
    int Month,
    int Year,
    string BonusType,
    string TargetType,
    string? Position,
    List<Guid>? EmployeeIds,
    decimal Amount,
    string Reason) : IRequest<Result<int>>;
