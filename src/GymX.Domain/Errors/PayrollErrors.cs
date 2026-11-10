using GymX.Domain.Common.Models;

namespace GymX.Domain.Errors;

public static class PayrollErrors
{
    public static Error PeriodNotFound(Guid id) =>
        Error.NotFound("Payroll.PeriodNotFound", $"Không tìm thấy kỳ tính lương với mã ID '{id}'.");

    public static Error PeriodNotFoundForMonth(int month, int year) =>
        Error.NotFound("Payroll.PeriodNotFoundForMonth", $"Không tìm thấy kỳ tính lương cho tháng {month:D2}/{year}.");

    public static readonly Error PeriodAlreadyConfirmed =
        Error.Conflict("Payroll.PeriodAlreadyConfirmed", "Kỳ tính lương này đã được chốt và khóa. Không thể tính lại hoặc sửa đổi.");

    public static readonly Error PeriodNotCalculated =
        Error.Validation("Payroll.PeriodNotCalculated", "Kỳ tính lương chưa được tính toán hoặc đang ở trạng thái không hợp lệ để khóa.");

    public static Error PayslipNotFound(Guid id) =>
        Error.NotFound("Payroll.PayslipNotFound", $"Không tìm thấy phiếu lương với mã ID '{id}'.");

    public static Error NoActiveFormulaFound(string role) =>
        Error.NotFound("Payroll.NoActiveFormulaFound", $"Không tìm thấy công thức tính lương đang hoạt động cho vai trò '{role}'. Vui lòng thiết lập công thức tính lương trước.");

    public static readonly Error NoActiveEmployeesFound =
        Error.Validation("Payroll.NoActiveEmployeesFound", "Không tìm thấy nhân viên đang hoạt động nào trong hệ thống để tính lương.");
}
