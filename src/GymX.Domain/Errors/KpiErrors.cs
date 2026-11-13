using GymX.Domain.Common.Models;

namespace GymX.Domain.Errors;

public static class KpiErrors
{
    public static readonly Error TemplateNotFound = Error.NotFound(
        "KpiTemplate.NotFound",
        "Mẫu KPI không tồn tại.");

    public static readonly Error TemplateNameExists = Error.Conflict(
        "KpiTemplate.NameExists",
        "Tên mẫu KPI đã tồn tại.");

    public static readonly Error EmployeeKpiNotFound = Error.NotFound(
        "EmployeeKpi.NotFound",
        "Phiếu KPI không tồn tại.");

    public static readonly Error RoleMismatch = Error.Validation(
        "EmployeeKpi.RoleMismatch",
        "Chức vụ không khớp với mẫu KPI.");

    public static readonly Error EmptyEmployees = Error.Validation(
        "EmployeeKpi.EmptyEmployees",
        "Chưa chọn nhân viên giao KPI.");

    public static readonly Error AssignFailed = Error.Failure(
        "EmployeeKpi.AssignFailed",
        "Giao chỉ tiêu KPI thất bại.");
}
