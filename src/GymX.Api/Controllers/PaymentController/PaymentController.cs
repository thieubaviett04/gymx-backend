using GymX.Api.Common.Controllers;
using GymX.Application.Features.Payments.Commands.CreateInvoice;
using GymX.Application.Features.Payments.Commands.HandleWebhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PayOS;
using PayOS.Models.Webhooks;

namespace GymX.Api.Controllers.PaymentController;

[Route("api/v1/payments")]
public class PaymentController(ISender sender, PayOSClient payOSClient) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentApiRequest request)
    {
        var command = new CreateInvoiceCommand(
            MemberId: request.MemberId,
            Amount: request.Amount,
            Description: request.Description ?? "Thanh toan GymX",
            BuyerName: request.BuyerName,
            BuyerEmail: request.BuyerEmail,
            BuyerPhone: request.BuyerPhone
        );

        var result = await sender.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook([FromBody] Webhook webhookBody)
    {
        try
        {
            // 1. Verify chữ ký bằng PayOS SDK
            var webhookData = await payOSClient.Webhooks.VerifyAsync(webhookBody);

            // 2. Lấy thông tin đơn hàng
            var orderCode = webhookData.OrderCode;
            var success = webhookData.Code == "00";

            var command = new HandlePaymentWebhookCommand(orderCode, success);
            var result = await sender.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { success = false, message = result.Error.Description });
            }

            // Theo document của PayOS, trả về JSON với format cụ thể
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public record CreatePaymentApiRequest(
    Guid? MemberId,
    decimal Amount,
    string? Description,
    string? BuyerName,
    string? BuyerEmail,
    string? BuyerPhone);
