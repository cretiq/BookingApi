using Booking.DataAccess.Dao;
using Booking.Models;
using Booking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingController(IBookingService service)
    {
        _service = service;
    }

    [HttpGet("all", Name = "Get All Bookings")]
    [Authorize]
    public async Task<List<BookingData>> GetAllBookings()
    {
        return await _service.GetAllBookings();
    }
    
    [HttpGet(Name = "Get My Bookings")]
    [Authorize]
    public async Task<List<BookingData>> GetMyBookings()
    {
        return await _service.GetUserBookings();
    }

    [HttpPost(Name = "Create Booking")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] BookingData bookingData)
    {
        var result = await _service.Book(bookingData);
        return result == null ? BadRequest() : CreatedAtAction(nameof(GetAllBookings), result);
    }
    
    [HttpDelete(Name = "Delete Booking")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid bookingId)
    {
        var result = await _service.Delete(bookingId);
        return result ? Ok() : BadRequest();
    }
}