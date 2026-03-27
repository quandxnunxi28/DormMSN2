
using DormMS.Models;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;

namespace DormMS.Service
{
    public interface IPayOSService
    {
        Task<CreatePaymentLinkResponse> CreatePaymentAsync(
            int orderId,
            decimal amount
        );

        Task<WebhookData> VerifyWebhookAsync(Webhook webhook);
        Task<PaymentStatusResponse> GetPaymentStatusAsync(int orderId);

    }

}
