using Booking.DataAccess.Dao;
using Booking.Models;

namespace Booking.Mappers;

public interface IBookingDataMapper
{
    BookingDataDao ToDao(DateTime washTime, Guid userId);
    BookingData FromDao(BookingDataDao bookingDataDao);
}