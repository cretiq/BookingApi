namespace Booking.Helper;

public class BookingSettings
{
    public TimeSpan SlotDuration { get; set; }
    public int MaxAmountPerUser { get; set; }
    public int MaxAmountPerUserPerWeek { get; set; }
    public int OpeningTime { get; set; }
    public int ClosingTime { get; set; }
}