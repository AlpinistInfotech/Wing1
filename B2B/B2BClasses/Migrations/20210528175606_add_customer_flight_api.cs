using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class add_customer_flight_api : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblWingCustomerFlightAPI",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Applicability = table.Column<int>(type: "int", nullable: false),
                    DirectFlight = table.Column<int>(type: "int", nullable: false),
                    IsAllProvider = table.Column<bool>(type: "bit", nullable: false),
                    IsAllCustomerType = table.Column<bool>(type: "bit", nullable: false),
                    IsAllCustomer = table.Column<bool>(type: "bit", nullable: false),
                    IsAllPessengerType = table.Column<bool>(type: "bit", nullable: false),
                    IsAllFlightClass = table.Column<bool>(type: "bit", nullable: false),
                    IsAllAirline = table.Column<bool>(type: "bit", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    DayCount = table.Column<int>(type: "int", nullable: false),
                    EffectiveFromDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveToDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingCustomerFlightAPI", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblWingCustomerFlightAPIAirline",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    AirlineId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingCustomerFlightAPIAirline", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingCustomerFlightAPIAirline_tblAirline_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "tblAirline",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblWingCustomerFlightAPIAirline_tblWingCustomerFlightAPI_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingCustomerFlightAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingCustomerFlightAPICustomerDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingCustomerFlightAPICustomerDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingCustomerFlightAPICustomerDetails_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblWingCustomerFlightAPICustomerDetails_tblWingCustomerFlightAPI_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingCustomerFlightAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingCustomerFlightAPICustomerType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    customerType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingCustomerFlightAPICustomerType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingCustomerFlightAPICustomerType_tblWingCustomerFlightAPI_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingCustomerFlightAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingCustomerFlightAPIFlightClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    CabinClass = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingCustomerFlightAPIFlightClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingCustomerFlightAPIFlightClass_tblWingCustomerFlightAPI_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingCustomerFlightAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingCustomerFlightAPIServiceProvider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    ServiceProvider = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingCustomerFlightAPIServiceProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingCustomerFlightAPIServiceProvider_tblWingCustomerFlightAPI_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingCustomerFlightAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 18,
                column: "Role",
                value: 106);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 19,
                column: "Role",
                value: 111);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 20,
                column: "Role",
                value: 112);

            migrationBuilder.InsertData(
                table: "tblUserRole",
                columns: new[] { "Id", "CreatedBy", "CreatedDt", "IsDeleted", "ModifiedBy", "ModifiedDt", "Role", "UserId" },
                values: new object[] { 21, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 113, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_tblWingCustomerFlightAPIAirline_AirlineId",
                table: "tblWingCustomerFlightAPIAirline",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingCustomerFlightAPIAirline_MarkupId",
                table: "tblWingCustomerFlightAPIAirline",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingCustomerFlightAPICustomerDetails_CustomerId",
                table: "tblWingCustomerFlightAPICustomerDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingCustomerFlightAPICustomerDetails_MarkupId",
                table: "tblWingCustomerFlightAPICustomerDetails",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingCustomerFlightAPICustomerType_MarkupId",
                table: "tblWingCustomerFlightAPICustomerType",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingCustomerFlightAPIFlightClass_MarkupId",
                table: "tblWingCustomerFlightAPIFlightClass",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingCustomerFlightAPIServiceProvider_MarkupId",
                table: "tblWingCustomerFlightAPIServiceProvider",
                column: "MarkupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblWingCustomerFlightAPIAirline");

            migrationBuilder.DropTable(
                name: "tblWingCustomerFlightAPICustomerDetails");

            migrationBuilder.DropTable(
                name: "tblWingCustomerFlightAPICustomerType");

            migrationBuilder.DropTable(
                name: "tblWingCustomerFlightAPIFlightClass");

            migrationBuilder.DropTable(
                name: "tblWingCustomerFlightAPIServiceProvider");

            migrationBuilder.DropTable(
                name: "tblWingCustomerFlightAPI");

            migrationBuilder.DeleteData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 18,
                column: "Role",
                value: 111);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 19,
                column: "Role",
                value: 112);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 20,
                column: "Role",
                value: 113);
        }
    }
}
