using System.Globalization;
using System.Text;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Analytics.DTOs.Response;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Finance;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class FinancialAnalyticsRepository(ApplicationDbContext context) : IFinancialAnalyticsRepository
{
    private static readonly CultureInfo VnCulture = new("vi-VN");

    public async Task<RevenueAnalyticsDto> GetRevenueAnalyticsAsync(
        string period,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? metric,
        CancellationToken cancellationToken = default)
    {
        var (startDate, endDate, priorStartDate, priorEndDate) = ResolvePeriodDateRanges(period, fromDate, toDate);

        var periodStartUtc = startDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);
        var periodEndUtc = endDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc).AddHours(-7);

        var priorPeriodStartUtc = priorStartDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(-7);
        var priorPeriodEndUtc = priorEndDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc).AddHours(-7);

        var currentInvoices = await context.Invoices
            .AsNoTracking()
            .Where(invoice => invoice.Status == InvoiceStatus.Paid &&
                              invoice.IssuedAt >= periodStartUtc &&
                              invoice.IssuedAt <= periodEndUtc)
            .Select(invoice => new
            {
                invoice.Id,
                invoice.TotalAmount,
                invoice.IssuedAt,
                invoice.ReferenceType
            })
            .ToListAsync(cancellationToken);

        var priorRevenue = await context.Invoices
            .AsNoTracking()
            .Where(invoice => invoice.Status == InvoiceStatus.Paid &&
                              invoice.IssuedAt >= priorPeriodStartUtc &&
                              invoice.IssuedAt <= priorPeriodEndUtc)
            .SumAsync(invoice => invoice.TotalAmount, cancellationToken);

        var currentExpenses = await context.Expenses
            .AsNoTracking()
            .Where(expense => expense.Status == ExpenseStatus.Confirmed &&
                              expense.ExpenseDate >= startDate &&
                              expense.ExpenseDate <= endDate)
            .Select(expense => new
            {
                expense.Id,
                expense.Amount,
                expense.ExpenseDate,
                expense.Category
            })
            .ToListAsync(cancellationToken);

        var totalRevenue = currentInvoices.Sum(x => x.TotalAmount);
        var totalExpense = currentExpenses.Sum(x => x.Amount);
        var netProfit = totalRevenue - totalExpense;
        var totalTransactions = currentInvoices.Count;
        var averageOrderValue = totalTransactions > 0 ? Math.Round(totalRevenue / totalTransactions, 2) : 0m;

        var revenueGrowthRate = 0.0;
        if (priorRevenue > 0)
        {
            revenueGrowthRate = Math.Round((double)((totalRevenue - priorRevenue) / priorRevenue) * 100, 1);
        }
        else if (totalRevenue > 0)
        {
            revenueGrowthRate = 100.0;
        }

        var summary = new FinancialSummaryDto
        {
            TotalRevenue = totalRevenue,
            TotalExpense = totalExpense,
            NetProfit = netProfit,
            TotalTransactions = totalTransactions,
            RevenueGrowthRate = revenueGrowthRate,
            AverageOrderValue = averageOrderValue
        };

        var chartData = BuildChartSlots(period, startDate, endDate, currentInvoices, currentExpenses, totalRevenue);
        var categoryBreakdown = BuildCategoryBreakdown(currentInvoices, totalRevenue);

        return new RevenueAnalyticsDto
        {
            HasData = totalTransactions > 0 || totalExpense > 0,
            Summary = summary,
            ChartData = chartData,
            CategoryBreakdown = categoryBreakdown
        };
    }

    public async Task<byte[]> GenerateRevenueReportCsvAsync(
        string period,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default)
    {
        var analytics = await GetRevenueAnalyticsAsync(period, fromDate, toDate, null, cancellationToken);

        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("GYMX - BÁO CÁO THỐNG KÊ DOANH THU & TÀI CHÍNH");
        stringBuilder.AppendLine($"Kỳ thống kê:;{GetPeriodDisplayName(period, fromDate, toDate)}");
        stringBuilder.AppendLine($"Thời gian xuất:;{DateTime.UtcNow.AddHours(7):yyyy-MM-dd HH:mm:ss}");
        stringBuilder.AppendLine();

        stringBuilder.AppendLine("CHỈ SỐ TỔNG QUAN");
        stringBuilder.AppendLine("Chỉ số;Giá trị;Ghi chú");
        stringBuilder.AppendLine($"Tổng doanh thu;{analytics.Summary.TotalRevenue.ToString("N0", VnCulture)} VNĐ;Doanh thu từ các hóa đơn đã thanh toán");
        stringBuilder.AppendLine($"Chi phí vận hành;{analytics.Summary.TotalExpense.ToString("N0", VnCulture)} VNĐ;Tổng các phiếu chi đã duyệt");
        stringBuilder.AppendLine($"Lợi nhuận ròng;{analytics.Summary.NetProfit.ToString("N0", VnCulture)} VNĐ;Doanh thu trừ Chi phí");
        stringBuilder.AppendLine($"Tổng lượt giao dịch;{analytics.Summary.TotalTransactions};Số hóa đơn thanh toán thành công");
        stringBuilder.AppendLine($"Giá trị trung bình/lượt;{analytics.Summary.AverageOrderValue.ToString("N0", VnCulture)} VNĐ;Trung bình mỗi hóa đơn");
        stringBuilder.AppendLine($"Tăng trưởng so với kỳ trước;{analytics.Summary.RevenueGrowthRate:F1}%;So sánh cùng kỳ trước đó");
        stringBuilder.AppendLine();

        stringBuilder.AppendLine("CHI TIẾT PHÂN BỔ THEO THỜI GIAN");
        stringBuilder.AppendLine("Khoảng thời gian;Số giao dịch;Doanh thu (VNĐ);Chi phí (VNĐ);Lợi nhuận (VNĐ);Tỷ trọng doanh thu (%)");
        foreach (var slot in analytics.ChartData)
        {
            stringBuilder.AppendLine($"{slot.Label};{slot.Transactions};{slot.Revenue.ToString("N0", VnCulture)};{slot.Expense.ToString("N0", VnCulture)};{slot.Profit.ToString("N0", VnCulture)};{slot.Percentage:F1}%");
        }
        stringBuilder.AppendLine();

        stringBuilder.AppendLine("CƠ CẤU DOANH THU THEO NGUỒN");
        stringBuilder.AppendLine("Danh mục nguồn thu;Doanh thu (VNĐ);Tỷ trọng (%)");
        foreach (var category in analytics.CategoryBreakdown)
        {
            stringBuilder.AppendLine($"{category.CategoryName};{category.Revenue.ToString("N0", VnCulture)};{category.Percentage:F1}%");
        }

        var encoding = new UTF8Encoding(true);
        return encoding.GetPreamble().Concat(encoding.GetBytes(stringBuilder.ToString())).ToArray();
    }

    private static (DateOnly startDate, DateOnly endDate, DateOnly priorStartDate, DateOnly priorEndDate) ResolvePeriodDateRanges(
        string period,
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        var nowVn = DateTime.UtcNow.AddHours(7);
        var todayVn = DateOnly.FromDateTime(nowVn);

        switch (period.ToLowerInvariant())
        {
            case "today":
            {
                var priorDay = todayVn.AddDays(-1);
                return (todayVn, todayVn, priorDay, priorDay);
            }
            case "7days":
            {
                var start = todayVn.AddDays(-6);
                var priorStart = start.AddDays(-7);
                var priorEnd = start.AddDays(-1);
                return (start, todayVn, priorStart, priorEnd);
            }
            case "this_month":
            {
                var start = new DateOnly(todayVn.Year, todayVn.Month, 1);
                var end = start.AddMonths(1).AddDays(-1);
                var priorStart = start.AddMonths(-1);
                var priorEnd = start.AddDays(-1);
                return (start, end, priorStart, priorEnd);
            }
            case "this_year":
            {
                var start = new DateOnly(todayVn.Year, 1, 1);
                var end = new DateOnly(todayVn.Year, 12, 31);
                var priorStart = new DateOnly(todayVn.Year - 1, 1, 1);
                var priorEnd = new DateOnly(todayVn.Year - 1, 12, 31);
                return (start, end, priorStart, priorEnd);
            }
            case "custom":
            default:
            {
                var start = fromDate ?? todayVn.AddDays(-30);
                var end = toDate ?? todayVn;
                if (start > end)
                {
                    (start, end) = (end, start);
                }

                var duration = end.DayNumber - start.DayNumber + 1;
                var priorEnd = start.AddDays(-1);
                var priorStart = priorEnd.AddDays(-duration + 1);
                return (start, end, priorStart, priorEnd);
            }
        }
    }

    private static IReadOnlyCollection<RevenueSlotDto> BuildChartSlots(
        string period,
        DateOnly startDate,
        DateOnly endDate,
        IEnumerable<dynamic> invoices,
        IEnumerable<dynamic> expenses,
        decimal totalRevenue)
    {
        var slots = new List<RevenueSlotDto>();

        if (period.Equals("today", StringComparison.OrdinalIgnoreCase))
        {
            for (var hour = 6; hour <= 21; hour++)
            {
                var label = $"{hour:D2}:00 - {(hour + 1):D2}:00";
                var slotInvoices = invoices.Where(i =>
                {
                    var vnDateTime = ((DateTime)i.IssuedAt).AddHours(7);
                    return vnDateTime.Hour == hour;
                }).ToList();

                var slotRevenue = slotInvoices.Sum(i => (decimal)i.TotalAmount);
                var slotTxCount = slotInvoices.Count;
                var percentage = totalRevenue > 0 ? Math.Round((double)(slotRevenue / totalRevenue) * 100, 1) : 0.0;

                slots.Add(new RevenueSlotDto
                {
                    Label = label,
                    Revenue = slotRevenue,
                    Expense = 0m,
                    Profit = slotRevenue,
                    Transactions = slotTxCount,
                    Percentage = percentage
                });
            }
            return slots;
        }

        var daySpan = endDate.DayNumber - startDate.DayNumber + 1;

        if (period.Equals("this_year", StringComparison.OrdinalIgnoreCase) || daySpan > 35)
        {
            var cursor = new DateOnly(startDate.Year, startDate.Month, 1);
            var limit = new DateOnly(endDate.Year, endDate.Month, 1);

            while (cursor <= limit)
            {
                var monthStart = cursor;
                var monthEnd = cursor.AddMonths(1).AddDays(-1);
                var label = $"Tháng {monthStart.Month:D2}/{monthStart.Year}";

                var monthInvoices = invoices.Where(i =>
                {
                    var vnDate = DateOnly.FromDateTime(((DateTime)i.IssuedAt).AddHours(7));
                    return vnDate >= monthStart && vnDate <= monthEnd;
                }).ToList();

                var monthExpenses = expenses.Where(e =>
                {
                    var expDate = (DateOnly)e.ExpenseDate;
                    return expDate >= monthStart && expDate <= monthEnd;
                }).ToList();

                var slotRevenue = monthInvoices.Sum(i => (decimal)i.TotalAmount);
                var slotExpense = monthExpenses.Sum(e => (decimal)e.Amount);
                var slotProfit = slotRevenue - slotExpense;
                var slotTxCount = monthInvoices.Count;
                var percentage = totalRevenue > 0 ? Math.Round((double)(slotRevenue / totalRevenue) * 100, 1) : 0.0;

                slots.Add(new RevenueSlotDto
                {
                    Label = label,
                    Revenue = slotRevenue,
                    Expense = slotExpense,
                    Profit = slotProfit,
                    Transactions = slotTxCount,
                    Percentage = percentage
                });

                cursor = cursor.AddMonths(1);
            }

            return slots;
        }

        var currentDate = startDate;
        while (currentDate <= endDate)
        {
            var label = currentDate.ToString("yyyy-MM-dd");
            var targetDate = currentDate;

            var dayInvoices = invoices.Where(i =>
            {
                var vnDate = DateOnly.FromDateTime(((DateTime)i.IssuedAt).AddHours(7));
                return vnDate == targetDate;
            }).ToList();

            var dayExpenses = expenses.Where(e => ((DateOnly)e.ExpenseDate) == targetDate).ToList();

            var slotRevenue = dayInvoices.Sum(i => (decimal)i.TotalAmount);
            var slotExpense = dayExpenses.Sum(e => (decimal)e.Amount);
            var slotProfit = slotRevenue - slotExpense;
            var slotTxCount = dayInvoices.Count;
            var percentage = totalRevenue > 0 ? Math.Round((double)(slotRevenue / totalRevenue) * 100, 1) : 0.0;

            slots.Add(new RevenueSlotDto
            {
                Label = label,
                Revenue = slotRevenue,
                Expense = slotExpense,
                Profit = slotProfit,
                Transactions = slotTxCount,
                Percentage = percentage
            });

            currentDate = currentDate.AddDays(1);
        }

        return slots;
    }

    private static IReadOnlyCollection<CategoryBreakdownDto> BuildCategoryBreakdown(
        IEnumerable<dynamic> invoices,
        decimal totalRevenue)
    {
        var categoryGroups = invoices
            .GroupBy(i =>
            {
                var refType = (string?)i.ReferenceType;
                return string.IsNullOrWhiteSpace(refType) ? "OTHER" : refType.Trim().ToUpperInvariant();
            })
            .Select(group =>
            {
                var categoryName = group.Key switch
                {
                    "PACKAGE" => "Gói tập hội viên",
                    "PT_SESSION" => "Huấn luyện viên kèm riêng (PT)",
                    "DAY_TICKET" => "Vé tập vãng lai theo ngày",
                    "GENERAL" => "Dịch vụ phòng tập chung",
                    _ => group.Key
                };

                var revenue = group.Sum(x => (decimal)x.TotalAmount);
                var percentage = totalRevenue > 0 ? Math.Round((double)(revenue / totalRevenue) * 100, 1) : 0.0;

                return new CategoryBreakdownDto
                {
                    CategoryName = categoryName,
                    Revenue = revenue,
                    Percentage = percentage
                };
            })
            .OrderByDescending(x => x.Revenue)
            .ToList();

        return categoryGroups;
    }

    private static string GetPeriodDisplayName(string period, DateOnly? fromDate, DateOnly? toDate)
    {
        return period.ToLowerInvariant() switch
        {
            "today" => "Hôm nay",
            "7days" => "7 ngày qua",
            "this_month" => "Tháng này",
            "this_year" => "Năm nay",
            "custom" => $"Tùy chọn ({fromDate:dd/MM/yyyy} - {toDate:dd/MM/yyyy})",
            _ => period
        };
    }
}
