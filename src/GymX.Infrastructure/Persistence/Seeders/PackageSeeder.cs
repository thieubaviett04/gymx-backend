using GymX.Domain.Entities.Packages;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Persistence.Seeders;

public static class PackageSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.MembershipPackages.AnyAsync())
        {
            await context.MembershipPackages.AddRangeAsync(
                MembershipPackage.Create("BASIC_1M", "Gói Cơ Bản 1 Tháng", 500000, 30, DateOnly.FromDateTime(DateTime.UtcNow), null, "Tập không giới hạn trong 1 tháng"),
                MembershipPackage.Create("PREMIUM_1Y", "Gói VIP 1 Năm", 5000000, 365, DateOnly.FromDateTime(DateTime.UtcNow), null, "Tập không giới hạn, khăn tắm miễn phí trong 1 năm"),
                MembershipPackage.Create("PT_10", "Gói PT 10 Buổi", 3000000, 90, DateOnly.FromDateTime(DateTime.UtcNow), 10, "10 buổi tập 1-1 với Huấn luyện viên")
            );
            await context.SaveChangesAsync();
        }
    }
}
