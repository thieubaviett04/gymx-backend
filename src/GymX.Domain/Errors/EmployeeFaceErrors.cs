using GymX.Domain.Common.Models;

namespace GymX.Domain.Errors;

public static class EmployeeFaceErrors
{
    public static readonly Error EmployeeNotFound = Error.NotFound(
        "Employee.NotFound",
        "Không tìm thấy thông tin nhân viên.");

    public static readonly Error FaceDataNotFound = Error.NotFound(
        "FaceData.NotFound",
        "Nhân viên chưa đăng ký dữ liệu khuôn mặt.");

    public static readonly Error NoFaceFound = Error.Validation(
        "FaceRecognition.NoFaceFound",
        "Không tìm thấy khuôn mặt nào trong ảnh. Vui lòng chụp lại với ánh sáng tốt hơn.");

    public static readonly Error MultipleFacesDetected = Error.Validation(
        "FaceRecognition.MultipleFacesDetected",
        "Phát hiện nhiều hơn một khuôn mặt trong ảnh. Vui lòng chỉ chụp một khuôn mặt duy nhất.");

    public static readonly Error InvalidFileFormat = Error.Validation(
        "FaceRegistration.InvalidFileFormat",
        "Định dạng tệp không hợp lệ. Chỉ chấp nhận các định dạng ảnh JPG, JPEG, PNG, WEBP.");

    public static readonly Error FileTooLarge = Error.Validation(
        "FaceRegistration.FileTooLarge",
        "Dung lượng tệp ảnh không được vượt quá 5MB.");

    public static readonly Error FileEmpty = Error.Validation(
        "FaceRegistration.FileEmpty",
        "Vui lòng chọn tệp ảnh khuôn mặt để đăng ký.");

    public static readonly Error DetectFailed = Error.Failure(
        "FaceRecognition.DetectFailed",
        "Lỗi phát hiện khuôn mặt từ dịch vụ Face++.");

    public static readonly Error AddFaceFailed = Error.Failure(
        "FaceRecognition.AddFaceFailed",
        "Không thể lưu khuôn mặt vào hệ thống Face++.");

    public static readonly Error ServiceUnavailable = Error.Failure(
        "FaceRecognition.ServiceUnavailable",
        "Dịch vụ nhận diện khuôn mặt hiện không khả dụng. Vui lòng thử lại sau.");
}
