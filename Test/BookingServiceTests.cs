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

    private static BookingService SetupBookingService(int maxBookingsPerUser, int maxBookingsPerUserPerWeek, int numberOfBookings)
    {
        var bookingRepositoryMock = SetupBookingRepositoryMock();
        bookingRepositoryMock.Setup(c => c.GetUsersBookings(It.IsAny<Guid>())).ReturnsAsync(CreateBookingDataDaos(numberOfBookings));
        var settingsMock = SetupSettingsMock(maxBookingsPerUser, maxBookingsPerUserPerWeek);
        var httpContextMock = SetupHttpContextMockWithFakeUser();
        var bookingService = SetupBookingService(bookingRepositoryMock, null, httpContextMock, settingsMock);
        return bookingService;
    }

    private static List<BookingDataDao> CreateBookingDataDaos(int numberOfBookings)
    {
        var listOfBookings = new List<BookingDataDao>();

        for (var i = 0; i < numberOfBookings; i++)
            listOfBookings.Add(new BookingDataDao
            {
                BookingDateTime = DateTime.Now + TimeSpan.FromHours(i * 3)
            });

        return listOfBookings;
    }

    [Test]
    public async Task ValidateNumberOfBookings_UserBelowMaxAmount_ReturnsSuccess()
    {
        // Arrange
        var bookingService = SetupBookingService(3, 5, 1);

        // Act
        var result = await bookingService.ValidateNumberOfBookings();

        // // Assert
        Assert.True(result.IsSuccessful);
    }

    [Test]
    public async Task ValidateNumberOfBookings_UserAtMaxAmount_ReturnsForbidden()
    {
        // Arrange
        var bookingService = SetupBookingService(3, 3, 3);

        // Act
        var result = await bookingService.ValidateNumberOfBookings();

        // Assert
        Assert.That(result.Error.Details, Is.EqualTo("You have maximum number of bookings: 3"));
    }

    [Test]
    public async Task ValidateNumberOfBookings_UserBelowMaxAmountPerWeek_ReturnsSuccess()
    {
        // Arrange
        var bookingService = SetupBookingService(5, 3, 2);

        // Act
        var result = await bookingService.ValidateNumberOfBookings();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Test]
    public async Task ValidateNumberOfBookings_UserAtMaxAmountPerWeek_ReturnsForbidden()
    {
        // Arrange
        var bookingService = SetupBookingService(5, 3, 4);

        // Act
        var result = await bookingService.ValidateNumberOfBookings();

        // Assert
        Assert.That(result.Error.Details, Is.EqualTo("You have the maximum number of booking in a week: 3"));
    }
}