using Booking.DataAccess.Dao;
using Booking.Helper;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.DataAccess;

public class BookingRepository(AppDbContext appDbContext) : IBookingRepository
{
    public async Task<IReadOnlyCollection<BookingDataDao>> GetAllBookings() =>
        appDbContext.Bookings.ToList().AsReadOnly();

    public async Task<BookingDataDao?> Create(BookingDataDao dao)
    {
        appDbContext.Bookings.Add(dao);
        var result = await appDbContext.SaveChangesAsync();
        return result == 1 ?
            dao :
            null;
    }

    public async Task<Operation> Delete(Guid bookingId)
    {
        var booking = await appDbContext.Bookings.FindAsync(bookingId);
        if (booking == null)
            return Error.NotFound();

        appDbContext.Bookings.Remove(booking);

        var result = await appDbContext.SaveChangesAsync();

        return result == 1 ?
            Operation.Success() :
            Error.Unknown();
    }

    public async Task<bool> IsConflicting(DateTime bookingTime, TimeSpan slotDuration)
    {
        var startTime = bookingTime - slotDuration;
        var endTime = bookingTime + slotDuration;

        //Add and subtract one minute to not collide on whole hour
        startTime = startTime.AddMinutes(1);
        endTime = endTime.AddMinutes(-1);

        return await appDbContext.Bookings.Where(b => b.BookingDateTime <= endTime && b.BookingDateTime >= startTime).AnyAsync();
    }

    public async Task<BookingDataDao?> GetBooking(Guid id) =>
        await appDbContext.Bookings.FindAsync(id);

    public async Task<IReadOnlyCollection<BookingDataDao>> GetUsersBookings(Guid userId)
    {
        var usersBookings = appDbContext.Bookings.Where(x => x.UserId == userId).ToList();
        return usersBookings;
    }
}

public interface IBookingRepository
{
    Task<BookingDataDao?> GetBooking(Guid id);
    Task<IReadOnlyCollection<BookingDataDao>> GetUsersBookings(Guid userId);
    Task<IReadOnlyCollection<BookingDataDao>> GetAllBookings();
    Task<BookingDataDao?> Create(BookingDataDao dao);
    Task<Operation> Delete(Guid bookingId);
    Task<bool> IsConflicting(DateTime bookingTime, TimeSpan slotDuration);
}