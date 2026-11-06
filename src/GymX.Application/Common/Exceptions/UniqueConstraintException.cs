namespace GymX.Application.Common.Exceptions;

public class UniqueConstraintException : ApplicationException
{
    public UniqueConstraintException(string message = "Đã xảy ra lỗi vi phạm ràng buộc dữ liệu duy nhất khi lưu thay đổi.") : base(message) { }

    public UniqueConstraintException(string message, Exception innerException) : base(message, innerException) { }
}

