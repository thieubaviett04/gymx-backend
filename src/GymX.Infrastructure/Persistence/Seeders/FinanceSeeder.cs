using GymX.Domain.Constants;
using GymX.Domain.Constants.Finance;
using GymX.Domain.Entities.Finance;
using GymX.Domain.Entities.Payroll;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Persistence.Seeders;

public static class FinanceSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@gymx.vn");
        var adminId = adminUser?.Id ?? Guid.NewGuid();

        var nowVn = DateTime.UtcNow.AddHours(7);
        var todayVn = DateOnly.FromDateTime(nowVn);

        if (!await context.SalaryFormulas.AnyAsync())
        {
            var trainerFormula = SalaryFormula.Create(
                "Chính sách lương Huấn luyện viên 2026",
                "TRAINER",
                SalaryBaseRateType.Monthly,
                15000000m,
                26,
                200000m,
                10.0m,
                todayVn.AddMonths(-2),
                adminId,
                "Áp dụng cho toàn bộ HLV cá nhân (PT) làm việc chính thức tại cơ sở");

            var receptionistFormula = SalaryFormula.Create(
                "Chính sách lương Lễ tân 2026",
                "RECEPTIONIST",
                SalaryBaseRateType.Monthly,
                10000000m,
                26,
                26,
                0m,
                todayVn.AddMonths(-2),
                adminId,
                "Áp dụng cho nhân viên lễ tân trực sảnh và tư vấn dịch vụ");

            await context.SalaryFormulas.AddRangeAsync(trainerFormula, receptionistFormula);
            await context.SaveChangesAsync();
        }

        if (!await context.Expenses.AnyAsync())
        {
            var expenses = new List<Expense>
            {
                Expense.Create(
                    "EXP-202611-0001",
                    ExpenseCategory.Rent,
                    10000000m,
                    todayVn.AddDays(-5),
                    "Chi phí thuê mặt bằng phòng gym kỳ này",
                    adminId),
                Expense.Create(
                    "EXP-202611-0002",
                    ExpenseCategory.Utilities,
                    2500000m,
                    todayVn.AddDays(-3),
                    "Chi phí tiền điện, nước, internet cáp quang",
                    adminId),
                Expense.Create(
                    "EXP-202611-0003",
                    ExpenseCategory.Maintenance,
                    1200000m,
                    todayVn.AddDays(-1),
                    "Bảo trì hệ thống máy chạy bộ và giàn tạ",
                    adminId),
                Expense.Create(
                    "EXP-202611-0004",
                    ExpenseCategory.Supplies,
                    800000m,
                    todayVn,
                    "Mua khăn tập, nước uống và vật tư vệ sinh",
                    adminId)
            };

            await context.Expenses.AddRangeAsync(expenses);
            await context.SaveChangesAsync();
        }

        if (!await context.Invoices.AnyAsync())
        {
            var invoices = new List<Invoice>();
            var todayBaseUtc = todayVn.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);

            var todaySlots = new (int Hour, int Count, decimal Total, string RefType, string Name)[]
            {
                (6, 5, 1000000m, "DAY_TICKET", "Vé tập vãng lai buổi sáng"),
                (10, 3, 500000m, "DAY_TICKET", "Vé tập vãng lai theo ngày"),
                (16, 6, 1200000m, "PACKAGE", "Gói tập hội viên Silver 1 tháng"),
                (16, 4, 800000m, "PT_SESSION", "Gói Huấn luyện viên kèm riêng"),
                (18, 4, 1000000m, "PACKAGE", "Gói tập hội viên Gold 3 tháng"),
                (21, 3, 500000m, "DAY_TICKET", "Vé tập vãng lai buổi tối")
            };

            var seq = 1;
            foreach (var slot in todaySlots)
            {
                var perInvoice = Math.Round(slot.Total / slot.Count, 0);
                for (var i = 0; i < slot.Count; i++)
                {
                    var issuedAtUtc = todayBaseUtc.AddHours(slot.Hour).AddMinutes(10 * (i + 1));
                    var invoiceNumber = $"INV-{todayVn:yyyyMMdd}-{seq:D4}";
                    seq++;

                    invoices.Add(Invoice.CreatePaid(
                        invoiceNumber,
                        slot.RefType,
                        Guid.NewGuid(),
                        perInvoice,
                        perInvoice,
                        issuedAtUtc,
                        null,
                        slot.Name,
                        "0987654321",
                        adminId));
                }
            }

            for (var d = 1; d <= 6; d++)
            {
                var pastDate = todayVn.AddDays(-d);
                var pastBaseUtc = pastDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);

                for (var t = 0; t < 3; t++)
                {
                    var hour = 8 + t * 4;
                    var amount = 500000m + (t * 250000m);
                    var refType = t % 2 == 0 ? "PACKAGE" : "PT_SESSION";
                    var issuedAtUtc = pastBaseUtc.AddHours(hour).AddMinutes(15);
                    var invoiceNumber = $"INV-{pastDate:yyyyMMdd}-{seq:D4}";
                    seq++;

                    invoices.Add(Invoice.CreatePaid(
                        invoiceNumber,
                        refType,
                        Guid.NewGuid(),
                        amount,
                        amount,
                        issuedAtUtc,
                        null,
                        "Hội viên GymX",
                        "0912345678",
                        adminId));
                }
            }

            for (var m = 1; m <= 10; m++)
            {
                var priorDate = todayVn.AddMonths(-1).AddDays(m * 2);
                var priorBaseUtc = priorDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);
                var invoiceNumber = $"INV-{priorDate:yyyyMMdd}-{seq:D4}";
                seq++;

                invoices.Add(Invoice.CreatePaid(
                    invoiceNumber,
                    "PACKAGE",
                    Guid.NewGuid(),
                    1000000m,
                    1000000m,
                    priorBaseUtc.AddHours(14),
                    null,
                    "Hội viên tháng trước",
                    "0909090909",
                    adminId));
            }

            await context.Invoices.AddRangeAsync(invoices);
            await context.SaveChangesAsync();
        }
    }
}

