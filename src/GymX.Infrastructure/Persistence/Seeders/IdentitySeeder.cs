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

        // 2. Seed Admin User
        if (!await context.Users.AnyAsync(u => u.Email == "admin@gymx.vn"))
        {
            // Bcrypt hash for "Admin@123"
            var adminUser = User.CreateWithPassword("admin@gymx.vn", "Admin GymX", "$2a$11$0H0xVd/n2.VbV49d1.2zL.D1F82fG4d9E47c0.k6w8V1QvW41");
            adminUser.VerifyEmail();
            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            // Add Admin Role
            var adminRole = await context.Roles.FirstAsync(r => r.Code == "ADMIN");
            await context.UserRoles.AddAsync(UserRole.Create(adminUser.Id, adminRole.Id));
            
            // Add Employee record for Admin
            var adminEmployee = Employee.Create(adminUser.Id, "EMP0001", "ADMIN", 20000000, DateOnly.FromDateTime(DateTime.UtcNow));
            await context.Employees.AddAsync(adminEmployee);

            await context.SaveChangesAsync();
        }
    }
}
