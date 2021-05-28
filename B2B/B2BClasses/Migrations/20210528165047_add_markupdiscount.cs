using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class add_markupdiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblWingDiscount",
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
                    table.PrimaryKey("PK_tblWingDiscount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblWingDiscountAirline",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    AirlineId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingDiscountAirline", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingDiscountAirline_tblAirline_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "tblAirline",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblWingDiscountAirline_tblWingDiscount_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingDiscount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingDiscountCustomerDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingDiscountCustomerDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingDiscountCustomerDetails_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblWingDiscountCustomerDetails_tblWingDiscount_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingDiscount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingDiscountCustomerType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    customerType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingDiscountCustomerType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingDiscountCustomerType_tblWingDiscount_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingDiscount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingDiscountFlightClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    CabinClass = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingDiscountFlightClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingDiscountFlightClass_tblWingDiscount_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingDiscount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingDiscountPassengerType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    PassengerType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingDiscountPassengerType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingDiscountPassengerType_tblWingDiscount_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingDiscount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingDiscountServiceProvider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    ServiceProvider = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingDiscountServiceProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingDiscountServiceProvider_tblWingDiscount_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingDiscount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 17,
                column: "Role",
                value: 105);

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

            migrationBuilder.InsertData(
                table: "tblUserRole",
                columns: new[] { "Id", "CreatedBy", "CreatedDt", "IsDeleted", "ModifiedBy", "ModifiedDt", "Role", "UserId" },
                values: new object[] { 20, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 113, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_tblWingDiscountAirline_AirlineId",
                table: "tblWingDiscountAirline",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingDiscountAirline_MarkupId",
                table: "tblWingDiscountAirline",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingDiscountCustomerDetails_CustomerId",
                table: "tblWingDiscountCustomerDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingDiscountCustomerDetails_MarkupId",
                table: "tblWingDiscountCustomerDetails",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingDiscountCustomerType_MarkupId",
                table: "tblWingDiscountCustomerType",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingDiscountFlightClass_MarkupId",
                table: "tblWingDiscountFlightClass",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingDiscountPassengerType_MarkupId",
                table: "tblWingDiscountPassengerType",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingDiscountServiceProvider_MarkupId",
                table: "tblWingDiscountServiceProvider",
                column: "MarkupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblWingDiscountAirline");

            migrationBuilder.DropTable(
                name: "tblWingDiscountCustomerDetails");

            migrationBuilder.DropTable(
                name: "tblWingDiscountCustomerType");

            migrationBuilder.DropTable(
                name: "tblWingDiscountFlightClass");

            migrationBuilder.DropTable(
                name: "tblWingDiscountPassengerType");

            migrationBuilder.DropTable(
                name: "tblWingDiscountServiceProvider");

            migrationBuilder.DropTable(
                name: "tblWingDiscount");

            migrationBuilder.DeleteData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 17,
                column: "Role",
                value: 111);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 18,
                column: "Role",
                value: 112);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 19,
                column: "Role",
                value: 113);
        }
    }
}
