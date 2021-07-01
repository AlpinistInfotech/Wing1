using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class tblSegments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "tblFlightBookingPassengerDetails");

            migrationBuilder.DropColumn(
                name: "ServiceProvider",
                table: "tblFlightBookingPassengerDetails");

            migrationBuilder.CreateTable(
                name: "tblFlightBookingSegmentMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SegmentDisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TravelDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TraceId = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    BookingId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BookingStatus = table.Column<int>(type: "int", nullable: false),
                    BookingMessage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ServiceProvider = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFlightBookingSegmentMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFlightBookingSegmentMaster_tblFlightBookingMaster_TraceId",
                        column: x => x.TraceId,
                        principalTable: "tblFlightBookingMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblFlightBookingSegmentMaster_TraceId",
                table: "tblFlightBookingSegmentMaster",
                column: "TraceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblFlightBookingSegmentMaster");

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

            migrationBuilder.AddColumn<string>(
                name: "BookingId",
                table: "tblFlightBookingPassengerDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProvider",
                table: "tblFlightBookingPassengerDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
