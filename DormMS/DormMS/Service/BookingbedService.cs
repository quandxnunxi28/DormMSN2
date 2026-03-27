using DormMS.Models;
using DormMS.Repository;

namespace DormMS.Service
{
    public class BookingbedService : IBookingbedService
    {
        private readonly IBookingbedRepository _bookingbedRepository;
        public BookingbedService(IBookingbedRepository bookingbedRepository)
        {
            _bookingbedRepository = bookingbedRepository;
        }

        public Task<List<Room>> GetRoomSpace()
        {
            return _bookingbedRepository.GetRoomSpace();
        }

        public Task<bool> GetStudentBookingbedExist(int id)
        {
            return _bookingbedRepository.GetStudentBookingbedExist(id);
        }

        public Task<Allotment> GetStudentBookingbeds(int id)
        {
         return  _bookingbedRepository.GetStudentBookingbeds(id);
             
        }
    }
}
