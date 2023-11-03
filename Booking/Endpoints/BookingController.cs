using Booking.Helper;
using Booking.Models;
using Booking.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class BookingController(IBookingService service) : ControllerBase
{
    [HttpGet("all", Name = "Get All Bookings")]
    public async Task<List<BookingData>> GetAllBookings() =>
        await service.GetAllBookings();

    [HttpGet(Name = "Get My Bookings")]
    public async Task<List<BookingData>> GetMyBookings() =>
        await service.GetMyBookings();

    [HttpPost(Name = "Create Booking")]
    public async Task<IActionResult> Create([FromBody] DateTime dateTime)
    {
        var result = await service.Create(dateTime);

        return ActionResult(result);
    }

    [HttpDelete(Name = "Delete Booking")]
    public async Task<IActionResult> Delete(Guid bookingId)
    {
        var result = await service.Delete(bookingId);

        return ActionResult(result);
    }

    private IActionResult ActionResult(OperationResult<BookingData> result) =>
        result.Status switch
        {
            OperationStatus.Success   => Ok(),
            OperationStatus.Created   => CreatedAtAction(nameof(GetAllBookings), result.Result),
            OperationStatus.Forbidden => BadRequest(result.Message),
            OperationStatus.Failed    => Problem(result.Message),
            OperationStatus.NotFound  => NotFound(result.Message),
            _                         => throw new ArgumentOutOfRangeException()
        };

    }
}