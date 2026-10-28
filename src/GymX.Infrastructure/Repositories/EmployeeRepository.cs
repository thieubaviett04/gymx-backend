using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Employees.DTOs;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Employees;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class EmployeeRepository(ApplicationDbContext context) : GenericRepository<Employee>(context), IEmployeeRepository
{
    public Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

    public Task<Employee?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
        => DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.NationalId == nationalId, cancellationToken);

    public Task<Employee?> GetByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken = default)
        => DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode, cancellationToken);

    public async Task<string> GenerateNextEmployeeCodeAsync(CancellationToken cancellationToken = default)
    {
        var latestCode = await DbSet
            .IgnoreQueryFilters()
            .Where(e => e.EmployeeCode.StartsWith("NV"))
            .OrderByDescending(e => e.EmployeeCode)
            .Select(e => e.EmployeeCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (!string.IsNullOrEmpty(latestCode) && int.TryParse(latestCode[2..], out var num))
        {
            return $"NV{(num + 1):D4}";
        }

        var latestEmpCode = await DbSet
            .IgnoreQueryFilters()
            .Where(e => e.EmployeeCode.StartsWith("EMP"))
            .OrderByDescending(e => e.EmployeeCode)
            .Select(e => e.EmployeeCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (!string.IsNullOrEmpty(latestEmpCode) && int.TryParse(latestEmpCode[3..], out var empNum))
        {
            return $"NV{(empNum + 1):D4}";
        }

        return "NV0001";
    }

    public async Task<PagedResult<EmployeeSummaryDto>> GetEmployeesPagedAsync(
        string? search,
        string? position,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Employees
            .AsNoTracking()
            .Where(e => !e.IsDeleted);

        if (!string.IsNullOrWhiteSpace(position))
        {
            var pos = position.Trim().ToUpperInvariant();
            query = query.Where(e => e.Position == pos);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var stat = status.Trim().ToUpperInvariant();
            query = query.Where(e => e.EmploymentStatus == stat);
        }

        var combinedQuery = from e in query
            join u in Context.Users.AsNoTracking() on e.UserId equals u.Id
            join t in Context.Trainers.AsNoTracking() on e.Id equals t.EmployeeId into trainers
            from t in trainers.DefaultIfEmpty()
            join s in Context.Specializations.AsNoTracking() on t.SpecializationId equals s.Id into specs
            from s in specs.DefaultIfEmpty()
            select new
            {
                Employee = e,
                User = u,
                Trainer = t,
                SpecializationName = s != null ? s.Name : null
            };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            combinedQuery = combinedQuery.Where(x =>
                x.Employee.EmployeeCode.ToLower().Contains(keyword) ||
                x.User.FullName.ToLower().Contains(keyword) ||
                x.User.Email.ToLower().Contains(keyword) ||
                (x.User.PhoneNumber != null && x.User.PhoneNumber.Contains(keyword)) ||
                (x.Employee.NationalId != null && x.Employee.NationalId.Contains(keyword)));
        }

        var totalCount = await combinedQuery.LongCountAsync(cancellationToken);

        var items = await combinedQuery
            .OrderByDescending(x => x.Employee.CreatedDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new EmployeeSummaryDto
            {
                Id = x.Employee.Id,
                UserId = x.User.Id,
                EmployeeCode = x.Employee.EmployeeCode,
                FullName = x.User.FullName,
                Email = x.User.Email,
                PhoneNumber = x.User.PhoneNumber,
                NationalId = x.Employee.NationalId,
                Position = x.Employee.Position,
                BaseSalaryRate = x.Employee.BaseSalaryRate,
                HiredAt = x.Employee.HiredAt,
                EmploymentStatus = x.Employee.EmploymentStatus,
                AvatarUrl = x.User.AvatarUrl,
                TrainerSpecialization = x.SpecializationName,
                TrainerExperienceYears = x.Trainer != null ? (short?)x.Trainer.ExperienceYears : null,
                CreatedDate = x.Employee.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<EmployeeSummaryDto>(items, totalCount, pageIndex, pageSize);
    }

    public async Task<EmployeeDetailDto?> GetEmployeeDetailByIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var query = from e in Context.Employees.AsNoTracking()
            where e.Id == employeeId && !e.IsDeleted
            join u in Context.Users.AsNoTracking() on e.UserId equals u.Id
            join t in Context.Trainers.AsNoTracking() on e.Id equals t.EmployeeId into trainers
            from t in trainers.DefaultIfEmpty()
            join s in Context.Specializations.AsNoTracking() on t.SpecializationId equals s.Id into specs
            from s in specs.DefaultIfEmpty()
            select new EmployeeDetailDto
            {
                Id = e.Id,
                UserId = u.Id,
                EmployeeCode = e.EmployeeCode,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                NationalId = e.NationalId,
                Gender = u.Gender,
                DateOfBirth = u.DateOfBirth,
                Address = u.Address,
                AvatarUrl = u.AvatarUrl,
                Position = e.Position,
                BaseSalaryRate = e.BaseSalaryRate,
                HiredAt = e.HiredAt,
                EmploymentStatus = e.EmploymentStatus,
                IsDeleted = e.IsDeleted,
                CreatedDate = e.CreatedDate,
                TrainerInfo = t != null ? new TrainerDetailInfoDto
                {
                    TrainerId = t.Id,
                    SpecializationId = t.SpecializationId,
                    SpecializationName = s != null ? s.Name : string.Empty,
                    ExperienceYears = t.ExperienceYears,
                    SalaryCoefficient = t.SalaryCoefficient,
                    Description = t.Description,
                    RatingAvg = t.RatingAvg,
                    RatingCount = t.RatingCount
                } : null
            };

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}
