using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Violations.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Violations.Queries.GetViolationById;

public class GetViolationByIdQueryHandler(IViolationRepository repository)
    : IRequestHandler<GetViolationByIdQuery, Result<ViolationDetailDto>>
{
    public async Task<Result<ViolationDetailDto>> Handle(GetViolationByIdQuery request, CancellationToken cancellationToken)
    {
        var detail = await repository.GetViolationDetailByIdAsync(request.Id, cancellationToken);
        if (detail == null)
        {
            return Result.Failure<ViolationDetailDto>(
                Error.NotFound("Violation.NotFound", "Biên bản vi phạm không tồn tại trong hệ thống."));
        }

        return Result.Success(detail);
    }
}

