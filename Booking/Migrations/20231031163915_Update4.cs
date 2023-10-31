using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class Update4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Bookings",
                newName: "RequestTimestamp");

            migrationBuilder.AddColumn<DateTime>(
                name: "BookingDateTime",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingDateTime",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "RequestTimestamp",
                table: "Bookings",
                newName: "Time");
        }
    }
}
