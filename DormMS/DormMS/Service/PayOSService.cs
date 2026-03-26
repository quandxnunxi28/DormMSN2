using System.Text.Json;
using DormMS.Models;
using Microsoft.Extensions.Configuration;
using PayOS;
using PayOS.Models;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;

namespace DormMS.Service
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOSClient _client;

        public PayOSService(IConfiguration config)
        {
            _client = new PayOSClient(
                config["PayOS:ClientId"]!,
                config["PayOS:ApiKey"]!,
                config["PayOS:ChecksumKey"]!
            );
        }

        public async Task<CreatePaymentLinkResponse> CreatePaymentAsync(int bookingId, decimal totalAmount)
        {
            var request = new CreatePaymentLinkRequest
            {
                OrderCode = bookingId,               // BookingId làm OrderCode
                Amount = (int)totalAmount,          // PayOS nhận số nguyên (VND)
                Description = $"Booking_{bookingId}",
                ReturnUrl = "https://localhost:7088/payment-success.html", // thanh toán thành công
                CancelUrl = "https://localhost:7088/payment-cancelled.html"   // hủy
            };

            return await _client.PaymentRequests.CreateAsync(request);
        }

        public async Task<WebhookData> VerifyWebhookAsync(Webhook webhook)
        {
            return await _client.Webhooks.VerifyAsync(webhook);
        }
        public async Task<PaymentStatusResponse> GetPaymentStatusAsync(int bookingId)
        {
            var paymentRaw = await _client.PaymentRequests.GetAsync(bookingId.ToString());

            var payment = JsonSerializer.Deserialize<PayOSPayment>(
                JsonSerializer.Serialize(paymentRaw),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            )!;

            return new PaymentStatusResponse
            {
                OrderCode = payment.OrderCode,
                Amount = payment.Amount,
                Code = payment.Status == "PAID" ? "00" : "01",
                Status = payment.Status
            };
        }




    }
}
