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
        var numberOfBookingsValidation = await ValidateNumberOfBookings();
        if (numberOfBookingsValidation.Status != OperationStatus.Success)
            return Operation.Forbidden<BookingData>(numberOfBookingsValidation.Message);
        
        var timeslotValidation = await ValidateBookingTimeslot(bookingTime);
        if (timeslotValidation.Status != OperationStatus.Success)
            return Operation.Forbidden<BookingData>(timeslotValidation.Message);

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

    private async Task<Operation> ValidateBookingTimeslot(DateTime bookingTime) =>
        await bookingRepository.IsConflicting(bookingTime, _bookingSettings.SlotDuration)
            ? Operation.Forbidden("Booking is conflicting with other booking")
            : Operation.Success();

    private async Task<Operation> ValidateNumberOfBookings()
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var usersBookings = await bookingRepository.GetUsersBookings(Guid.Parse(userId!));

        //Verifies if users total amount of booking are topped out
        if (usersBookings.Count >= _bookingSettings.MaxAmountPerUser)
            return Operation.Forbidden($"You have maximum number of bookings: {_bookingSettings.MaxAmountPerUser}");

        //Verifies if users total amount of booking are topped out for a rolling week (from today)
        var timeOneWeekAhead = DateTime.Now + TimeSpan.FromDays(7);
        if(usersBookings.Count(x => x.BookingDateTime > DateTime.Now && x.BookingDateTime < timeOneWeekAhead) >= _bookingSettings.MaxAmountPerUserPerWeek)
            return Operation.Forbidden($"You have the maximum number of booking in a week: {_bookingSettings.MaxAmountPerUserPerWeek}");
        
        return Operation.Success();
    }
}

public interface IBookingService
{
    Task<List<BookingData>> GetMyBookings();
    Task<List<BookingData>> GetAllBookings();
    Task<OperationResult<BookingData>> Create(DateTime bookingTime);
    Task<Operation> Delete(Guid id);
}