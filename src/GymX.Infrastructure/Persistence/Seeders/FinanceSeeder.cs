using GymX.Domain.Constants;
using GymX.Domain.Constants.Finance;
using GymX.Domain.Constants.Payroll;
using GymX.Domain.Entities.Attendance;
using GymX.Domain.Entities.Finance;
using GymX.Domain.Entities.Payroll;
using GymX.Domain.Entities.Training;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Persistence.Seeders;

public static class FinanceSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@gymx.vn");
        var adminId = adminUser?.Id ?? Guid.NewGuid();

        var trainerUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "trainer@gymx.vn");
        var trainerId = trainerUser?.Id ?? adminId;

        var recUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "receptionist@gymx.vn");
        var recId = recUser?.Id ?? adminId;

        var nowVn = DateTime.UtcNow.AddHours(7);
        var todayVn = DateOnly.FromDateTime(nowVn);

        if (!await context.SalaryFormulas.AnyAsync())
        {
            var trainerFormula = SalaryFormula.Create(
                "Chính sách lương Huấn luyện viên 2026",
                "TRAINER",
                SalaryBaseRateType.Monthly,
                5000000m,
                26,
                100000m,
                10.0m,
                todayVn.AddMonths(-2),
                adminId,
                "Áp dụng cho toàn bộ HLV cá nhân (PT) làm việc chính thức tại cơ sở");

            var receptionistFormula = SalaryFormula.Create(
                "Chính sách lương Lễ tân 2026",
                "RECEPTIONIST",
                SalaryBaseRateType.Monthly,
                6500000m,
                26,
                0m,
                3.0m,
                todayVn.AddMonths(-2),
                adminId,
                "Áp dụng cho nhân viên lễ tân trực sảnh và tư vấn dịch vụ");

            await context.SalaryFormulas.AddRangeAsync(trainerFormula, receptionistFormula);
            await context.SaveChangesAsync();
        }
        else
        {
            var existingTrainerFormula = await context.SalaryFormulas
                .FirstOrDefaultAsync(f => f.TargetRole == "TRAINER" && f.Status == EmployeeStatus.Active);
            if (existingTrainerFormula != null && existingTrainerFormula.BaseRate > 5000000m)
            {
                existingTrainerFormula.Update(
                    existingTrainerFormula.Name,
                    existingTrainerFormula.TargetRole,
                    existingTrainerFormula.BaseRateType,
                    5000000m,
                    existingTrainerFormula.StandardWorkDays,
                    100000m,
                    10.0m,
                    existingTrainerFormula.EffectiveDate,
                    "Áp dụng cho toàn bộ HLV cá nhân (PT) làm việc chính thức tại cơ sở");
            }

            var existingRecFormula = await context.SalaryFormulas
                .FirstOrDefaultAsync(f => f.TargetRole == "RECEPTIONIST" && f.Status == EmployeeStatus.Active);
            if (existingRecFormula != null && (existingRecFormula.BaseRate > 6500000m || existingRecFormula.SalesCommissionPercent == 0m))
            {
                existingRecFormula.Update(
                    existingRecFormula.Name,
                    existingRecFormula.TargetRole,
                    existingRecFormula.BaseRateType,
                    6500000m,
                    existingRecFormula.StandardWorkDays,
                    0m,
                    3.0m,
                    existingRecFormula.EffectiveDate,
                    "Áp dụng cho nhân viên lễ tân trực sảnh và tư vấn dịch vụ");
            }

            await context.SaveChangesAsync();
        }

        if (!await context.ShiftDefinitions.AnyAsync())
        {
            var morningShift = ShiftDefinition.Create("Ca sáng", new TimeOnly(6, 0), new TimeOnly(14, 0));
            var afternoonShift = ShiftDefinition.Create("Ca chiều", new TimeOnly(14, 0), new TimeOnly(22, 0));
            await context.ShiftDefinitions.AddRangeAsync(morningShift, afternoonShift);
            await context.SaveChangesAsync();
        }

        var shifts = await context.ShiftDefinitions.ToListAsync();
        var morning = shifts.FirstOrDefault(s => s.Name == "Ca sáng") ?? shifts.First();
        var afternoon = shifts.FirstOrDefault(s => s.Name == "Ca chiều") ?? shifts.First();

        var recEmp = await context.Employees.FirstOrDefaultAsync(e => e.Position == "RECEPTIONIST" && !e.IsDeleted);
        var trainerEmp = await context.Employees.FirstOrDefaultAsync(e => e.Position == "TRAINER" && !e.IsDeleted);

        var monthStart = new DateOnly(todayVn.Year, todayVn.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        if (recEmp != null && !await context.EmployeeWorkSchedules.AnyAsync(s => s.EmployeeId == recEmp.Id && s.WorkDate >= monthStart && s.WorkDate <= monthEnd))
        {
            var recSchedules = new List<EmployeeWorkSchedule>();
            var dayCount = Math.Min(26, monthEnd.Day);
            for (var d = 1; d <= dayCount; d++)
            {
                var workDate = new DateOnly(todayVn.Year, todayVn.Month, d);
                var schedule = EmployeeWorkSchedule.Create(recEmp.Id, morning.Id, workDate);
                schedule.Approve(adminId);
                recSchedules.Add(schedule);
            }
            await context.EmployeeWorkSchedules.AddRangeAsync(recSchedules);
            await context.SaveChangesAsync();

            var recRecords = new List<EmployeeAttendanceRecord>();
            for (var i = 0; i < recSchedules.Count; i++)
            {
                var sch = recSchedules[i];
                if (i >= 24) continue;

                var isLate = i == 3 || i == 11;
                var checkInTime = sch.WorkDate.ToDateTime(isLate ? new TimeOnly(6, 15) : new TimeOnly(5, 55), DateTimeKind.Utc).AddHours(-7);
                var checkOutTime = sch.WorkDate.ToDateTime(new TimeOnly(14, 5), DateTimeKind.Utc).AddHours(-7);

                var att = EmployeeAttendanceRecord.Create(sch.Id, AttendanceStatus.Present);
                att.MarkCheckIn(checkInTime, AttendanceMethod.FaceRecognition, isLate ? 15 : 0);
                att.MarkCheckOut(checkOutTime, AttendanceMethod.FaceRecognition, 0);
                recRecords.Add(att);
            }
            await context.EmployeeAttendanceRecords.AddRangeAsync(recRecords);
            await context.SaveChangesAsync();
        }

        if (trainerEmp != null && !await context.EmployeeWorkSchedules.AnyAsync(s => s.EmployeeId == trainerEmp.Id && s.WorkDate >= monthStart && s.WorkDate <= monthEnd))
        {
            var trainerSchedules = new List<EmployeeWorkSchedule>();
            var dayCount = Math.Min(26, monthEnd.Day);
            for (var d = 1; d <= dayCount; d++)
            {
                var workDate = new DateOnly(todayVn.Year, todayVn.Month, d);
                var schedule = EmployeeWorkSchedule.Create(trainerEmp.Id, afternoon.Id, workDate);
                schedule.Approve(adminId);
                trainerSchedules.Add(schedule);
            }
            await context.EmployeeWorkSchedules.AddRangeAsync(trainerSchedules);
            await context.SaveChangesAsync();

            var trainerRecords = new List<EmployeeAttendanceRecord>();
            foreach (var sch in trainerSchedules)
            {
                var checkInTime = sch.WorkDate.ToDateTime(new TimeOnly(13, 55), DateTimeKind.Utc).AddHours(-7);
                var checkOutTime = sch.WorkDate.ToDateTime(new TimeOnly(22, 5), DateTimeKind.Utc).AddHours(-7);

                var att = EmployeeAttendanceRecord.Create(sch.Id, AttendanceStatus.Present);
                att.MarkCheckIn(checkInTime, AttendanceMethod.FaceRecognition, 0);
                att.MarkCheckOut(checkOutTime, AttendanceMethod.FaceRecognition, 0);
                trainerRecords.Add(att);
            }
            await context.EmployeeAttendanceRecords.AddRangeAsync(trainerRecords);
            await context.SaveChangesAsync();
        }

        if (trainerEmp != null)
        {
            var trainerEntity = await context.Trainers.FirstOrDefaultAsync(t => t.EmployeeId == trainerEmp.Id);
            if (trainerEntity != null)
            {
                var startUtc = monthStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);
                var existingSessionsCount = await context.TrainingSessions
                    .CountAsync(s => s.TrainerId == trainerEntity.Id && s.StartAt >= startUtc);

                if (existingSessionsCount == 0)
                {
                    var trainingSessions = new List<TrainingSession>();
                    for (var s = 1; s <= 30; s++)
                    {
                        var day = (s % 26) + 1;
                        var sessionDate = new DateOnly(todayVn.Year, todayVn.Month, day);
                        var sessionStart = sessionDate.ToDateTime(new TimeOnly(15, 0), DateTimeKind.Utc).AddHours(-7);
                        var sessionEnd = sessionDate.ToDateTime(new TimeOnly(16, 0), DateTimeKind.Utc).AddHours(-7);

                        var session = TrainingSession.Create(Guid.NewGuid(), trainerEntity.Id, Guid.NewGuid(), sessionStart, sessionEnd, 200000m);
                        session.Complete(trainerEntity.Id);
                        trainingSessions.Add(session);
                    }
                    await context.TrainingSessions.AddRangeAsync(trainingSessions);
                    await context.SaveChangesAsync();
                }
            }
        }

        if (recEmp != null)
        {
            var startUtc = monthStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);
            if (!await context.Violations.AnyAsync(v => v.EmployeeId == recEmp.Id && v.OccurredAt >= startUtc))
            {
                var penaltyRule = await context.PenaltyRules.FirstOrDefaultAsync(r => r.Status == PenaltyRuleStatus.Active)
                    ?? await context.PenaltyRules.FirstOrDefaultAsync();

                if (penaltyRule != null)
                {
                    var violation = Violation.Create(
                        "VIO-202611-0001",
                        recEmp.Id,
                        penaltyRule.Id,
                        100000m,
                        todayVn.AddDays(-2).ToDateTime(new TimeOnly(6, 15), DateTimeKind.Utc).AddHours(-7),
                        adminId,
                        null,
                        null,
                        "Đi trễ 15 phút không báo trước");
                    await context.Violations.AddAsync(violation);
                    await context.SaveChangesAsync();
                }
            }
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
                    "Thanh toán tiền điện và nước tháng này",
                    adminId),
                Expense.Create(
                    "EXP-202611-0003",
                    ExpenseCategory.Maintenance,
                    1200000m,
                    todayVn.AddDays(-1),
                    "Bảo dưỡng định kỳ dàn máy chạy bộ phòng tạ",
                    adminId)
            };

            await context.Expenses.AddRangeAsync(expenses);
            await context.SaveChangesAsync();
        }

        if (!await context.Invoices.AnyAsync())
        {
            var invoices = new List<Invoice>();
            var seq = 1;
            var todayBaseUtc = todayVn.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);

            var todaySlots = new[]
            {
                new { Hour = 7,  Count = 3, RefType = "DAY_PASS", Total = 150000m, Name = "Vé tập ngày" },
                new { Hour = 9,  Count = 4, RefType = "PACKAGE",  Total = 3200000m, Name = "Gói 1 tháng Tiêu chuẩn" },
                new { Hour = 11, Count = 2, RefType = "PACKAGE",  Total = 2400000m, Name = "Gói 3 tháng Khỏe đẹp" },
                new { Hour = 14, Count = 3, RefType = "PT_SESSION", Total = 1800000m, Name = "Kèm PT 1:1" },
                new { Hour = 16, Count = 4, RefType = "PACKAGE",  Total = 4000000m, Name = "Gói 6 tháng Chuyên sâu" },
                new { Hour = 18, Count = 5, RefType = "DAY_PASS", Total = 250000m, Name = "Vé tập ngày cao điểm" },
                new { Hour = 20, Count = 4, RefType = "PACKAGE",  Total = 5000000m, Name = "Gói VIP 12 tháng" },
            };

            foreach (var slot in todaySlots)
            {
                var perInvoice = Math.Round(slot.Total / slot.Count, 0);
                var invoiceCreatorId = slot.RefType == "PT_SESSION" ? trainerId : recId;

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
                        invoiceCreatorId));
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
                    var pastCreatorId = refType == "PT_SESSION" ? trainerId : recId;
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
                        pastCreatorId));
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
                    recId));
            }

            await context.Invoices.AddRangeAsync(invoices);
            await context.SaveChangesAsync();
        }
        else if (adminUser != null)
        {
            var unassignedInvoices = await context.Invoices
                .Where(i => i.CreatorId == adminUser.Id || i.CreatorId == null)
                .ToListAsync();

            if (unassignedInvoices.Count > 0)
            {
                foreach (var inv in unassignedInvoices)
                {
                    var targetCreator = inv.ReferenceType == "PT_SESSION" ? trainerId : recId;
                    inv.AssignCreator(targetCreator);
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
