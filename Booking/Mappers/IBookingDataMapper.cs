using Booking.DataAccess.Dao;
using Booking.Models;

namespace Booking.Mappers;

public interface IBookingDataMapper
{
    BookingDataDao ToDao(BookingData bookingData, Guid userId);
    BookingData FromDao(BookingDataDao bookingDataDao);
}