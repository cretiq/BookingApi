using Booking.Helper;
using Booking.Models;
using Booking.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class BookingController(IBookingService service, IEndPointHelper endPointHelper) : ControllerBase
{
    [HttpGet("all", Name = "Get All Bookings")]
    public async Task<List<BookingData>> GetAllBookings() => await service.GetAllBookings();

    [HttpGet(Name = "Get My Bookings")]
    public async Task<List<BookingData>> GetMyBookings() => await service.GetMyBookings();

    [HttpPost(Name = "Create Booking")]
    public async Task<IResult> Create([FromBody] DateTime dateTime) => await endPointHelper.Run(dateTime, () => service.Create(dateTime));

    [HttpDelete(Name = "Delete Booking")]
    public async Task<IResult> Delete(Guid bookingId) => await endPointHelper.Run(() => service.Delete(bookingId));
}