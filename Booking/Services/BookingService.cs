using System.Security.Claims;
using Booking.DataAccess;
using Booking.Helper;
using Booking.Mappers;
using Booking.Models;
using Microsoft.Extensions.Options;

namespace Booking.Services;

public class BookingService(
        IBookingRepository bookingRepository,
        IBookingDataMapper bookingDataMapper,
        IHttpContextAccessor httpContextAccessor,
        IOptions<BookingSettings> bookingSettings
    )
    : IBookingService
{
    private readonly BookingSettings _bookingSettings = bookingSettings.Value;

    public async Task<List<BookingData>> GetMyBookings()
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var daos = await bookingRepository.GetUsersBookings(Guid.Parse(userId!));
        return daos.Select(bookingDataMapper.FromDao).ToList();
    }

    public async Task<OperationResult<BookingData>> Create(DateTime bookingTime)
    {
        var validation = await ValidateBookingTimeslot(bookingTime);

        if (validation.Status != OperationStatus.Success)
            return Operation.Forbidden<BookingData>(validation.Message);

        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var dao = bookingDataMapper.ToDao(bookingTime, Guid.Parse(userId!));
        var result = await bookingRepository.Create(dao);

        return result != null ? 
            OperationResult<BookingData>.Success(bookingDataMapper.FromDao(dao)) : 
            Operation.Failed<BookingData>();
    }

    public async Task<Operation> Delete(Guid id)
    {
        if (!await IsBookingMadeByUser(id))
            return Operation.Forbidden("This booking is not made by you");

        var result = await bookingRepository.Delete(id);
        return result;
    }

    public async Task<List<BookingData>> GetAllBookings()
    {
        var daos = await bookingRepository.GetAllBookings();
        return daos.Select(bookingDataMapper.FromDao).ToList();
    }

    public async Task<BookingData?> GetBooking(Guid id)
    {
        var dao = await bookingRepository.GetBooking(id);
        return dao == null ? null : bookingDataMapper.FromDao(dao);
    }

    private async Task<bool> IsBookingMadeByUser(Guid bookingId)
    {
        var currentUser = Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var dao = await bookingRepository.GetBooking(bookingId);
        if (dao == null) return false;

        return dao.UserId == currentUser;
    }

    private async Task<Operation> ValidateBookingTimeslot(DateTime bookingData) =>
        await bookingRepository.IsConflicting(bookingData, _bookingSettings.SlotDuration)
            ? Operation.Forbidden("Booking is conflicting with other booking")
            : Operation.Success();
}

public interface IBookingService
{
    Task<List<BookingData>> GetMyBookings();
    Task<List<BookingData>> GetAllBookings();
    Task<OperationResult<BookingData>> Create(DateTime bookingTime);
    Task<Operation> Delete(Guid id);
}