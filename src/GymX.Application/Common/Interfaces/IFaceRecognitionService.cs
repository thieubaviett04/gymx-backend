using GymX.Domain.Common.Models;

namespace GymX.Application.Common.Interfaces;

public interface IFaceRecognitionService
{
    /// <summary>
    /// Đăng ký khuôn mặt mới vào hệ thống.
    /// Trả về FaceToken (Mã khuôn mặt vĩnh viễn) để lưu vào Database của GymX.
    /// </summary>
    Task<Result<string>> RegisterFaceAsync(byte[] imageBytes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tìm kiếm khuôn mặt trong ảnh chụp từ Camera.
    /// Trả về FaceToken khớp nhất trong kho dữ liệu của Face++.
    /// </summary>
    Task<Result<string>> IdentifyFaceAsync(byte[] imageBytes, CancellationToken cancellationToken = default);
}
