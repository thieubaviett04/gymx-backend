using FluentValidation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PayrollBonuses.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PayrollBonuses.Commands.UpdatePayrollBonus;

public sealed record UpdatePayrollBonusCommand(
    Guid Id,
    decimal Amount,
    string Reason,
    string BonusType) : IRequest<Result<PayrollBonusDto>>;

public sealed class UpdatePayrollBonusCommandHandler(
    IPayrollBonusRepository bonusRepository) : IRequestHandler<UpdatePayrollBonusCommand, Result<PayrollBonusDto>>
{
    public async Task<Result<PayrollBonusDto>> Handle(
        UpdatePayrollBonusCommand request,
        CancellationToken cancellationToken)
    {
        return await bonusRepository.UpdateBonusAsync(
            request.Id,
            request.Amount,
            request.Reason,
            request.BonusType,
            cancellationToken);
    }
}

public sealed class UpdatePayrollBonusCommandValidator : AbstractValidator<UpdatePayrollBonusCommand>
{
    public UpdatePayrollBonusCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Số tiền thưởng không được nhỏ hơn 0.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Vui lòng nhập lý do / dịp khen thưởng.")
            .MaximumLength(500)
            .WithMessage("Lý do khen thưởng không vượt quá 500 ký tự.");
    }
}

