using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using MediatR;

namespace GymX.Application.Modules.PenaltyRules.Commands.DeletePenaltyRule;

public class DeletePenaltyRuleCommandHandler(
    IPenaltyRuleRepository penaltyRuleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePenaltyRuleCommand, Result>
{
    public async Task<Result> Handle(DeletePenaltyRuleCommand command, CancellationToken cancellationToken)
    {
        var penaltyRule = await penaltyRuleRepository.GetByIdAsync(command.RuleId, cancellationToken);
        if (penaltyRule is null || penaltyRule.IsDeleted)
        {
            return Result.Failure(Error.NotFound("PenaltyRule.NotFound", "Không tìm thấy thông tin quy định phạt."));
        }

        var isPenaltyRuleApplied = await penaltyRuleRepository.IsRuleAppliedAsync(penaltyRule.Id, cancellationToken);
        if (penaltyRule.Status == PenaltyRuleStatus.Applied && isPenaltyRuleApplied)
        {
            return Result.Failure(
                Error.Conflict("PenaltyRule.CannotDeleteInUse", "Quy định phạt này đã từng được áp dụng để phạt nhân viên nên không thể xóa. Bạn có muốn chuyển sang trạng thái 'Ngừng áp dụng' không?"));
        }

        penaltyRule.SoftDelete();
        penaltyRuleRepository.Update(penaltyRule);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
