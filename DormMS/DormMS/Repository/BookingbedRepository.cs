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

        public async Task<BookingBed> GetStudentBookingbedExist(int id)
        {
            var bookingbedExist =await _context.BookingBeds.FirstOrDefaultAsync(b => b.UserId == id);
            return bookingbedExist;
        }

            public async Task<Allotment> GetStudentBookingbeds(int id)
            {
                var bookingbedStudent =await _context.Allotments.Include(a => a.User)
            .Include(a => a.Room).ThenInclude(r => r.Hostel).FirstOrDefaultAsync(a => a.UserId == id);
                return bookingbedStudent;
            }
    }
}
