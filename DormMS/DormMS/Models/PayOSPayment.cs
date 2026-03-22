using System.Text.Json.Serialization;

namespace DormMS.Models
{
    public class PayOSPayment
    {
        [JsonPropertyName("order_code")]
        public string OrderCode { get; set; } = null!;

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;
    }
}
