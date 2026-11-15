using GymX.Domain.Common.Models;

namespace GymX.Domain.Errors;

public static class EmployeeFaceErrors
{
    public static readonly Error EmployeeNotFound = Error.NotFound(
        "Employee.NotFound",
        "Không tìm thấy nhân viên.");

    public static readonly Error FaceDataNotFound = Error.NotFound(
        "FaceData.NotFound",
        "Nhân viên chưa đăng ký FaceID.");

    public static readonly Error NoFaceFound = Error.Validation(
        "FaceRecognition.NoFaceFound",
        "Không tìm thấy khuôn mặt trong ảnh.");

    public static readonly Error MultipleFacesDetected = Error.Validation(
        "FaceRecognition.MultipleFacesDetected",
        "Phát hiện nhiều khuôn mặt trong ảnh.");

    public static readonly Error InvalidFileFormat = Error.Validation(
        "FaceRegistration.InvalidFileFormat",
        "Định dạng ảnh không hợp lệ (JPG, PNG).");

    public static readonly Error FileTooLarge = Error.Validation(
        "FaceRegistration.FileTooLarge",
        "Dung lượng ảnh vượt quá 5MB.");

    public static readonly Error FileEmpty = Error.Validation(
        "FaceRegistration.FileEmpty",
        "Vui lòng chọn ảnh khuôn mặt.");

    public static readonly Error DetectFailed = Error.Failure(
        "FaceRecognition.DetectFailed",
        "Không thể nhận diện khuôn mặt.");

    public static readonly Error AddFaceFailed = Error.Failure(
        "FaceRecognition.AddFaceFailed",
        "Lỗi lưu dữ liệu khuôn mặt.");

    public static readonly Error ServiceUnavailable = Error.Failure(
        "FaceRecognition.ServiceUnavailable",
        "Dịch vụ FaceID tạm thời không khả dụng.");

    public static readonly Error FaceAlreadyEnrolledToAnother = Error.Conflict(
        "FaceRecognition.FaceAlreadyEnrolledToAnother",
        "Khuôn mặt đã được đăng ký cho nhân viên khác.");

    public static readonly Error FaceImageBlurry = Error.Validation(
        "FaceRecognition.FaceImageBlurry",
        "Ảnh bị mờ hoặc rung lắc. Vui lòng giữ yên.");

    public static readonly Error FaceQualityLow = Error.Validation(
        "FaceRecognition.FaceQualityLow",
        "Chất lượng ảnh không đạt tiêu chuẩn.");
}
