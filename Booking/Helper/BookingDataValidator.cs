namespace Booking.Helper;

using FluentValidation;

public class DateTimeValidator : AbstractValidator<DateTime>
{
    public DateTimeValidator()
    {
        RuleFor(dateTime => dateTime.Minute).Equal(0).WithMessage("Minutes should be 0.");
        RuleFor(dateTime => dateTime.Second).Equal(0).WithMessage("Seconds should be 0.");
    }
}
