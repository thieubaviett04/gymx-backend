using GymX.Domain.Common.Models;

namespace GymX.Domain.Errors;

public static class PackageCategoryErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("PackageCategory.NotFound", $"Không tìm thấy loại hình gói tập với mã ID '{id}'.");

    public static readonly Error CodeAlreadyExists =
        Error.Conflict("PackageCategory.CodeAlreadyExists", "Mã loại hình gói tập đã tồn tại trong hệ thống.");

    public static readonly Error NameAlreadyExists =
        Error.Conflict("PackageCategory.NameAlreadyExists", "Tên loại hình gói tập đã tồn tại trong hệ thống.");

    public static readonly Error CannotDeleteInUse =
        Error.Conflict("PackageCategory.CannotDeleteInUse", "Không thể xóa loại hình này do đang có gói tập liên kết. Bạn có thể chuyển trạng thái sang 'Ngừng hoạt động' (INACTIVE) để ẩn khỏi danh sách lựa chọn.");

    public static readonly Error InvalidStatus =
        Error.Validation("PackageCategory.InvalidStatus", "Trạng thái loại hình gói tập không hợp lệ. Chỉ chấp nhận ACTIVE hoặc INACTIVE.");
}
