using System.Security.Claims;
using Booking.DataAccess;
using Booking.Helper;
using Booking.Mappers;
using Booking.Models;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Booking.Services;

public class BookingService(
        IBookingRepository bookingRepository,
        IBookingDataMapper bookingDataMapper,
        IHttpContextAccessor httpContextAccessor,
        IValidator<DateTime> _validator,
        IOptions<BookingSettings> bookingSettings
    )
    : IBookingService
{
    private readonly BookingSettings _bookingSettings = bookingSettings.Value;

    public async Task<List<BookingData>> GetMyBookings()
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var usersBookings = await bookingRepository.GetUsersBookings(Guid.Parse(userId!));
        return usersBookings.Select(bookingDataMapper.FromDao).ToList();
    }

    public async Task<OperationResult<BookingData>> Create(DateTime bookingTime)
    {
        //FluentValidation
        var validate = _validator.Validate(bookingTime);
        if (!validate.IsValid)
            return Operation.Forbidden<BookingData>(validate.Errors.First().ErrorMessage);

        //Verifies number of bookings made by user
        var numberOfBookingsValidation = await ValidateNumberOfBookings();
        if (numberOfBookingsValidation.Status != OperationStatus.Success)
            return Operation.Forbidden<BookingData>(numberOfBookingsValidation.Message);

        //Verifies if booking is conflicting with other bookings
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
        var allBookingsDaos = await bookingRepository.GetAllBookings();
        return allBookingsDaos.Select(bookingDataMapper.FromDao).ToList();
    }

    public async Task<BookingData?> GetBooking(Guid id)
    {
        var bookingDao = await bookingRepository.GetBooking(id);
        return bookingDao == null ? null : bookingDataMapper.FromDao(bookingDao);
    }

    private async Task<bool> IsBookingMadeByUser(Guid bookingId)
    {
        var currentUser = Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var bookingDao = await bookingRepository.GetBooking(bookingId);
        if (bookingDao == null)
            return false;

        return bookingDao.UserId == currentUser;
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