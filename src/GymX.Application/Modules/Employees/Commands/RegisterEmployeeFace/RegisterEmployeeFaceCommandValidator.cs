using FluentValidation;

namespace GymX.Application.Modules.Employees.Commands.RegisterEmployeeFace;

public class RegisterEmployeeFaceCommandValidator : AbstractValidator<RegisterEmployeeFaceCommand>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/png", "image/webp", "image/jpg"];
    private const long MaxFileSize = 5 * 1024 * 1024;

    public RegisterEmployeeFaceCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Mã định danh nhân viên không được để trống.");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("File ảnh không được rỗng.")
            .LessThanOrEqualTo(MaxFileSize).WithMessage("Dung lượng ảnh khuôn mặt không được vượt quá 5MB.");

        RuleFor(x => x.ImageBytes)
            .NotNull().WithMessage("Dữ liệu ảnh không được để trống.")
            .Must(bytes => bytes != null && bytes.Length > 0).WithMessage("Dữ liệu ảnh không được rỗng.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("Tên file không hợp lệ.")
            .Must(fileName =>
            {
                var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
                return !string.IsNullOrEmpty(ext) && AllowedExtensions.Contains(ext);
            }).WithMessage("Định dạng file không được hỗ trợ. Chỉ chấp nhận các định dạng: .jpg, .jpeg, .png, .webp.");

        RuleFor(x => x.ContentType)
            .Must(mime => !string.IsNullOrEmpty(mime) && AllowedMimeTypes.Contains(mime.ToLowerInvariant()))
            .WithMessage("Loại tệp không hợp lệ. Phải là file hình ảnh (JPG, PNG, WEBP).");
    }
}
