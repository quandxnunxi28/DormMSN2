using DormMS.Models;

namespace DormMS.Service
{
    public interface IBookingbedService
    {
            public Task<Allotment> GetStudentBookingbeds(int id);
            public Task<BookingBed> GetStudentBookingbedExist(int id);
        public Task<List<Room>> GetRoomSpace();

    }
}
