using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payroll;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Kpis.Commands.CreateKpiTemplate;

public sealed class CreateKpiTemplateCommandHandler(
    IKpiTemplateRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateKpiTemplateCommand, Result<KpiTemplateDto>>
{
    public async Task<Result<KpiTemplateDto>> Handle(CreateKpiTemplateCommand command, CancellationToken cancellationToken)
    {
        var exists = await repository.ExistsNameAsync(command.Name, null, cancellationToken);
        if (exists)
        {
            return Result.Failure<KpiTemplateDto>(KpiErrors.TemplateNameExists);
        }

        var template = KpiTemplate.Create(
            command.Name,
            command.TargetRole,
            command.TargetRevenue,
            command.TargetBonusAmount,
            command.TargetPtSessions,
            command.TargetNewMembers,
            command.Description,
            command.Status);

        await repository.AddAsync(template, cancellationToken);
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

