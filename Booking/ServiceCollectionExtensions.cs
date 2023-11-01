using Booking.DataAccess;
using Booking.Mappers;
using Booking.Services;

namespace Booking;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Register(this IServiceCollection services)
    {
        services.AddTransient<IBookingService, BookingService>();

        services.AddTransient<IBookingRepository, BookingRepository>();

        services.AddTransient<IBookingDataMapper, BookingDataMapper>();


        return services;
    }
}