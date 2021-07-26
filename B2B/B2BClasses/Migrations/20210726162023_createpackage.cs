using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class createpackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblBookingNumberMaster",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    BookingNumber = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    YearMonth = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    TotalDigit = table.Column<byte>(type: "tinyint", nullable: false),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBookingNumberMaster", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "tblPackageMaster",
                columns: table => new
                {
                    PackageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDomestic = table.Column<bool>(type: "bit", nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LongDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ThumbnailImage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AllImage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NumberOfDay = table.Column<int>(type: "int", nullable: false),
                    NumberOfNight = table.Column<int>(type: "int", nullable: false),
                    EffectiveFromDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveToDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdultPrice = table.Column<double>(type: "float", nullable: false),
                    ChildPrice = table.Column<double>(type: "float", nullable: false),
                    InfantPrice = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPackageMaster", x => x.PackageId);
                });

            migrationBuilder.CreateTable(
                name: "tblPackageBooking",
                columns: table => new
                {
                    BookingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    AdultPrice = table.Column<double>(type: "float", nullable: false),
                    ChildPrice = table.Column<double>(type: "float", nullable: false),
                    InfantPrice = table.Column<double>(type: "float", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    NetPrice = table.Column<double>(type: "float", nullable: false),
                    BookingStatus = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPackageBooking", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_tblPackageBooking_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblPackageBooking_tblPackageMaster_PackageId",
                        column: x => x.PackageId,
                        principalTable: "tblPackageMaster",
                        principalColumn: "PackageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblPackageBookingDiscussionDetails",
                columns: table => new
                {
                    Discussionid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPackageBookingDiscussionDetails", x => x.Discussionid);
                    table.ForeignKey(
                        name: "FK_tblPackageBookingDiscussionDetails_tblPackageBooking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "tblPackageBooking",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblPackageBookingPassengerDetails",
                columns: table => new
                {
                    Pid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    passengerType = table.Column<int>(type: "int", nullable: false),
                    dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    pNum = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PassportExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PassportIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPackageBookingPassengerDetails", x => x.Pid);
                    table.ForeignKey(
                        name: "FK_tblPackageBookingPassengerDetails_tblPackageBooking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "tblPackageBooking",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 20,
                column: "ClaimId",
                value: 108);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 21,
                column: "ClaimId",
                value: 111);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 22,
                column: "ClaimId",
                value: 112);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 23,
                column: "ClaimId",
                value: 113);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 24,
                column: "ClaimId",
                value: 114);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 25,
                column: "ClaimId",
                value: 10001);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 26,
                column: "ClaimId",
                value: 10002);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 27,
                column: "ClaimId",
                value: 10003);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 28,
                column: "ClaimId",
                value: 10004);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 29,
                column: "ClaimId",
                value: 10005);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 30,
                column: "ClaimId",
                value: 10006);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 31,
                column: "ClaimId",
                value: 10007);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 32,
                column: "ClaimId",
                value: 10008);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 33,
                column: "ClaimId",
                value: 10009);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 34,
                column: "ClaimId",
                value: 10010);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 35,
                column: "ClaimId",
                value: 10011);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 36,
                column: "ClaimId",
                value: 10012);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 37,
                column: "ClaimId",
                value: 10013);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 38,
                column: "ClaimId",
                value: 10100);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 39,
                column: "ClaimId",
                value: 10101);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 40,
                column: "ClaimId",
                value: 10102);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 113,
                column: "ClaimId",
                value: 107);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 114,
                column: "ClaimId",
                value: 10001);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 115,
                column: "ClaimId",
                value: 10002);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 116,
                column: "ClaimId",
                value: 10003);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 117,
                column: "ClaimId",
                value: 10004);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 118,
                column: "ClaimId",
                value: 10005);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 119,
                column: "ClaimId",
                value: 10006);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 120,
                column: "ClaimId",
                value: 10007);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 121,
                column: "ClaimId",
                value: 10008);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 122,
                column: "ClaimId",
                value: 10009);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 123,
                column: "ClaimId",
                value: 10010);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 124,
                column: "ClaimId",
                value: 10011);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 125,
                column: "ClaimId",
                value: 10012);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 126,
                column: "ClaimId",
                value: 10013);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 127,
                column: "ClaimId",
                value: 10100);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 128,
                column: "ClaimId",
                value: 10101);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 129,
                column: "ClaimId",
                value: 10102);

            migrationBuilder.InsertData(
                table: "tblRoleClaim",
                columns: new[] { "Id", "ClaimId", "CreatedBy", "CreatedDt", "IsDeleted", "ModifiedBy", "ModifiedDt", "Role" },
                values: new object[,]
                {
                    { 42, 10150, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 130, 10103, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 41, 10103, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblPackageBooking_CustomerId",
                table: "tblPackageBooking",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPackageBooking_PackageId",
                table: "tblPackageBooking",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPackageBookingDiscussionDetails_BookingId",
                table: "tblPackageBookingDiscussionDetails",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPackageBookingPassengerDetails_BookingId",
                table: "tblPackageBookingPassengerDetails",
                column: "BookingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblBookingNumberMaster");

            migrationBuilder.DropTable(
                name: "tblPackageBookingDiscussionDetails");

            migrationBuilder.DropTable(
                name: "tblPackageBookingPassengerDetails");

            migrationBuilder.DropTable(
                name: "tblPackageBooking");

            migrationBuilder.DropTable(
                name: "tblPackageMaster");

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 20,
                column: "ClaimId",
                value: 111);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 21,
                column: "ClaimId",
                value: 112);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 22,
                column: "ClaimId",
                value: 113);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 23,
                column: "ClaimId",
                value: 114);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 24,
                column: "ClaimId",
                value: 10001);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 25,
                column: "ClaimId",
                value: 10002);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 26,
                column: "ClaimId",
                value: 10003);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 27,
                column: "ClaimId",
                value: 10004);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 28,
                column: "ClaimId",
                value: 10005);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 29,
                column: "ClaimId",
                value: 10006);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 30,
                column: "ClaimId",
                value: 10007);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 31,
                column: "ClaimId",
                value: 10008);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 32,
                column: "ClaimId",
                value: 10009);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 33,
                column: "ClaimId",
                value: 10010);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 34,
                column: "ClaimId",
                value: 10011);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 35,
                column: "ClaimId",
                value: 10012);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 36,
                column: "ClaimId",
                value: 10013);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 37,
                column: "ClaimId",
                value: 10100);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 38,
                column: "ClaimId",
                value: 10101);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 39,
                column: "ClaimId",
                value: 10102);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 40,
                column: "ClaimId",
                value: 10103);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 113,
                column: "ClaimId",
                value: 10001);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 114,
                column: "ClaimId",
                value: 10002);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 115,
                column: "ClaimId",
                value: 10003);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 116,
                column: "ClaimId",
                value: 10004);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 117,
                column: "ClaimId",
                value: 10005);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 118,
                column: "ClaimId",
                value: 10006);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 119,
                column: "ClaimId",
                value: 10007);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 120,
                column: "ClaimId",
                value: 10008);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 121,
                column: "ClaimId",
                value: 10009);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 122,
                column: "ClaimId",
                value: 10010);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 123,
                column: "ClaimId",
                value: 10011);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 124,
                column: "ClaimId",
                value: 10012);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 125,
                column: "ClaimId",
                value: 10013);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 126,
                column: "ClaimId",
                value: 10100);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 127,
                column: "ClaimId",
                value: 10101);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 128,
                column: "ClaimId",
                value: 10102);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 129,
                column: "ClaimId",
                value: 10103);
        }
    }
}
