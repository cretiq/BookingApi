using System.Security.Claims;
using Booking.DataAccess;
using Booking.Helper;
using Booking.Mappers;
using Booking.Models;

namespace Booking.Services;

public class BookingService(
        IBookingRepository bookingRepository,
        IBookingDataMapper bookingDataMapper,
        IHttpContextAccessor httpContextAccessor
    )
    : IBookingService
{
    public async Task<BookingData?> GetBooking(Guid id)
    {
        var dao = await bookingRepository.GetBooking(id);
        return dao == null ? null : bookingDataMapper.FromDao(dao);
    }

    public async Task<List<BookingData>> GetMyBookings()
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var daos = await bookingRepository.GetUsersBookings(Guid.Parse(userId!));
        return daos.Select(bookingDataMapper.FromDao).ToList();
    }

    public async Task<BookingData?> Create(DateTime washTime)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var dao = bookingDataMapper.ToDao(washTime, Guid.Parse(userId!));
        var result = await bookingRepository.Create(dao);
        return result == null ? null : bookingDataMapper.FromDao(result);
    }

    public async Task<Operation> Delete(Guid id)
    {
        var booking = await GetBooking(id);
        if (booking == null) return Operation.NotFound();

        if (await IsBookingMadeByUser(id, Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!)))
            return Operation.Forbidden("The booking is not made by you");

        await bookingRepository.Delete(id);
        return Operation.Success();
    }

    public async Task<List<BookingData>> GetAllBookings()
    {
        var daos = await bookingRepository.GetAllBookings();
        return daos.Select(bookingDataMapper.FromDao).ToList();
    }

    public async Task<bool> IsBookingMadeByUser(Guid bookingId, Guid userId)
    {
        var dao = await bookingRepository.GetBooking(bookingId);
        if (dao == null) return false;

        return dao.UserId == userId;
    }
}

public interface IBookingService
{
    Task<BookingData?> GetBooking(Guid id);
    Task<List<BookingData>> GetMyBookings();
    Task<List<BookingData>> GetAllBookings();
    Task<BookingData?> Create(DateTime washTime);
    Task<Operation> Delete(Guid id);
}