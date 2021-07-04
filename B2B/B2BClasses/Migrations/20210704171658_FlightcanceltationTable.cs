using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class FlightcanceltationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelationId",
                table: "tblFlightBookingSegmentMaster",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelationRemarks",
                table: "tblFlightBookingSegmentMaster",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tblFlightCancelation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SegmentDisplayOrder = table.Column<int>(type: "int", nullable: false),
                    src = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    flightNumbers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    airlines = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ln = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amendmentCharges = table.Column<double>(type: "float", nullable: false),
                    refundableamount = table.Column<double>(type: "float", nullable: false),
                    totalFare = table.Column<double>(type: "float", nullable: false),
                    bookingId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amendmentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TraceId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CancelDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CancelRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFlightCancelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFlightCancelation_tblFlightBookingMaster_TraceId",
                        column: x => x.TraceId,
                        principalTable: "tblFlightBookingMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblFlightCancelation_TraceId",
                table: "tblFlightCancelation",
                column: "TraceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblFlightCancelation");

            migrationBuilder.DropColumn(
                name: "CancelationId",
                table: "tblFlightBookingSegmentMaster");

            migrationBuilder.DropColumn(
                name: "CancelationRemarks",
                table: "tblFlightBookingSegmentMaster");
        }
    }
}
