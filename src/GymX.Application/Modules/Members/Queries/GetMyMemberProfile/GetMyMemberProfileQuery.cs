using GymX.Application.Modules.Members.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Members.Queries.GetMyMemberProfile;

public record GetMyMemberProfileQuery() : IRequest<Result<MemberProfileDto>>;
