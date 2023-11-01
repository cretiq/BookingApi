using Booking.DataAccess.Dao;
using Booking.Models;

namespace Booking.Mappers;

public class BookingDataMapper : IBookingDataMapper
{
    public BookingDataDao ToDao(DateTime washTime, Guid userId) =>
        new()
        {
            BookingDateTime = washTime,
            UserId = userId,
            RequestTimestamp = DateTime.Now
        };

    public BookingData FromDao(BookingDataDao bookingDataDao) =>
        new()
        {
            Time = bookingDataDao.BookingDateTime,
            UserId = bookingDataDao.UserId,
            BookingId = bookingDataDao.BookingId
        };
}