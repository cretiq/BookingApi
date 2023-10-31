using Booking.DataAccess.Dao;
using Booking.Models;

namespace Booking.DataAccess;

public class BookingRepository(AppDbContext appDbContext) : IBookingRepository
{
    public async Task<IReadOnlyCollection<BookingDataDao>> GetAllBookings() => appDbContext.Bookings.ToList().AsReadOnly();

    public async Task<BookingDataDao?> Create(BookingDataDao dao)
    {
        appDbContext.Bookings.Add(dao);
        var result = await appDbContext.SaveChangesAsync();
        return result == 1 ? dao : null;
    }

    public async Task<bool> Delete(Guid bookingId)
    {
        appDbContext.Bookings.Remove(new BookingDataDao
            {BookingId = bookingId});
        var result = await appDbContext.SaveChangesAsync();
        return result == 1;
    }

    public async Task<BookingDataDao?> GetBooking(Guid id) => await appDbContext.Bookings.FindAsync(id);

    public async Task<IReadOnlyCollection<BookingDataDao>> GetUsersBookings(Guid userId)
    {
        var usersBookings = appDbContext.Bookings.ToList();
        return usersBookings.Where(x => x.UserId == userId).ToList();
    }
}

public interface IBookingRepository
{
    Task<BookingDataDao?> GetBooking(Guid id);
    Task<IReadOnlyCollection<BookingDataDao>> GetUsersBookings(Guid userId);
    Task<IReadOnlyCollection<BookingDataDao>> GetAllBookings();
    Task<BookingDataDao?> Create(BookingDataDao dao);
    Task<bool> Delete(Guid bookingId);
}