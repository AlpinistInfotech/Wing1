using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class BookingDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookingId",
                table: "tblFlightBookingProviderTraceId",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookingMessage",
                table: "tblFlightBookingProviderTraceId",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookingStatus",
                table: "tblFlightBookingProviderTraceId",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "tblFlightBookingProviderTraceId");

            migrationBuilder.DropColumn(
                name: "BookingMessage",
                table: "tblFlightBookingProviderTraceId");

            migrationBuilder.DropColumn(
                name: "BookingStatus",
                table: "tblFlightBookingProviderTraceId");
        }
    }
}
