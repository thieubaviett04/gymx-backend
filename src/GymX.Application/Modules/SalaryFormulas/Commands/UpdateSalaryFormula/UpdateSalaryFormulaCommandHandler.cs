using GymX.Application.Common.Exceptions;
using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.SalaryFormulas.Commands.UpdateSalaryFormula;

public sealed class UpdateSalaryFormulaCommandHandler(
    ISalaryFormulaRepository formulaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateSalaryFormulaCommand, Result<SalaryFormulaDto>>
{
    public async Task<Result<SalaryFormulaDto>> Handle(UpdateSalaryFormulaCommand request, CancellationToken cancellationToken)
    {
        var formula = await formulaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (formula == null)
        {
            return Result.Failure<SalaryFormulaDto>(SalaryFormulaErrors.NotFound(request.Id));
        }

        var isInUse = await formulaRepository.IsFormulaInUseAsync(formula.Id, cancellationToken);
        if (isInUse)
        {
            return Result.Failure<SalaryFormulaDto>(SalaryFormulaErrors.CannotModifyInUse);
        }

        var nameExists = await formulaRepository.ExistsNameAsync(request.Name, request.Id, cancellationToken);
        if (nameExists)
        {
            return Result.Failure<SalaryFormulaDto>(SalaryFormulaErrors.NameAlreadyExists);
        }

        formula.Update(
            request.Name,
            request.TargetRole,
            request.BaseRateType,
            request.BaseRate,
            request.StandardWorkDays,
            request.PricePerPtSession,
            request.SalesCommissionPercent,
            request.EffectiveDate,
            request.Description);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintException)
        {
            return Result.Failure<SalaryFormulaDto>(SalaryFormulaErrors.NameAlreadyExists);
        }

        var detail = await formulaRepository.GetFormulaDetailByIdAsync(formula.Id, cancellationToken);
        if (detail == null)
        {
            return Result.Failure<SalaryFormulaDto>(SalaryFormulaErrors.NotFound(formula.Id));
        }

        return Result.Success(detail);
    }
}
