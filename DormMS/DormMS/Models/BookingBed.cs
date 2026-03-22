    namespace DormMS.Models
{
    public class BookingBed
    {
        public int BookingId { get; set; }

        public int UserId { get; set; }

        public int RoomId { get; set; }

        public string? Status { get; set; } 

        public DateTime? CreatedAt { get; set; }

        public decimal TotalAmount { get; set; } 
                                                 // Navigation (optional nếu bạn dùng FK sau này)

    }
}
