using Booking.DataAccess;
using Booking.DataAccess.Dao;
using Booking.Models;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Test;

[TestFixture]
public class BookingRepositoryTests
{
    private static TimeSpan Hours(int hours) => new(0, hours, 0, 0, 0);
    private static TimeSpan ThreeHours => new(0, 3, 0, 0, 0);
    private static DateTime TodayAtTwelve => new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0);

    [Test]
    public async Task IsConflicting_Conflicting_ReturnsTrue()
    {
        // Arrange
        var bookingDataDaos = new List<BookingDataDao>
        {
            new() { BookingDateTime = TodayAtTwelve },
        };

        var mock = bookingDataDaos.BuildMock().BuildMockDbSet();
        var bookingContextMock = new Mock<AppDbContext>();
        bookingContextMock.Setup(x => x.Bookings).Returns(mock.Object);

        //Act
        var bookingRepository = new BookingRepository(bookingContextMock.Object);
        var result = await bookingRepository.IsConflicting(TodayAtTwelve + Hours(1), ThreeHours);

        //Assert
        Assert.True(result);
    }

    [Test]
    public async Task IsConflicting_NotConflicting_ReturnsFalse()
    {
        // Arrange
        var bookingDataDaos = new List<BookingDataDao>
        {
            new() { BookingDateTime = TodayAtTwelve },
        };

        var mock = bookingDataDaos.BuildMock().BuildMockDbSet();
        var bookingContextMock = new Mock<AppDbContext>();
        bookingContextMock.Setup(x => x.Bookings).Returns(mock.Object);

        //Act
        var bookingRepository = new BookingRepository(bookingContextMock.Object);
        var result = await bookingRepository.IsConflicting(TodayAtTwelve + Hours(3), ThreeHours);

        //Assert
        Assert.False(result);
    }
}