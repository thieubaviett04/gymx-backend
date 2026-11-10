using GymX.Domain.Common.Models;

namespace GymX.Domain.Errors;

public static class ExpenseErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Expense.NotFound", $"Không tìm thấy phiếu chi với mã ID '{id}'.");

    public static readonly Error CodeAlreadyExists =
        Error.Conflict("Expense.CodeAlreadyExists", "Mã phiếu chi đã tồn tại trong hệ thống.");

    public static readonly Error AlreadyCancelled =
        Error.Conflict("Expense.AlreadyCancelled", "Phiếu chi này đã được hủy trước đó.");

    public static readonly Error InvalidCategory =
        Error.Validation("Expense.InvalidCategory", "Danh mục chi phí không hợp lệ.");

    public static readonly Error InvalidAmount =
        Error.Validation("Expense.InvalidAmount", "Số tiền chi phải là số dương lớn hơn 0.");

    public static readonly Error InvalidDate =
        Error.Validation("Expense.InvalidDate", "Ngày phát sinh chi phí không được vượt quá ngày hiện tại.");

    public static readonly Error CancellationReasonRequired =
        Error.Validation("Expense.CancellationReasonRequired", "Bắt buộc phải nhập lý do khi hủy phiếu chi.");
}
