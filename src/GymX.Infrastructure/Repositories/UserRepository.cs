using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Employees.DTOs;
using GymX.Application.Modules.Members.DTOs;
using GymX.Application.Modules.Trainers.DTOs;
using GymX.Application.Modules.Users.DTOs.Response;
using GymX.Domain.Entities.Identity;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : GenericRepository<User>(context), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => Context.Users
        .IgnoreQueryFilters()
        .Include(u => u.RefreshTokens)
        .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => Context.Users
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

    public Task<User?> GetUserWithRolesByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => Context.Users
        .IgnoreQueryFilters()
        .Include(u => u.UserRoles)
        .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

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
        var user = await Context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null) return null;

        var roles = user.UserRoles.Select(ur => ur.Role.Code).ToList();

        var member = await Context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);

        var employee = await Context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

        MemberProfileDto? memberInfo = null;
        if (member is not null)
        {
            memberInfo = new MemberProfileDto
            {
                MemberCode = member.MemberCode,
                JoinDate = member.JoinDate,
                EmergencyContactName = member.EmergencyContactName,
                EmergencyContactPhone = member.EmergencyContactPhone,
                HealthNote = member.HealthNote,
                MemberStatus = member.MemberStatus
            };
        }

        EmployeeProfileDto? employeeInfo = null;
        TrainerProfileDto? trainerInfo = null;

        if (employee is not null && employee.Position != "ADMIN")
        {
            employeeInfo = new EmployeeProfileDto
            {
                EmployeeCode = employee.EmployeeCode,
                Position = employee.Position,
                HiredAt = employee.HiredAt,
                NationalId = employee.NationalId,
                EmploymentStatus = employee.EmploymentStatus
            };

            if (employee.Position == "TRAINER" || roles.Contains("TRAINER"))
            {
                var trainer = await Context.Trainers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.EmployeeId == employee.Id, cancellationToken);

                if (trainer is not null)
                {
                    var spec = await Context.Specializations
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Id == trainer.SpecializationId, cancellationToken);

                    var docs = await Context.TrainerDocuments
                        .AsNoTracking()
                        .Where(d => d.TrainerId == trainer.Id && !d.IsDeleted)
                        .Select(d => new TrainerDocumentDto
                        {
                            Id = d.Id,
                            DocumentType = d.DocumentType,
                            DocumentName = d.DocumentName,
                            FileUrl = d.FileUrl,
                            IssuingOrganization = d.IssuingOrganization,
                            DocumentNumber = d.DocumentNumber,
                            IssuedDate = d.IssuedDate,
                            ExpiryDate = d.ExpiryDate,
                            VerificationStatus = d.VerificationStatus,
                            RejectionReason = d.RejectionReason
                        })
                        .ToListAsync(cancellationToken);

                    trainerInfo = new TrainerProfileDto
                    {
                        Id = trainer.Id,
                        SpecializationName = spec?.Name ?? string.Empty,
                        ExperienceYears = trainer.ExperienceYears,
                        Description = trainer.Description,
                        RatingAvg = trainer.RatingAvg,
                        RatingCount = trainer.RatingCount,
                        Documents = docs
                    };
                }
            }
        }

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            AvatarUrl = user.AvatarUrl,
            Status = user.Status,
            EmailVerifiedAt = user.EmailVerifiedAt,
            Roles = roles,
            MemberInfo = memberInfo,
            EmployeeInfo = employeeInfo,
            TrainerInfo = trainerInfo
        };
    }
}
