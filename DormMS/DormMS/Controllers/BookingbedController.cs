using DormMS.Models;
using DormMS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PayOS.Models.Webhooks;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace DormMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingbedController : ControllerBase
    {
        private readonly IBookingbedService _bookingbedService;
        private readonly DormMsnContext _context;
        private readonly IPayOSService _payOSService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public BookingbedController(IBookingbedService bookingbedService, DormMsnContext context, IPayOSService payOSService, IHubContext<NotificationHub> hubContext)
        {
            _bookingbedService = bookingbedService;
            _context = context; 
            _payOSService = payOSService;
            _hubContext = hubContext;
        }

        [HttpGet("student/{id}")]
        public async Task<IActionResult> GetStudentBookingbeds(int id)
        {
            var bookingbed = await _bookingbedService.GetStudentBookingbeds(id);

            if (bookingbed == null)
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

            var roomOccupied = _context.Rooms.Where(x => x.RoomId == dto.RoomId).FirstOrDefault();
            roomOccupied.Occupied += 1;

            var paymentEntity = new Payment
            {
                UserId = booking.UserId,
                Amount = booking.TotalAmount,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Method = "Online",
                Description = "null nhe!",
                Status = "Pending"
            };

            _context.Payments.Add(paymentEntity);
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
        [HttpPost("createEx")]
        public async Task<IActionResult> CreateBookingg([FromBody] BookingBed dto)
        {
            if (dto == null || dto.UserId == 0 || dto.RoomId == 0)
                return BadRequest("Dữ liệu không hợp lệ");

            // 1️⃣ Lưu booking
            var booking = new BookingBed
            {
                UserId = dto.UserId,
                RoomId = dto.RoomId,
                Status = "Pending",
                TotalAmount = dto.TotalAmount
            };
            //check userId TỒN TẠI 

            _context.BookingBeds.Add(booking);

           
            
            
            var paymentEntity = new Payment
            {
                UserId = booking.UserId,
                Amount = booking.TotalAmount,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Method = "Online",
                Description = "null nhe!",
                Status = "Pending"
            };

            _context.Payments.Add(paymentEntity);
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


        //[HttpGet("payment-success")]
        //public async Task<IActionResult> PaymentSuccess([FromQuery] string code, [FromQuery] long orderCode)
        //{
        //    // Trong API, chúng ta trả về JSON hoặc Redirect về trang Front-end (React/Vue/Angular)
        //    if (code == "00")
        //    {
        //        var bookID = orderCode;
        //        if (bookID == null)

        //            return BadRequest();

        //        var bookingBedId = _context.BookingBeds.Where(x => x.BookingId == bookID).FirstOrDefault();
        //        if (bookingBedId == null)
        //        {
        //            return BadRequest();
        //        }
        //        var userIdd = _context.Allotments.Where(x => x.UserId == bookingBedId.UserId).FirstOrDefault();
        //        Allotment a = new Allotment
        //        {
        //            UserId = bookingBedId.UserId,   
        //            RoomId = bookingBedId.RoomId,
        //            AllotDate = userIdd.AllotDate.HasValue
        //? userIdd.AllotDate.Value.AddMonths(4)
        //: DateOnly.FromDateTime(DateTime.Now).AddMonths(4),
        //            LeaveDate = null,
        //            Status = "Active"
        //        };
        //        _context.Allotments.Add(a);
        //        return Ok(new
        //        {
        //            status = "Success",
        //            message = $"Thanh toán thành công cho đơn hàng {orderCode}",
        //            orderCode = orderCode
        //        });


        //        // HOẶC Redirect về trang chủ/trang lịch sử của FE:
        //        // return Redirect($"http://localhost:3000/payment-status?status=success&orderId={orderCode}");
        //    }

        //    return BadRequest(new { status = "Fail", message = "Mã phản hồi không hợp lệ" });
        //}


        [HttpGet("payment-success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string code, [FromQuery] long orderCode)
        {
            if (code != "00")
            {
                return BadRequest(new { status = "Fail" });
            }

            var booking = await _context.BookingBeds
                .FirstOrDefaultAsync(x => x.BookingId == orderCode);

            if (booking == null)
                return BadRequest("Không tìm thấy booking");

            // ✅ Update trạng thái thanh toán
            booking.Status = "Accepted";

            // ✅ Tạo allotment
            var oldAllot = await _context.Allotments
                .Where(x => x.UserId == booking.UserId)
                .OrderByDescending(x => x.AllotDate)
                .FirstOrDefaultAsync();

            //thuật toán tính tháng






            DateOnly baseDate = new DateOnly(2025, 12, 10); // mốc kỳ đầu tiên

            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            // Nếu đã có allotment → giữ logic cũ
            DateOnly newAllotDate;

            if (oldAllot != null && oldAllot.AllotDate.HasValue)
            {
                oldAllot.AllotDate = oldAllot.AllotDate.Value.AddMonths(4);
                var paymentId=  _context.Payments.Where(x => x.UserId == booking.UserId).OrderByDescending(x => x.PaymentId).FirstOrDefault();
                if(paymentId != null)
                {
                    paymentId.Status = "Paid";
                }

                await _hubContext.Clients.User(booking.UserId.ToString())
.SendAsync(
    "ReceiveNotification", // tên event bên JS
    "🎉 Thanh toán thành công! Phòng đã được xác nhận."
);
            }
            else
            {
                // 👉 Tính kỳ gần nhất theo chu kỳ 4 tháng
                int monthsDiff = ((today.Year - baseDate.Year) * 12 + today.Month - baseDate.Month);

                int cycle = monthsDiff / 4;

                newAllotDate = baseDate.AddMonths(cycle * 4);

                // Nếu hôm nay đã qua kỳ đó → nhảy sang kỳ tiếp
                if (newAllotDate < today)
                {
                    newAllotDate = newAllotDate.AddMonths(4);
                }
                var newAllot = new Allotment
                {
                    UserId = booking.UserId,
                    RoomId = booking.RoomId,
                    AllotDate = newAllotDate,
                    LeaveDate = null,
                    Status = "Active"
                };
                _context.Allotments.Add(newAllot);

                var payment = await _context.Payments
.FirstOrDefaultAsync(x => x.UserId == booking.UserId);

                if (payment != null)
                {
                    payment.Status = "Paid";
                }
                await _hubContext.Clients.User(booking.UserId.ToString())
.SendAsync(
"ReceiveNotification", // tên event bên JS
"🎉 Thanh toán thành công! Phòng đã được xác nhận."
);


            }






            // mêmmemmemememe










            // ✅ Lưu DB
            await _context.SaveChangesAsync();



        
            return Ok(new
            {
                status = "Success",
                orderCode
            });
        }
        [HttpGet("check-payment/{bookingId}")]
        public async Task<IActionResult> CheckPayment(int bookingId)
        {
            var booking = await _context.BookingBeds.FindAsync(bookingId);

            if (booking == null)
                return NotFound();

            return Ok(new
            {
                status = booking.Status
            });
        }

        [HttpGet("payment-cancelled")]
        public async Task<IActionResult> PaymentCancelled([FromQuery] string code, [FromQuery] long orderCode)
        {

            var bookingbedId = _context.BookingBeds.Where(x => x.BookingId == orderCode).FirstOrDefault();
            var allotmentId = _context.Allotments.Where(x => x.UserId == bookingbedId.UserId).FirstOrDefault();
            if (bookingbedId != null)
            {
                
                //nếu trong allotment vẫn còn kaka thì
            if(allotmentId.UserId == null) {
                    var roomId = _context.Rooms.Where(x => x.RoomId == bookingbedId.RoomId).FirstOrDefault();
                if (roomId != null)
                {
                    roomId.Occupied -= 1;
                    _context.SaveChanges();
                } 
                }


                    
                
                
                


                var payment = await _context.Payments
                    .FirstOrDefaultAsync(x => x.UserId == bookingbedId.UserId);

                if (payment != null)
                {
                    _context.Payments.Remove(payment);
                }


                //thông báo 
                await _hubContext.Clients.User(bookingbedId.UserId.ToString())
        .SendAsync(
            "ReceiveNotification",
            "❌ Thanh toán đã bị hủy."
        );
                //xóa
                _context.Remove(bookingbedId);

                await _context.SaveChangesAsync();
            }

            // Xử lý khi người dùng nhấn "Hủy thanh toán" trên giao diện PayOS
            return Ok(new
            {
                status = "Cancelled",
                message = $"Giao dịch đơn hàng {orderCode} đã bị hủy bởi người dùng.",
                orderCode = orderCode
            });

            
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
            var userExist = await _bookingbedService.GetStudentBookingbedExist(id);
            return Ok(new
            {
                exists = userExist
            });
        }
        [HttpGet("check-booking-phase/{userId}")]
        public async Task<IActionResult> CheckBookingPhase(int userId)
        {
            string phase;
            var allotment = await _context.Allotments
                .Where(a => a.UserId == userId && a.Status == "Active")
                .OrderByDescending(a => a.AllotDate)
                .FirstOrDefaultAsync();

            if (allotment == null || !allotment.AllotDate.HasValue)
            {
                var todayy = DateOnly.FromDateTime(DateTime.Now);
                //222
                DateOnly baseDate = new DateOnly(2025, 12, 10); // mốc kỳ đầu tiên
                    
                //DateOnly today = DateOnly.FromDateTime(DateTime.Now);

                // Nếu đã có allotment → giữ logic cũ
                DateOnly newAllotDate;



                // 👉 Tính kỳ gần nhất theo chu kỳ 4 tháng
                int monthsDiff = ((todayy.Year - baseDate.Year) * 12 + todayy.Month - baseDate.Month);

                int cycle = monthsDiff / 4;

                if (cycle == 0)
                {
                    newAllotDate = baseDate;
                }
                else
                {
                    newAllotDate = baseDate.AddMonths((cycle * 4) - 4);
                }




                //2223

                var globalStartDate = newAllotDate;
                var globalEndDate = globalStartDate.AddMonths(4);//10/4/2026
                var openDatee = globalEndDate.AddDays(-20);//21/3/2026
                var phase1Endd = openDatee.AddDays(10);////1/4/2026

                bool canNewBookingg = false;

                if (todayy < openDatee)
                {
                    phase = "NOT_OPEN";
                }
                else if (todayy >= openDatee && todayy < phase1Endd)
                {
                    phase = "RENEW"; // nhưng user chưa có phòng → thực tế không renew được
                }
                else if (todayy >= phase1Endd && todayy <= globalEndDate)
                {
                    phase = "NEW";
                    canNewBookingg = true;
                }
                else
                {
                    phase = "EXPIRED";
                }

                return Ok(new
                {
                    phase,
                    canRenew = false,
                    canNewBookingg,
                    message = "Sinh viên chưa có phòng"
                });

            }

            // Lấy ngày hiện tại kiểu DateOnly để so sánh chính xác với các mốc ngày
            var today = DateOnly.FromDateTime(DateTime.Now);

            // 🔥 1. Tính các mốc thời gian (Kiểu DateOnly)
            var startDate = allotment.AllotDate.Value;
            var endDate = startDate.AddMonths(4);
            var openDate = endDate.AddDays(-20);    // Mở cổng trước 20 ngày
            var phase1End = openDate.AddDays(10);   // Giai đoạn RENEW kéo dài 10 ngày


            bool canRenew = false;
            bool canNewBooking = false;

            // 🔥 2. Xác định giai đoạn (So sánh DateOnly với DateOnly)
            if (today < openDate)
            {
                phase = "NOT_OPEN";
            }
            else if (today >= openDate && today < phase1End)
            {
                phase = "RENEW"; // Giai đoạn giữ chỗ (Gia hạn)
                canRenew = true;
            }
            else if (today >= phase1End && today <= endDate)
            {
                phase = "NEW"; // Giai đoạn đăng ký mới (Công khai)
                canNewBooking = true;
            }
            else
            {
                phase = "EXPIRED"; // Đã quá hạn endDate
            }

            return Ok(new
            {
                startDate,
                endDate,
                openDate,
                phase1End,
                today,
                phase,
                canRenew,
                canNewBooking,
                message = phase switch
                {
                    "RENEW" => "Đang trong thời gian gia hạn (giữ chỗ cũ)",
                    "NEW" => "Đang trong thời gian đăng ký phòng mới",
                    "NOT_OPEN" => $"Chưa mở đăng ký (Sẽ mở vào ngày {openDate:dd/MM/yyyy})",
                    "EXPIRED" => "Đã hết hạn đăng ký cho kỳ này",
                    _ => "Không xác định"
                }
            });
        }

        [HttpDelete("cancel-allotment/{userId}")]
        public async Task<IActionResult> CancelAndRemoveAllotment(int userId)
        {
            // 1. Tìm bản ghi giữ chỗ (Keep bed) mới nhất của User này
            var allotment = await _context.Allotments
                .Where(a => a.UserId == userId && a.Status == "Keep bed")
                .OrderByDescending(a => a.AllotDate)
                .FirstOrDefaultAsync();

            if (allotment == null)
            {
                return NotFound("Không tìm thấy bản ghi giữ chỗ cần hủy.");
            }

            try
            {
                // 2. Xóa bản ghi khỏi Database
                _context.Allotments.Remove(allotment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã hủy giữ chỗ thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi xóa dữ liệu: " + ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("expired-users")]
        public async Task<IActionResult> GetExpiredUsers()
        {
            DateOnly baseDate = new DateOnly(2025, 12, 10);
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            int monthsDiff = ((today.Year - baseDate.Year) * 12 + today.Month - baseDate.Month);
            int cycle = monthsDiff / 4;

            DateOnly nextCycle = baseDate.AddMonths((cycle + 1) * 4);

            var expired = await _context.Allotments
                .Where(x => x.AllotDate < nextCycle)
                .Select(x => new
                {
                    x.UserId,
                    x.RoomId,
                    x.AllotDate
                })
                .ToListAsync();

            return Ok(expired);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-expired")]
        public async Task<IActionResult> DeleteExpired()
        {
            DateOnly baseDate = new DateOnly(2025, 12, 10);
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            int monthsDiff = ((today.Year - baseDate.Year) * 12 + today.Month - baseDate.Month);
            int cycle = monthsDiff / 4;

            DateOnly nextCycle = baseDate.AddMonths((cycle + 1) * 4);

            var expired = await _context.Allotments
                .Where(x => x.AllotDate < nextCycle)
                .ToListAsync();


            var roomGroups = expired
        .GroupBy(x => x.RoomId)
        .Select(g => new
        {
            RoomId = g.Key,
            Count = g.Count()
        })
        .ToList();
            foreach (var group in roomGroups)
            {
                var room = await _context.Rooms.FindAsync(group.RoomId);
                if (room != null)
                {
                    room.Occupied -= group.Count;

                    // tránh âm
                    if (room.Occupied < 0)
                        room.Occupied = 0;
                }
            }

            _context.Allotments.RemoveRange(expired);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa" });
        }
        [HttpGet("id")]

        public async Task<IActionResult> test()
        {
            DateOnly baseDate = new DateOnly(2025, 12, 10);
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            int monthsDiff = ((today.Year - baseDate.Year) * 12 + today.Month - baseDate.Month);
            int cycle = monthsDiff / 4;

            DateOnly nextCycle = baseDate.AddMonths((cycle + 1) * 4);
            var expired = await _context.Allotments
               .Where(x => x.AllotDate < nextCycle)
               .ToListAsync();

            var roomGroups = expired
                    .GroupBy(x => x.RoomId)
                    .Select(g => new
                    {
                        RoomId = g.Key,
                        Count = g.Count()
                    })
                    .ToList();
            return Ok(roomGroups);
        }

        [HttpGet("test-signal")]
        public async Task<IActionResult> TestSignal()
        {
            await _hubContext.Clients.All
                .SendAsync("ReceiveNotification", "🔥 Test realtime nè!");

            return Ok();
        }


    }
}
