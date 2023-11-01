namespace Booking.Models;

public class BookingData
{
    public Guid BookingId { get; set; }

    public Guid UserId { get; set; }

    public DateTime Time { get; set; }
}