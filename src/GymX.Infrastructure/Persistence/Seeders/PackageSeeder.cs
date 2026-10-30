using GymX.Domain.Entities.Packages;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Persistence.Seeders;

public static class PackageSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.Packages.AnyAsync())
        {
            await context.Packages.AddRangeAsync(
                Package.Create("Gói Cơ Bản 1 Tháng", 500000, 1, DateOnly.FromDateTime(DateTime.UtcNow), description: "Tập không giới hạn trong 1 tháng"),
                Package.Create("Gói Tiêu Chuẩn 6 Tháng", 2500000, 6, DateOnly.FromDateTime(DateTime.UtcNow), description: "Tập không giới hạn trong 6 tháng"),
                Package.Create("Gói VIP 1 Năm", 5000000, 12, DateOnly.FromDateTime(DateTime.UtcNow), description: "Tập không giới hạn, khăn tắm miễn phí trong 1 năm"),
                Package.Create("Gói PT 3 Tháng", 3000000, 3, DateOnly.FromDateTime(DateTime.UtcNow), maxCheckins: 30, description: "30 buổi tập 1-1 với Huấn luyện viên")
            );
            await context.SaveChangesAsync();
        }
    }
}
