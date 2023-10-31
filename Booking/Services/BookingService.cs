using System.Security.Claims;
using Booking.DataAccess;
using Booking.DataAccess.Dao;
using Booking.Mappers;
using Booking.Models;

namespace Booking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingDataMapper _bookingDataMapper;
    private readonly IBookingRepository _bookingRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BookingService(
        IBookingRepository bookingRepository,
        IBookingDataMapper bookingDataMapper,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _bookingRepository = bookingRepository;
        _bookingDataMapper = bookingDataMapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BookingData?> GetBooking(Guid id)
    {
        var dao = await _bookingRepository.GetBooking(id);
        return dao == null ? null : _bookingDataMapper.FromDao(dao);
    }

    public async Task<List<BookingData>> GetUserBookings()
    {
        var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var daos = await _bookingRepository.GetUsersBookings(Guid.Parse(userId!));
        return daos.Select(_bookingDataMapper.FromDao).ToList();
    }

    public async Task<BookingData?> Book(BookingData bookingData)
    {
        var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var dao = _bookingDataMapper.ToDao(bookingData, Guid.Parse(userId!));
        var result = await _bookingRepository.Create(dao);
        return result == null ? null : _bookingDataMapper.FromDao(result);
    }

    public async Task<bool> Delete(Guid id)
    {
        var booking = await GetBooking(id);
        if (booking == null) return false;

        if (await IsBookingMadeByUser(id,
                Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!)))
            return false;

        await _bookingRepository.Delete(id);

        return true;
    }

    public async Task<bool> IsBookingMadeByUser(Guid bookingId, Guid userId)
    {
        var dao = await _bookingRepository.GetBooking(bookingId);
        if (dao == null) return false;

        return dao.UserId == userId;
    }

    public async Task<List<BookingData>> GetAllBookings()
    {
        var daos = await _bookingRepository.GetAllBookings();
        return daos.Select(_bookingDataMapper.FromDao).ToList();
    }
}

public interface IBookingService
{
    Task<BookingData?> GetBooking(Guid id);
    Task<List<BookingData>> GetUserBookings();
    Task<List<BookingData>> GetAllBookings();
    
    Task<BookingData?> Book(BookingData bookingData);
    
    Task<bool> Delete(Guid id);
}