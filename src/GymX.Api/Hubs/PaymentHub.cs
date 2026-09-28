using Microsoft.AspNetCore.SignalR;

namespace GymX.Api.Hubs;

/// <summary>
/// PaymentHub — kết nối realtime giữa Backend với màn hình Admin và Tablet khách hàng.
///
/// Luồng hoạt động:
/// 1. FE (Tablet) kết nối và join Group theo OrderCode.
/// 2. Backend (Webhook handler) bắn sự kiện vào Group đó.
/// 3. Tablet tự render thông báo thành công / thất bại.
///
/// Sự kiện Backend → FE:
/// - "PaymentSuccess"  : Thanh toán thành công → Tablet hiện "Thành công" + chuyển luồng chụp mặt.
/// - "PaymentCancelled": Đơn hàng bị huỷ.
/// - "PaymentPending"  : Đang chờ thanh toán (dùng khi Admin reload trang).
/// </summary>
public class PaymentHub : Hub
{
    /// <summary>
    /// Client gọi hàm này để đăng ký nhận sự kiện của một đơn hàng cụ thể.
    /// </summary>
    public async Task JoinOrderGroup(string orderCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderCode}");
    }

    /// <summary>
    /// Client gọi hàm này để rời khỏi group khi đóng màn hình thanh toán.
    /// </summary>
    public async Task LeaveOrderGroup(string orderCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order-{orderCode}");
    }
}
