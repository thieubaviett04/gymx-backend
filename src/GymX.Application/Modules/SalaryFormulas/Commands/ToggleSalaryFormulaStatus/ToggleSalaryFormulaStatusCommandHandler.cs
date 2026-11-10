using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.SalaryFormulas.Commands.ToggleSalaryFormulaStatus;

public sealed class ToggleSalaryFormulaStatusCommandHandler(
    ISalaryFormulaRepository formulaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ToggleSalaryFormulaStatusCommand, Result<SalaryFormulaDto>>
{
    public async Task<Result<SalaryFormulaDto>> Handle(ToggleSalaryFormulaStatusCommand request, CancellationToken cancellationToken)
    {
        var formula = await formulaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (formula == null)
        {
            return Result.Failure<SalaryFormulaDto>(SalaryFormulaErrors.NotFound(request.Id));
        }

        if (request.IsActive)
        {
            formula.Activate();
        }
        else
        {
            formula.Deactivate();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var detail = await formulaRepository.GetFormulaDetailByIdAsync(formula.Id, cancellationToken);
        if (detail == null)
        {
            return Result.Failure<SalaryFormulaDto>(SalaryFormulaErrors.NotFound(formula.Id));
        }

        return Result.Success(detail);
    }
}
