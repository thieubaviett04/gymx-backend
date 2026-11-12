using GymX.Application.Modules.PayrollBonuses.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PayrollBonuses.Queries.GetBonusEmployeesLookup;

public sealed record GetBonusEmployeesLookupQuery(
    int? Month,
    int? Year,
    string? Search) : IRequest<Result<IReadOnlyCollection<BonusEmployeeLookupDto>>>;
