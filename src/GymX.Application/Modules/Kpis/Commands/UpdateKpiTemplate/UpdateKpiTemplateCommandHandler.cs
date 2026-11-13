using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Kpis.Commands.UpdateKpiTemplate;

public sealed class UpdateKpiTemplateCommandHandler(
    IKpiTemplateRepository repository,
    IEmployeeKpiRepository employeeKpiRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateKpiTemplateCommand, Result<KpiTemplateDto>>
{
    public async Task<Result<KpiTemplateDto>> Handle(UpdateKpiTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (template == null || template.IsDeleted)
        {
            return Result.Failure<KpiTemplateDto>(KpiErrors.TemplateNotFound);
        }

        var exists = await repository.ExistsNameAsync(command.Name, command.Id, cancellationToken);
        if (exists)
        {
            return Result.Failure<KpiTemplateDto>(KpiErrors.TemplateNameExists);
        }

        template.Update(
            command.Name,
            command.TargetRole,
            command.TargetRevenue,
            command.TargetBonusAmount,
            command.TargetPtSessions,
            command.TargetNewMembers,
            command.Description,
            command.Status);

        var activeKpis = await employeeKpiRepository.GetActiveByTemplateIdAsync(template.Id, cancellationToken);
        foreach (var kpi in activeKpis)
        {
            kpi.UpdateTargets(
                command.TargetRevenue,
                command.TargetBonusAmount,
                command.TargetPtSessions,
                command.TargetNewMembers,
                kpi.Note);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new KpiTemplateDto(
            template.Id,
            template.Name,
            template.TargetRole,
            template.TargetPtSessions,
            template.TargetRevenue,
            template.TargetNewMembers,
            template.TargetBonusAmount,
            template.Description,
            template.Status,
            template.CreatedDate,
            template.LastModifiedDate));
    }
}

