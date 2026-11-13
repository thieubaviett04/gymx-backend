using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Commands.DeleteKpiTemplate;

public sealed record DeleteKpiTemplateCommand(Guid Id) : IRequest<Result>;
