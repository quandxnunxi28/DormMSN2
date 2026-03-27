using DormMS.Models;
using Microsoft.EntityFrameworkCore;

namespace DormMS.Repository
{
    public class BookingbedRepository : IBookingbedRepository
    {
        private readonly DormMsnContext _context;
        public BookingbedRepository(DormMsnContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetRoomSpace()
        {
            var roomSpace = await _context.Rooms.Where(x => ((x.Capacity ?? 0) - (x.Occupied ?? 0)) > 0).ToListAsync();
            return roomSpace;
        }

        public async Task<bool> GetStudentBookingbedExist(int id)
        {
            var bookingbedExist = await _context.Allotments.FirstOrDefaultAsync(b => b.UserId == id);



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
                newAllotDate = baseDate.AddMonths(4);
            }
            else
            {
                newAllotDate = baseDate.AddMonths((cycle * 4) + 4);
            }


            // 👉 Nếu chưa từng đặt → cho đặt
            if (bookingbedExist == null)
                return true;

            // 👉 Lấy ngày đã đặt trong DB
            DateOnly allotDate = bookingbedExist.AllotDate ?? DateOnly.MinValue;

            // 👉 So sánh
            if (allotDate < newAllotDate)
                return true;  // được đặt

            return false; // không được đặt
        }

        public async Task<Allotment> GetStudentBookingbeds(int id)
            {
                var bookingbedStudent =await _context.Allotments.Include(a => a.User)
            .Include(a => a.Room).ThenInclude(r => r.Hostel).FirstOrDefaultAsync(a => a.UserId == id);
                return bookingbedStudent;
            }
    }
}
