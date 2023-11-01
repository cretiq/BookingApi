using Booking.DataAccess.Dao;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Booking.Models;

public class AppDbContext : IdentityDbContext<MyUser>
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<BookingDataDao> Bookings { get; set; }
}