using DormMS.Models;

namespace DormMS.Service
{
    public interface IBookingbedService
    {
            public Task<Allotment> GetStudentBookingbeds(int id);
            public Task<bool> GetStudentBookingbedExist(int id);
        public Task<List<Room>> GetRoomSpace();

    }
}
