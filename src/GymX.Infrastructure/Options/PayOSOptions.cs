namespace GymX.Infrastructure.Options;

public class PayOSOptions
{
    public const string SectionName = "PayOS";

    public string ClientId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ChecksumKey { get; set; } = string.Empty;

    // URL FE sẽ redirect đến sau khi thanh toán (thành công / huỷ)
    // Điền URL của FE khi deploy thật, dùng localhost khi dev
    public string ReturnUrl { get; set; } = "http://localhost:3000/payment/result";
    public string CancelUrl { get; set; } = "http://localhost:3000/payment/cancel";
}
