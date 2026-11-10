using GymX.Domain.Common.Models;

namespace GymX.Domain.Errors;

public static class SalaryFormulaErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("SalaryFormula.NotFound", $"Không tìm thấy công thức tính lương với mã ID '{id}'.");

    public static readonly Error NameAlreadyExists =
        Error.Conflict("SalaryFormula.NameAlreadyExists", "Tên công thức tính lương đã tồn tại trong hệ thống.");

    public static readonly Error InvalidTargetRole =
        Error.Validation("SalaryFormula.InvalidTargetRole", "Vai trò áp dụng không hợp lệ. Chỉ chấp nhận TRAINER, RECEPTIONIST hoặc ALL.");

    public static readonly Error InvalidBaseRate =
        Error.Validation("SalaryFormula.InvalidBaseRate", "Mức lương cơ bản phải là số không âm (>= 0).");

    public static readonly Error InvalidStandardWorkDays =
        Error.Validation("SalaryFormula.InvalidStandardWorkDays", "Số ngày công chuẩn trong tháng phải từ 1 đến 31 ngày.");

    public static readonly Error InvalidPricePerPtSession =
        Error.Validation("SalaryFormula.InvalidPricePerPtSession", "Đơn giá buổi dạy PT phải là số không âm (>= 0).");

    public static readonly Error InvalidSalesCommissionPercent =
        Error.Validation("SalaryFormula.InvalidSalesCommissionPercent", "Tỷ lệ hoa hồng bán gói phải nằm trong khoảng từ 0% đến 100%.");

    public static readonly Error EffectiveDateInPast =
        Error.Validation("SalaryFormula.EffectiveDateInPast", "Ngày áp dụng công thức mới không được là ngày trong quá khứ.");

    public static readonly Error CannotModifyInUse =
        Error.Conflict("SalaryFormula.CannotModifyInUse", "Không thể xóa hoặc sửa công thức này do đã được áp dụng trong kỳ tính lương đã khóa.");
}
