using FluentValidation;
using Microsoft.Extensions.Options;

namespace Booking.Helper;

public class DateTimeValidator : AbstractValidator<DateTime>
{
    public DateTimeValidator(IOptions<BookingSettings> bookingSettings)
    {
        var settings = bookingSettings.Value;

        RuleFor(dateTime => dateTime.Minute).Equal(0).WithMessage("Minutes should be 0.");
        RuleFor(dateTime => dateTime.Second).Equal(0).WithMessage("Seconds should be 0.");
        RuleFor(dateTime => dateTime.Hour).LessThanOrEqualTo(settings.ClosingTime - 3).WithMessage("You're making a booking after closing time");
        RuleFor(dateTime => dateTime.Hour).GreaterThanOrEqualTo(settings.OpeningTime).WithMessage("You're making a booking before opening time");
    }
}