using System.Security.Claims;
using Booking.DataAccess;
using Booking.DataAccess.Dao;
using Booking.Helper;
using Booking.Mappers;
using Booking.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Test;

[TestFixture]
public class BookingServiceTests
{
    private static BookingService SetupBookingService(
        Mock<IBookingRepository>? bookingRepositoryMock,
        Mock<IBookingDataMapper>? bookingDataMapperMock,
        Mock<IHttpContextAccessor>? httpContextAccessorMock,
        Mock<IOptions<BookingSettings>>? settingsMock
    )
    {
        bookingRepositoryMock ??= new Mock<IBookingRepository>();
        bookingDataMapperMock ??= new Mock<IBookingDataMapper>();
        httpContextAccessorMock ??= new Mock<IHttpContextAccessor>();
        settingsMock ??= new Mock<IOptions<BookingSettings>>();

        var bookingService = new BookingService(
            bookingRepositoryMock.Object,
            bookingDataMapperMock.Object,
            httpContextAccessorMock.Object,
            settingsMock.Object
        );

        return bookingService;
    }

    private static Mock<IOptions<BookingSettings>> SetupSettingsMock(int? maxAmountPerUser, int? maxAmountPerUserPerWeek)
    {
        var settings = new BookingSettings
        {
            MaxAmountPerUser = maxAmountPerUser!.Value,
            MaxAmountPerUserPerWeek = maxAmountPerUserPerWeek!.Value
        };
        var settingsMock = new Mock<IOptions<BookingSettings>>();
        settingsMock.Setup(c => c.Value).Returns(settings);
        return settingsMock;
    }

    private static Mock<IHttpContextAccessor> SetupHttpContextMockWithFakeUser()
    {
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                    }))
        };

        httpContextAccessorMock.Setup(c => c.HttpContext).Returns(httpContext);
        return httpContextAccessorMock;
    }

    private static Mock<IBookingRepository> SetupBookingRepositoryMock()
    {
        var bookingRepositoryMock = new Mock<IBookingRepository>();
        return bookingRepositoryMock;
    }

    private static BookingService SetupBookingService(int maxBookingsPerUser, int maxBookingsPerUserPerWeek, IReadOnlyCollection<BookingDataDao> bookingDataDaos)
    {
        var bookingRepositoryMock = SetupBookingRepositoryMock();
        bookingRepositoryMock.Setup(c => c.GetUsersBookings(It.IsAny<Guid>())).ReturnsAsync(bookingDataDaos);
        var settingsMock = SetupSettingsMock(maxBookingsPerUser, maxBookingsPerUserPerWeek);
        var httpContextMock = SetupHttpContextMockWithFakeUser();
        var bookingService = SetupBookingService(bookingRepositoryMock, null, httpContextMock, settingsMock);
        return bookingService;
    }

    private static BookingDataDao BookingDataDao(DateTime bookingDateTime) => new() { BookingDateTime = bookingDateTime };

    private static List<BookingDataDao> SetupUpNBookingDataDaos(int numberOfBookings) => Enumerable.Range(0, numberOfBookings).Select(x => new BookingDataDao()).ToList();

    [Test]
    public async Task ValidateNumberOfBookings_UserBelowMaxAmount_ReturnsSuccess()
    {
        // Arrange
        var bookings = new List<BookingDataDao>
        {
            BookingDataDao(DateTime.Now),
            BookingDataDao(DateTime.Now + TimeSpan.FromDays(3)),
        };

        var bookingService = SetupBookingService(3, 5, bookings);

        // Act
        var result = await bookingService.ValidateNumberOfBookings();

        // // Assert
        Assert.True(result.IsSuccessful);
    }

    [Test]
    public async Task ValidateNumberOfBookings_UserAtMaxAmount_ReturnsForbidden()
    {
        // Arrange
        var bookings = SetupUpNBookingDataDaos(3);
        var bookingService = SetupBookingService(3, 3, bookings);

        // Act
        var result = await bookingService.ValidateNumberOfBookings();

        // Assert
        Assert.That(result.Error.Details, Is.EqualTo("You have maximum number of bookings: 3"));
    }

    [Test]
    public async Task ValidateNumberOfBookings_UserBelowMaxAmountPerWeek_ReturnsSuccess()
    {
        // Arrange
        var bookings = SetupUpNBookingDataDaos(1);
        var bookingService = SetupBookingService(5, 3, bookings);

        // Act
        var result = await bookingService.ValidateNumberOfBookings();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Test]
    public async Task ValidateNumberOfBookings_UserAtMaxAmountPerWeek_ReturnsForbidden()
    {
        // Arrange
        var bookings = new List<BookingDataDao>
        {
            BookingDataDao(DateTime.Now),
            BookingDataDao(DateTime.Now + TimeSpan.FromDays(1)),
            BookingDataDao(DateTime.Now + TimeSpan.FromDays(2)),
        };

        var bookingService = SetupBookingService(5, 3, bookings);

        // Act
        var result = await bookingService.ValidateNumberOfBookings();

        // Assert
        Assert.That(result.Error.Details, Is.EqualTo("You have the maximum number of bookings in a week: 3"));
    }
}