using Booking.DataAccess.Dao;
using Booking.Models;

namespace Booking.DataAccess;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _appDbContext;

    public BookingRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    
    public async Task<IReadOnlyCollection<BookingDataDao>> GetAllBookings()
    {
        return _appDbContext.Bookings.ToList().AsReadOnly();
    }

    public async Task<BookingDataDao?> Create(BookingDataDao dao)
    {
        _appDbContext.Bookings.Add(dao);
        var result = await _appDbContext.SaveChangesAsync();
        return result == 1 ? dao : null;
    }
    
    public async Task<bool> Delete(Guid bookingId)
    {
        _appDbContext.Bookings.Remove(new BookingDataDao() {BookingId = bookingId});
        var result = await _appDbContext.SaveChangesAsync();
        return result == 1;
    }

    public async Task<BookingDataDao?> GetBooking(Guid id) => await _appDbContext.Bookings.FindAsync(id);
    
    public async Task<IReadOnlyCollection<BookingDataDao>> GetUsersBookings(Guid userId)
    {
        var usersBookings = (_appDbContext.Bookings).ToList();
        return usersBookings.Where(x => x.UserId == userId).ToList();
    }
}

public interface IBookingRepository
{
    Task<IReadOnlyCollection<BookingDataDao>> GetAllBookings();
    Task<BookingDataDao?> Create(BookingDataDao dao);
    Task<bool> Delete(Guid bookingId);
    Task<BookingDataDao?> GetBooking(Guid id);
    Task<IReadOnlyCollection<BookingDataDao>> GetUsersBookings(Guid userId);
}