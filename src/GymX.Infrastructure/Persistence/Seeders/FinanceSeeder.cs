using GymX.Domain.Constants;
using GymX.Domain.Constants.Finance;
using GymX.Domain.Constants.Payroll;
using GymX.Domain.Entities.Attendance;
using GymX.Domain.Entities.Finance;
using GymX.Domain.Entities.Payroll;
using GymX.Domain.Entities.Training;
using GymX.Domain.Entities.Employees;
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

        var inactiveEmployees = await context.Employees
            .Where(e => !e.IsDeleted && (e.EmployeeCode == "NV0018" || e.EmployeeCode == "NV0020"))
            .ToListAsync();

        foreach (var emp in inactiveEmployees)
        {
            emp.UpdateInfo(emp.Position, emp.BaseSalaryRate, EmployeeStatus.Active);
        }
        if (inactiveEmployees.Count > 0)
        {
            await context.SaveChangesAsync();
        }

        var employees = await context.Employees
            .Where(e => !e.IsDeleted && e.EmploymentStatus == EmployeeStatus.Active)
            .OrderBy(e => e.EmployeeCode)
            .Take(10)
            .ToListAsync();

        var monthStart = new DateOnly(todayVn.Year, todayVn.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var startUtc = monthStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);

        var spec = await context.Specializations.FirstOrDefaultAsync();
        var specId = spec?.Id ?? Guid.NewGuid();

        var profiles = new Dictionary<string, (int attendedDays, int[] lateDays, int ptSessions, decimal salesRevenue, bool hasViolation)>
        {
            ["NV0018"] = (26, [], 35, 30000000m, false),
            ["NV0020"] = (25, [], 0, 14000000m, false),
            ["NV0021"] = (24, [4, 12], 0, 15000000m, true),
            ["NV0022"] = (26, [], 32, 25000000m, false),
            ["NV0023"] = (25, [7], 0, 10000000m, false),
            ["NV0024"] = (24, [5], 25, 18000000m, false),
            ["NV0025"] = (26, [], 28, 20000000m, false),
            ["NV0026"] = (22, [], 0, 8000000m, false),
            ["NV0027"] = (26, [], 0, 12000000m, false),
            ["NV0028"] = (20, [8], 0, 6000000m, false)
        };

        var empIds = employees.Select(e => e.Id).ToList();
        var existingSchedulesCount = await context.EmployeeWorkSchedules
            .CountAsync(s => empIds.Contains(s.EmployeeId) && s.WorkDate >= monthStart && s.WorkDate <= monthEnd);

        if (existingSchedulesCount < employees.Count * 26)
        {
            var oldSchedules = await context.EmployeeWorkSchedules
                .Where(s => empIds.Contains(s.EmployeeId) && s.WorkDate >= monthStart && s.WorkDate <= monthEnd)
                .ToListAsync();

            if (oldSchedules.Count > 0)
            {
                var oldScheduleIds = oldSchedules.Select(s => s.Id).ToList();
                var oldRecords = await context.EmployeeAttendanceRecords
                    .Where(a => oldScheduleIds.Contains(a.WorkScheduleId))
                    .ToListAsync();
                context.EmployeeAttendanceRecords.RemoveRange(oldRecords);
                context.EmployeeWorkSchedules.RemoveRange(oldSchedules);
                await context.SaveChangesAsync();
            }

            var allSchedules = new List<EmployeeWorkSchedule>();
            var allRecords = new List<EmployeeAttendanceRecord>();

            foreach (var emp in employees)
            {
                profiles.TryGetValue(emp.EmployeeCode, out var profile);
                if (profile.attendedDays == 0)
                {
                    profile = (26, [], emp.Position == "TRAINER" ? 20 : 0, 10000000m, false);
                }

                var isTrainer = emp.Position == "TRAINER";
                var shift = isTrainer ? afternoon : morning;
                var shiftStart = isTrainer ? new TimeOnly(14, 0) : new TimeOnly(6, 0);
                var shiftEnd = isTrainer ? new TimeOnly(22, 0) : new TimeOnly(14, 0);

                var empSchedules = new List<EmployeeWorkSchedule>();
                var dayLimit = Math.Min(26, monthEnd.Day);
                for (var d = 1; d <= dayLimit; d++)
                {
                    var workDate = new DateOnly(todayVn.Year, todayVn.Month, d);
                    var schedule = EmployeeWorkSchedule.Create(emp.Id, shift.Id, workDate);
                    schedule.Approve(adminId);
                    empSchedules.Add(schedule);
                }
                allSchedules.AddRange(empSchedules);
            }

            await context.EmployeeWorkSchedules.AddRangeAsync(allSchedules);
            await context.SaveChangesAsync();

            foreach (var emp in employees)
            {
                profiles.TryGetValue(emp.EmployeeCode, out var profile);
                if (profile.attendedDays == 0) profile.attendedDays = 26;

                var isTrainer = emp.Position == "TRAINER";
                var shiftStart = isTrainer ? new TimeOnly(14, 0) : new TimeOnly(6, 0);
                var shiftEnd = isTrainer ? new TimeOnly(22, 0) : new TimeOnly(14, 0);

                var empSchedules = allSchedules.Where(s => s.EmployeeId == emp.Id).OrderBy(s => s.WorkDate).ToList();
                var attendCount = Math.Min(profile.attendedDays, empSchedules.Count);

                for (var i = 0; i < attendCount; i++)
                {
                    var sch = empSchedules[i];
                    var isLate = profile.lateDays != null && profile.lateDays.Contains(i + 1);
                    var checkInTime = sch.WorkDate.ToDateTime(isLate ? shiftStart.AddMinutes(15) : shiftStart.AddMinutes(-5), DateTimeKind.Utc).AddHours(-7);
                    var checkOutTime = sch.WorkDate.ToDateTime(shiftEnd.AddMinutes(5), DateTimeKind.Utc).AddHours(-7);

                    var att = EmployeeAttendanceRecord.Create(sch.Id, AttendanceStatus.Present);
                    att.MarkCheckIn(checkInTime, AttendanceMethod.FaceRecognition, isLate ? 15 : 0);
                    att.MarkCheckOut(checkOutTime, AttendanceMethod.FaceRecognition, 0);
                    allRecords.Add(att);
                }
            }

            await context.EmployeeAttendanceRecords.AddRangeAsync(allRecords);
            await context.SaveChangesAsync();
        }

        foreach (var emp in employees.Where(e => e.Position == "TRAINER"))
        {
            var trainerEntity = await context.Trainers.FirstOrDefaultAsync(t => t.EmployeeId == emp.Id);
            if (trainerEntity == null)
            {
                trainerEntity = Trainer.Create(emp.Id, specId, 3, 1.2m, "HLV chuyên nghiệp GymX");
                await context.Trainers.AddAsync(trainerEntity);
                await context.SaveChangesAsync();
            }

            profiles.TryGetValue(emp.EmployeeCode, out var profile);
            var targetSessions = profile.ptSessions > 0 ? profile.ptSessions : 25;

            var existingCount = await context.TrainingSessions
                .CountAsync(s => s.TrainerId == trainerEntity.Id && s.StartAt >= startUtc);

            if (existingCount < targetSessions)
            {
                var needed = targetSessions - existingCount;
                var sessionsToAdd = new List<TrainingSession>();
                for (var s = 1; s <= needed; s++)
                {
                    var day = ((existingCount + s) % 26) + 1;
                    var sessionDate = new DateOnly(todayVn.Year, todayVn.Month, day);
                    var sessionStart = sessionDate.ToDateTime(new TimeOnly(15, 0), DateTimeKind.Utc).AddHours(-7);
                    var sessionEnd = sessionDate.ToDateTime(new TimeOnly(16, 0), DateTimeKind.Utc).AddHours(-7);

                    var session = TrainingSession.Create(Guid.NewGuid(), trainerEntity.Id, Guid.NewGuid(), sessionStart, sessionEnd, 200000m);
                    session.Complete(trainerEntity.Id);
                    sessionsToAdd.Add(session);
                }
                await context.TrainingSessions.AddRangeAsync(sessionsToAdd);
                await context.SaveChangesAsync();
            }
        }

        var invoiceSeq = 8000;
        foreach (var emp in employees)
        {
            profiles.TryGetValue(emp.EmployeeCode, out var profile);
            var targetRevenue = profile.salesRevenue > 0 ? profile.salesRevenue : 10000000m;

            var existingRevenue = await context.Invoices
                .Where(i => i.CreatorId == emp.UserId && i.Status == InvoiceStatus.Paid && i.IssuedAt >= startUtc)
                .SumAsync(i => i.TotalAmount);

            if (existingRevenue < targetRevenue)
            {
                var neededRevenue = targetRevenue - existingRevenue;
                var invoiceCount = Math.Max(1, (int)(neededRevenue / 5000000m));
                var perInvoice = Math.Round(neededRevenue / invoiceCount, 0);

                var empInvoices = new List<Invoice>();
                for (var k = 0; k < invoiceCount; k++)
                {
                    var day = ((k * 3) % 25) + 1;
                    var invDate = new DateOnly(todayVn.Year, todayVn.Month, day);
                    var issuedAt = invDate.ToDateTime(new TimeOnly(10, 30), DateTimeKind.Utc).AddHours(-7);
                    var invNumber = $"INV-{invDate:yyyyMMdd}-{invoiceSeq++:D4}";

                    var refType = emp.Position == "TRAINER" ? "PT_SESSION" : "PACKAGE";
                    empInvoices.Add(Invoice.CreatePaid(
                        invNumber,
                        refType,
                        Guid.NewGuid(),
                        perInvoice,
                        perInvoice,
                        issuedAt,
                        null,
                        emp.Position == "TRAINER" ? "Gói tập kèm PT cá nhân" : "Gói tập hội viên GymX",
                        "0912345678",
                        emp.UserId));
                }
                await context.Invoices.AddRangeAsync(empInvoices);
                await context.SaveChangesAsync();
            }
        }

        var existingRules = await context.PenaltyRules.ToListAsync();
        foreach (var rule in existingRules)
        {
            if (rule.Name == "Trang phục và tác phong làm việc" && rule.ViolationType == "STAFF")
            {
                rule.UpdateInfo(rule.Name, rule.FineAmount, rule.SeverityLevel, rule.EffectiveFrom, rule.Status, rule.Description, RuleTargetAudience.General);
            }
        }

        var ruleTemplates = new (string name, decimal fine, string severity, string targetAudience, string desc)[]
        {
            ("Đi muộn hoặc về sớm quá 15 phút", 50000m, "MEDIUM", RuleTargetAudience.General, "Áp dụng cho toàn thể nhân viên khi chấm công trễ"),
            ("Nghỉ làm không phép / không báo trước", 200000m, "HIGH", RuleTargetAudience.General, "Tự ý bỏ ca trực không có sự đồng ý của quản lý"),
            ("HLV vắng mặt hoặc trễ giờ ca dạy kèm PT", 100000m, "MEDIUM", RuleTargetAudience.Trainer, "Huấn luyện viên không có mặt đúng giờ hẹn với hội viên"),
            ("HLV không cập nhật giáo án tập luyện của hội viên", 150000m, "MEDIUM", RuleTargetAudience.Trainer, "Không theo dõi và ghi chép lộ trình tập luyện"),
            ("Sử dụng điện thoại làm việc riêng trong giờ hướng dẫn", 50000m, "LOW", RuleTargetAudience.Trainer, "HLV lơ là không hỗ trợ hội viên đang tập luyện"),
            ("Rời quầy lễ tân không có người trực thay", 100000m, "MEDIUM", RuleTargetAudience.Staff, "Bỏ vị trí trực sảnh đón tiếp hội viên"),
            ("Không kiểm soát hội viên check-in vào phòng tập", 50000m, "MEDIUM", RuleTargetAudience.Staff, "Để khách vào tập khi chưa thực hiện quét mã check-in")
        };

        foreach (var t in ruleTemplates)
        {
            if (!existingRules.Any(r => r.Name == t.name))
            {
                var newRule = PenaltyRule.Create(
                    t.name,
                    t.fine,
                    t.severity,
                    todayVn.AddMonths(-3).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                    t.desc,
                    PenaltyRuleStatus.Active,
                    null,
                    null,
                    t.targetAudience);
                await context.PenaltyRules.AddAsync(newRule);
            }
        }
        await context.SaveChangesAsync();

        foreach (var emp in employees)
        {
            profiles.TryGetValue(emp.EmployeeCode, out var profile);
            if (profile.hasViolation)
            {
                var existingViolation = await context.Violations
                    .AnyAsync(v => v.EmployeeId == emp.Id && v.OccurredAt >= startUtc);

                if (!existingViolation)
                {
                    var penaltyRule = await context.PenaltyRules.FirstOrDefaultAsync(r => r.Status == PenaltyRuleStatus.Active)
                        ?? await context.PenaltyRules.FirstOrDefaultAsync();

                    if (penaltyRule != null)
                    {
                        var violation = Violation.Create(
                            $"VIO-{todayVn:yyyyMMdd}-{emp.EmployeeCode}",
                            emp.Id,
                            penaltyRule.Id,
                            100000m,
                            todayVn.AddDays(-3).ToDateTime(new TimeOnly(6, 15), DateTimeKind.Utc).AddHours(-7),
                            adminId,
                            null,
                            null,
                            "Đi trễ ca sáng 15 phút không có lý do chính đáng");
                        await context.Violations.AddAsync(violation);
                        await context.SaveChangesAsync();
                    }
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
    }
}
