using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Kpis.Commands.DeleteKpiTemplate;

public sealed class DeleteKpiTemplateCommandHandler(
    IKpiTemplateRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteKpiTemplateCommand, Result>
{
    public async Task<Result> Handle(DeleteKpiTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (template == null || template.IsDeleted)
        {
            return Result.Failure(KpiErrors.TemplateNotFound);
        }

        template.SoftDelete();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

