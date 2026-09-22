namespace GymX.Infrastructure.Options;

public class FacePlusPlusOptions
{
    public const string SectionName = "FacePlusPlus";

    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;

    // [Fix 3] BaseUrl đặt trong config để có thể đổi từ US sang Asia mà không cần rebuild
    public string BaseUrl { get; set; } = "https://api-us.faceplusplus.com";

    // Token của bộ sưu tập khuôn mặt (FaceSet) dành riêng cho GymX trên Face++
    // Cần tạo FaceSet 1 lần qua API rồi lưu token này vào đây
    public string FaceSetToken { get; set; } = string.Empty;
}
