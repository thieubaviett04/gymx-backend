using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.SalaryFormulas.Commands.ToggleSalaryFormulaStatus;

public sealed record ToggleSalaryFormulaStatusCommand(
    Guid Id,
    bool IsActive) : IRequest<Result<SalaryFormulaDto>>;
