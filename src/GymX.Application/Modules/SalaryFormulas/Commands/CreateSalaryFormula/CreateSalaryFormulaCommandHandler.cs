using GymX.Application.Common.Exceptions;
using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payroll;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.SalaryFormulas.Commands.CreateSalaryFormula;

public sealed class CreateSalaryFormulaCommandHandler(
    ISalaryFormulaRepository formulaRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateSalaryFormulaCommand, Result<SalaryFormulaDto>>
{
    public async Task<Result<SalaryFormulaDto>> Handle(CreateSalaryFormulaCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var currentUserId))
        {
            return Result.Failure<SalaryFormulaDto>(Error.Unauthorized("Auth.Unauthorized", "Người dùng chưa đăng nhập hoặc thông tin xác thực không hợp lệ."));
        }

        var nameExists = await formulaRepository.ExistsNameAsync(request.Name, null, cancellationToken);
        if (nameExists)
        {
            return Result.Failure<SalaryFormulaDto>(SalaryFormulaErrors.NameAlreadyExists);
        }

        var formula = SalaryFormula.Create(
            request.Name,
            request.TargetRole,
            request.BaseRateType,
            request.BaseRate,
            request.StandardWorkDays,
            request.PricePerPtSession,
            request.SalesCommissionPercent,
            request.EffectiveDate,
            currentUserId,
            request.Description);

        await formulaRepository.AddAsync(formula, cancellationToken);

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
