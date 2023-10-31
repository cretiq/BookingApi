using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Booking.DataAccess;
using Booking.Mappers;
using Booking.Models;
using Microsoft.AspNetCore.Http;

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

    public async Task<List<BookingData>> GetUserBookings()
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var daos = await bookingRepository.GetUsersBookings(Guid.Parse(userId!));
        return daos.Select(bookingDataMapper.FromDao).ToList();
    }

    public async Task<BookingData?> Book(DateTime washTime)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var dao = bookingDataMapper.ToDao(washTime, Guid.Parse(userId!));
        var result = await bookingRepository.Create(dao);
        return result == null ? null : bookingDataMapper.FromDao(result);
    }

    public async Task<bool> Delete(Guid id)
    {
        var booking = await GetBooking(id);
        if (booking == null) return false;

        if (await IsBookingMadeByUser(id,
                Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!)))
            return false;

        await bookingRepository.Delete(id);

        return true;
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
    Task<List<BookingData>> GetUserBookings();
    Task<List<BookingData>> GetAllBookings();

    Task<BookingData?> Book(DateTime washTime);

    Task<bool> Delete(Guid id);
}