using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Features.Users.DTOs;
using GymX.Domain.Entities.Identity;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

/// <summary>
/// "Thợ chuyên biệt" cho User — kế thừa toàn bộ CRUD từ GenericRepository,
/// chỉ viết thêm các hàm đặc thù mà chỉ bảng User mới cần.
/// </summary>
public class UserRepository(ApplicationDbContext context) : GenericRepository<User>(context), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => Context.Users
        .Include(u => u.RefreshTokens)
        .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => Context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

    public Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
        => Context.Users
        .Include(u => u.RefreshTokens)
        .FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken);

    public async Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserProfileDto?> GetFullProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = from u in Context.Users.AsNoTracking()
                    where u.Id == userId
                    join m in Context.Members.AsNoTracking() on u.Id equals m.UserId into memberGroup
                    from m in memberGroup.DefaultIfEmpty()
                    join e in Context.Employees.AsNoTracking() on u.Id equals e.UserId into employeeGroup
                    from e in employeeGroup.DefaultIfEmpty()
                    select new UserProfileDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        FullName = u.FullName,
                        PhoneNumber = u.PhoneNumber,
                        Gender = u.Gender,
                        DateOfBirth = u.DateOfBirth,
                        Address = u.Address,
                        AvatarUrl = u.AvatarUrl,
                        Status = u.Status,
                        EmailVerifiedAt = u.EmailVerifiedAt,
                        Roles = u.UserRoles.Select(ur => ur.Role.Code).ToList(),
                        MemberInfo = m == null ? null : new MemberProfileDto
                        {
                            MemberCode = m.MemberCode,
                            JoinDate = m.JoinDate,
                            EmergencyContactName = m.EmergencyContactName,
                            EmergencyContactPhone = m.EmergencyContactPhone,
                            HealthNote = m.HealthNote,
                            MemberStatus = m.MemberStatus
                        },
                        EmployeeInfo = e == null ? null : new EmployeeProfileDto
                        {
                            EmployeeCode = e.EmployeeCode,
                            Position = e.Position,
                            HiredAt = e.HiredAt,
                            NationalId = e.NationalId,
                            EmploymentStatus = e.EmploymentStatus
                        }
                    };

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}
