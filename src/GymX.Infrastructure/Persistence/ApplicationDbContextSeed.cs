using GymX.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymX.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static async Task SeedSampleDataAsync(ApplicationDbContext context, ILogger logger)
    {
        try
        {
            logger.LogInformation("Bắt đầu tự động cập nhật Database (Migration)...");
            if (context.Database.IsNpgsql())
            {
                await context.Database.MigrateAsync();
            }

            logger.LogInformation("Bắt đầu Seed dữ liệu mẫu...");
            
            await IdentitySeeder.SeedAsync(context);
            logger.LogInformation("- Đã seed xong Identity (Roles, Admin User).");

            await FinanceSeeder.SeedAsync(context);
            logger.LogInformation("- Đã seed xong Dữ liệu mẫu Tài chính & Doanh thu (Invoices, Expenses, Formulas).");

            logger.LogInformation("Quá trình Seed dữ liệu hoàn tất!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Đã xảy ra lỗi trong quá trình Migration/Seed dữ liệu!");
            throw;
        }
    }
}
