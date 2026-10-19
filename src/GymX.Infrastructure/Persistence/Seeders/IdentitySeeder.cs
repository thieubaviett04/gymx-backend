using GymX.Domain.Constants;
using GymX.Domain.Entities.Employees;
using GymX.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Persistence.Seeders;

public static class IdentitySeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // 1. Seed Roles
        if (!await context.Roles.AnyAsync())
        {
            await context.Roles.AddRangeAsync(
                Role.Create("ADMIN", "Quản trị viên"),
                Role.Create("RECEPTIONIST", "Lễ tân"),
                Role.Create("TRAINER", "Huấn luyện viên"),
                Role.Create("MEMBER", "Hội viên")
            );
            await context.SaveChangesAsync();
        }

        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@gymx.vn");
        if (adminUser is null)
        {
            adminUser = User.CreateWithPassword("admin@gymx.vn", "Admin GymX", BCrypt.Net.BCrypt.HashPassword("Admin@123", 11));
            
            adminUser.UpdateProfile("0987654321", "MALE", new DateOnly(1990, 1, 1), "Hà Nội, Việt Nam");
            adminUser.VerifyEmail();
            
            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            var adminRole = await context.Roles.FirstAsync(r => r.Code == "ADMIN");
            await context.UserRoles.AddAsync(UserRole.Create(adminUser.Id, adminRole.Id));
            
            var adminEmployee = Employee.Create(adminUser.Id, "EMP0001", "ADMIN", 20000000, DateOnly.FromDateTime(DateTime.UtcNow));
            await context.Employees.AddAsync(adminEmployee);

            await context.SaveChangesAsync();
        }
        else
        {
            adminUser.UpdatePassword(BCrypt.Net.BCrypt.HashPassword("Admin@123", 11));
            await context.SaveChangesAsync();
        }
    }
}
