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

        // 2. Seed Receptionist
        var recUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "receptionist@gymx.vn");
        if (recUser is null)
        {
            recUser = User.CreateWithPassword("receptionist@gymx.vn", "Lễ Tân GymX", BCrypt.Net.BCrypt.HashPassword("Receptionist@123", 11));
            recUser.UpdateProfile("0987654322", "FEMALE", new DateOnly(1995, 5, 10), "Hà Nội, Việt Nam");
            recUser.VerifyEmail();
            await context.Users.AddAsync(recUser);
            await context.SaveChangesAsync();

            var recRole = await context.Roles.FirstAsync(r => r.Code == "RECEPTIONIST");
            await context.UserRoles.AddAsync(UserRole.Create(recUser.Id, recRole.Id));

            var recEmployee = Employee.Create(recUser.Id, "EMP0002", "RECEPTIONIST", 10000000, DateOnly.FromDateTime(DateTime.UtcNow));
            await context.Employees.AddAsync(recEmployee);
            await context.SaveChangesAsync();
        }
        else
        {
            recUser.UpdatePassword(BCrypt.Net.BCrypt.HashPassword("Receptionist@123", 11));
            await context.SaveChangesAsync();
        }

        // 3. Seed Trainer
        var trainerUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "trainer@gymx.vn");
        if (trainerUser is null)
        {
            trainerUser = User.CreateWithPassword("trainer@gymx.vn", "HLV GymX", BCrypt.Net.BCrypt.HashPassword("Trainer@123", 11));
            trainerUser.UpdateProfile("0987654323", "MALE", new DateOnly(1993, 8, 15), "Hà Nội, Việt Nam");
            trainerUser.VerifyEmail();
            await context.Users.AddAsync(trainerUser);
            await context.SaveChangesAsync();

            var trainerRole = await context.Roles.FirstAsync(r => r.Code == "TRAINER");
            await context.UserRoles.AddAsync(UserRole.Create(trainerUser.Id, trainerRole.Id));

            var trainerEmployee = Employee.Create(trainerUser.Id, "EMP0003", "TRAINER", 15000000, DateOnly.FromDateTime(DateTime.UtcNow));
            await context.Employees.AddAsync(trainerEmployee);
            await context.SaveChangesAsync();
        }
        else
        {
            trainerUser.UpdatePassword(BCrypt.Net.BCrypt.HashPassword("Trainer@123", 11));
            await context.SaveChangesAsync();
        }

        // 4. Seed Member
        var memberUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "member@gymx.vn");
        if (memberUser is null)
        {
            memberUser = User.CreateWithPassword("member@gymx.vn", "Hội Viên GymX", BCrypt.Net.BCrypt.HashPassword("Member@123", 11));
            memberUser.UpdateProfile("0987654324", "MALE", new DateOnly(1998, 12, 20), "Hà Nội, Việt Nam");
            memberUser.VerifyEmail();
            await context.Users.AddAsync(memberUser);
            await context.SaveChangesAsync();

            var memberRole = await context.Roles.FirstAsync(r => r.Code == "MEMBER");
            await context.UserRoles.AddAsync(UserRole.Create(memberUser.Id, memberRole.Id));
            await context.SaveChangesAsync();
        }
        else
        {
            memberUser.UpdatePassword(BCrypt.Net.BCrypt.HashPassword("Member@123", 11));
            await context.SaveChangesAsync();
        }
    }
}
