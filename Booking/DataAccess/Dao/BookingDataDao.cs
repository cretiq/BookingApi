using System.ComponentModel.DataAnnotations;

namespace Booking.DataAccess.Dao;

public class BookingDataDao
{
    [Key] public Guid BookingId { get; set; }
    
    [Required] public DateTime BookingDateTime { get; set; }
    
    [Required] public Guid UserId { get; set; }
    
    public DateTime RequestTimestamp { get; set; }
}