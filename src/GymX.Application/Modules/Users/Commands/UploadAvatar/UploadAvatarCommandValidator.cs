using FluentValidation;

namespace GymX.Application.Modules.Users.Commands.UploadAvatar;

public class UploadAvatarCommandValidator : AbstractValidator<UploadAvatarCommand>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/png", "image/webp", "image/jpg"];
    private const long MaxFileSize = 5 * 1024 * 1024;

    public UploadAvatarCommandValidator()
    {
        RuleFor(x => x.Length)
            .GreaterThan(0).WithMessage("File ảnh không được rỗng.")
            .LessThanOrEqualTo(MaxFileSize).WithMessage("Dung lượng ảnh đại diện không được vượt quá 5MB.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("Tên file không hợp lệ.")
            .Must(fileName =>
            {
                var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
                return !string.IsNullOrEmpty(ext) && AllowedExtensions.Contains(ext);
            }).WithMessage("Định dạng file không được hỗ trợ. Chỉ chấp nhận các định dạng: .jpg, .jpeg, .png, .webp.");

        RuleFor(x => x.ContentType)
            .Must(mime => !string.IsNullOrEmpty(mime) && AllowedMimeTypes.Contains(mime.ToLowerInvariant()))
            .WithMessage("Loại file không hợp lệ. Phải là file hình ảnh.");
    }
}
