using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Members.DTOs.Response;
using GymX.Domain.Entities.Members;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class MemberRepository(ApplicationDbContext context) : GenericRepository<Member>(context), IMemberRepository
{
    public Task<Member?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);

    public Task<MemberProfileDto?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .Where(m => m.UserId == userId && !m.IsDeleted)
            .Select(m => new MemberProfileDto
            {
                MemberCode = m.MemberCode,
                JoinDate = m.JoinDate,
                EmergencyContactName = m.EmergencyContactName,
                EmergencyContactPhone = m.EmergencyContactPhone,
                HealthNote = m.HealthNote,
                MemberStatus = m.MemberStatus
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
