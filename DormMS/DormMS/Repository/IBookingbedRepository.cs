using DormMS.Models;

namespace DormMS.Repository
{
    public interface IBookingbedRepository
    {
        //public Task<List<BookResponseDto>> GetBooks();
        //public Task<BookResponseDto> GetBook(int id);
        //public Task<string> CreateBook(bookdto b);
        //public Task UpdateBook(int id, Book book);

        //public Task DeleteBook(int id);

        public Task<Allotment> GetStudentBookingbeds(int id);
        public Task<List<Room>> GetRoomSpace();

        public Task<BookingBed> GetStudentBookingbedExist(int id);

    }
}
