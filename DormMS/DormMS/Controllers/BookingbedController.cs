using DormMS.Models;
using DormMS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DormMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingbedController : ControllerBase
    {
        private readonly IBookingbedService _bookingbedService;
        private readonly DormMsnContext _context;
        private readonly IPayOSService _payOSService;
        public BookingbedController(IBookingbedService bookingbedService, DormMsnContext context, IPayOSService payOSService)
        {
            _bookingbedService = bookingbedService;
            _context = context;
            _payOSService = payOSService;
        }

        [HttpGet("student/{id}")]
        public async Task<IActionResult> GetStudentBookingbeds(int id)
        {
            var bookingbed = await _bookingbedService.GetStudentBookingbeds(id);

            if (bookingbed == null )
            {
                return NotFound();
            }

            return Ok(bookingbed);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingBed dto)
        {
            if (dto == null || dto.UserId == 0 || dto.RoomId == 0)
                return BadRequest("Dữ liệu không hợp lệ");

            // 1️⃣ Lưu booking
            var booking = new BookingBed
            {
                UserId = dto.UserId,
                RoomId = dto.RoomId,
                Status = "New Bed",
                TotalAmount = dto.TotalAmount
            };


            _context.BookingBeds.Add(booking);
            await _context.SaveChangesAsync();

            // 2️⃣ Tạo link thanh toán PayOS
            var payment = await _payOSService.CreatePaymentAsync(booking.BookingId, booking.TotalAmount);

            // 3️⃣ Trả về link + QR
            return Ok(new
            {
                bookingId = booking.BookingId,
                paymentUrl = payment.CheckoutUrl,
                qrCode = payment.QrCode
            });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] PayOS.Models.Webhooks.Webhook webhook)
        {
            try
            {
                // 1. Kiểm tra nếu là request test hoặc dữ liệu trống từ PayOS
                if (webhook == null || webhook.Data == null)
                {
                    return Ok(new { message = "Bypass test" });
                }

                // 2. Thử xác thực chữ ký
                var data = await _payOSService.VerifyWebhookAsync(webhook);

                // 3. Logic xử lý Database của bạn
                var bookingId = (int)data.OrderCode;
                var booking = await _context.BookingBeds.FindAsync(bookingId);

                if (booking != null)
                {
                    booking.Status = "Paid";
                    booking.TotalAmount = (decimal)data.Amount;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Payment recorded" });
            }
            catch (Exception ex)
            {
                // QUAN TRỌNG: Khi PayOS nhấn "Lưu", họ sẽ gửi request test. 
                // Nếu verify lỗi (do Ngrok hoặc sai signature), ta vẫn trả về OK 200 
                // để PayOS chấp nhận URL này.
                Console.WriteLine($"Webhook Error: {ex.Message}");
                return Ok(new { message = "Webhook received with warning" });
            }
        }
        [HttpGet("payment-success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string code, [FromQuery] long orderCode)
        {
            // Trong API, chúng ta trả về JSON hoặc Redirect về trang Front-end (React/Vue/Angular)
            if (code == "00")
            {
                return Ok(new
                {
                    status = "Success",
                    message = $"Thanh toán thành công cho đơn hàng {orderCode}",
                    orderCode = orderCode
                });

                // HOẶC Redirect về trang chủ/trang lịch sử của FE:
                // return Redirect($"http://localhost:3000/payment-status?status=success&orderId={orderCode}");
            }

            return BadRequest(new { status = "Fail", message = "Mã phản hồi không hợp lệ" });
        }

        [HttpGet("payment-cancelled")]
        public async Task<IActionResult> PaymentCancelled([FromQuery] string code, [FromQuery] long orderCode)
        {
            // Xử lý khi người dùng nhấn "Hủy thanh toán" trên giao diện PayOS
            return Ok(new
            {
                status = "Cancelled",
                message = $"Giao dịch đơn hàng {orderCode} đã bị hủy bởi người dùng.",
                orderCode = orderCode
            });

            // HOẶC Redirect:
            // return Redirect($"http://localhost:3000/payment-status?status=cancelled&orderId={orderCode}");
        }

        [HttpGet("rooms/available")]
        public async Task<IActionResult> GetAvailableRooms()
        {
            var rooms = await _bookingbedService.GetRoomSpace();

            if (rooms == null || rooms.Count == 0)
                return NotFound(new { message = "Không còn phòng trống" });

            return Ok(rooms);
        }

        [HttpPost("bookingbede/{id}")]
        public async Task<IActionResult> CheckuserExist(int id)
        {
            
        }

    }
}
