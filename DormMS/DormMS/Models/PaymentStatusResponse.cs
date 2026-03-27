namespace DormMS.Models
{
    public class PaymentStatusResponse
    {
        public string OrderCode { get; set; } = null!;

        // "00" = thanh toán thành công, "01" = thất bại
        public string Code { get; set; } = null!;

        // Sửa từ int → decimal để tương thích database
        public decimal Amount { get; set; }

        public string Status { get; set; } = null!;
    }
}
