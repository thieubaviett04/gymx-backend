using GymX.Application.Modules.PayrollBonuses.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PayrollBonuses.Queries.GetPayrollBonusesPaged;

public sealed record GetPayrollBonusesPagedQuery(
    int? Month,
    int? Year,
    string? Search,
    string? Position,
    string? BonusType,
    int PageIndex,
    int PageSize) : IRequest<Result<PayrollBonusPagedResultDto>>;
