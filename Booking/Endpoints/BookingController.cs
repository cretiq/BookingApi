using System.ComponentModel.DataAnnotations;
using Booking.Helper;
using Booking.Models;
using Booking.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class BookingController(IBookingService service) : ControllerBase
{
    [HttpGet("all", Name = "Get All Bookings")]
    [Authorize]
    public async Task<List<BookingData>> GetAllBookings() => await service.GetAllBookings();

    [HttpGet(Name = "Get My Bookings")]
    [Authorize]
    public async Task<List<BookingData>> GetMyBookings() => await service.GetMyBookings();

    [HttpPost(Name = "Create Booking")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] DateTime dateTime)
    {
        var result = await service.Create(dateTime);
        
        return result.Status switch
        {
            OperationStatus.Success => CreatedAtAction(nameof(GetAllBookings), result.Result),
            OperationStatus.Forbidden => Conflict(result.Message),
            OperationStatus.Failed => Problem(result.Message),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpDelete(Name = "Delete Booking")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid bookingId)
    {
        var result = await service.Delete(bookingId);

        return result.Status switch
        {
            OperationStatus.Success => Ok(),
            OperationStatus.NotFound => NotFound(result.Message),
            OperationStatus.Forbidden => BadRequest(result.Message),
            OperationStatus.Failed => Problem(result.Message),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}