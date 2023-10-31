using Booking.DataAccess.Dao;
using Booking.Models;

namespace Booking.Mappers;

public class BookingDataMapper : IBookingDataMapper
{
    public BookingDataDao ToDao(BookingData bookingData, Guid userId)
    {
        return new BookingDataDao
        {
            BookingDateTime = bookingData.Time,
            UserId = userId,
            RequestTimestamp = DateTime.Now
        };
    }
    
    public BookingData FromDao(BookingDataDao bookingDataDao)
    {
        return new BookingData
        {
            Time = bookingDataDao.BookingDateTime
        };
    }
}