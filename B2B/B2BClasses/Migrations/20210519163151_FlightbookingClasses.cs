using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class FlightbookingClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblFlightBookingMaster",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AdultCount = table.Column<int>(type: "int", nullable: false),
                    ChildCount = table.Column<int>(type: "int", nullable: false),
                    InfantCount = table.Column<int>(type: "int", nullable: false),
                    DirectFlight = table.Column<bool>(type: "bit", nullable: false),
                    JourneyType = table.Column<int>(type: "int", nullable: false),
                    BookingStatus = table.Column<int>(type: "int", nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFlightBookingMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblFlightBookingFareDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SegmentDisplayOrder = table.Column<int>(type: "int", nullable: false),
                    TraceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WingAdultMarkup = table.Column<double>(type: "float", nullable: false),
                    WingChildMarkup = table.Column<double>(type: "float", nullable: false),
                    WingInfantMarkup = table.Column<double>(type: "float", nullable: false),
                    WingTotalMarkup = table.Column<double>(type: "float", nullable: false),
                    CustomerMarkup = table.Column<double>(type: "float", nullable: false),
                    TotalFare = table.Column<double>(type: "float", nullable: false),
                    convenience = table.Column<double>(type: "float", nullable: false),
                    NetFare = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFlightBookingFareDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFlightBookingFareDetails_tblFlightBookingMaster_TraceId",
                        column: x => x.TraceId,
                        principalTable: "tblFlightBookingMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblFlightBookingFarePurchaseDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SegmentDisplayOrder = table.Column<int>(type: "int", nullable: false),
                    TraceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Provider = table.Column<int>(type: "int", nullable: false),
                    AdultBaseFare = table.Column<double>(type: "float", nullable: false),
                    ChildBaseFare = table.Column<double>(type: "float", nullable: false),
                    InfantBaseFare = table.Column<double>(type: "float", nullable: false),
                    AdultTotalFare = table.Column<double>(type: "float", nullable: false),
                    ChildTotalFare = table.Column<double>(type: "float", nullable: false),
                    InfantTotalFare = table.Column<double>(type: "float", nullable: false),
                    AdultNetFare = table.Column<double>(type: "float", nullable: false),
                    ChildNetFare = table.Column<double>(type: "float", nullable: false),
                    InfantNetFare = table.Column<double>(type: "float", nullable: false),
                    TotalFare = table.Column<double>(type: "float", nullable: false),
                    NetFare = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFlightBookingFarePurchaseDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFlightBookingFarePurchaseDetails_tblFlightBookingMaster_TraceId",
                        column: x => x.TraceId,
                        principalTable: "tblFlightBookingMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblFlightBookingGSTDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    gstNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    registeredName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TraceId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFlightBookingGSTDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFlightBookingGSTDetails_tblFlightBookingMaster_TraceId",
                        column: x => x.TraceId,
                        principalTable: "tblFlightBookingMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblFlightBookingPassengerDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    passengerType = table.Column<int>(type: "int", nullable: false),
                    dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    pNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassportExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PassportIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TraceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BookingId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceProvider = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFlightBookingPassengerDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFlightBookingPassengerDetails_tblFlightBookingMaster_TraceId",
                        column: x => x.TraceId,
                        principalTable: "tblFlightBookingMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblFlightBookingProviderTraceId",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SegmentDisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ServiceProvider = table.Column<int>(type: "int", nullable: false),
                    ProviderTraceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TraceId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFlightBookingProviderTraceId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFlightBookingProviderTraceId_tblFlightBookingMaster_TraceId",
                        column: x => x.TraceId,
                        principalTable: "tblFlightBookingMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblFlightBookingSegment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TripIndicator = table.Column<int>(type: "int", nullable: false),
                    SegmentDisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CabinClass = table.Column<int>(type: "int", nullable: false),
                    ClassOfBooking = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TravelDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Airline = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirlineCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceProvider = table.Column<int>(type: "int", nullable: false),
                    ProviderResultIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TraceId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFlightBookingSegment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFlightBookingSegment_tblFlightBookingMaster_TraceId",
                        column: x => x.TraceId,
                        principalTable: "tblFlightBookingMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblFlightBookingServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    SegmentDisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    PassengerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFlightBookingServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFlightBookingServices_tblFlightBookingPassengerDetails_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "tblFlightBookingPassengerDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblFlightBookingFareDetails_TraceId",
                table: "tblFlightBookingFareDetails",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFlightBookingFarePurchaseDetails_TraceId",
                table: "tblFlightBookingFarePurchaseDetails",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFlightBookingGSTDetails_TraceId",
                table: "tblFlightBookingGSTDetails",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFlightBookingPassengerDetails_TraceId",
                table: "tblFlightBookingPassengerDetails",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFlightBookingProviderTraceId_TraceId",
                table: "tblFlightBookingProviderTraceId",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFlightBookingSegment_TraceId",
                table: "tblFlightBookingSegment",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFlightBookingServices_PassengerId",
                table: "tblFlightBookingServices",
                column: "PassengerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblFlightBookingFareDetails");

            migrationBuilder.DropTable(
                name: "tblFlightBookingFarePurchaseDetails");

            migrationBuilder.DropTable(
                name: "tblFlightBookingGSTDetails");

            migrationBuilder.DropTable(
                name: "tblFlightBookingProviderTraceId");

            migrationBuilder.DropTable(
                name: "tblFlightBookingSegment");

            migrationBuilder.DropTable(
                name: "tblFlightBookingServices");

            migrationBuilder.DropTable(
                name: "tblFlightBookingPassengerDetails");

            migrationBuilder.DropTable(
                name: "tblFlightBookingMaster");
        }
    }
}
