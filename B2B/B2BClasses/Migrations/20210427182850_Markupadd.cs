using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class Markupadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsApplicableOnEachPasenger",
                table: "tblWingMarkupMaster",
                newName: "IsAllPessengerType");

            migrationBuilder.RenameColumn(
                name: "IsAllPessenger",
                table: "tblWingMarkupMaster",
                newName: "IsAllCustomerType");

            migrationBuilder.AddColumn<int>(
                name: "Applicability",
                table: "tblWingMarkupMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllAirline",
                table: "tblWingMarkupMaster",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "tblWingMarkupAirline",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    AirlineId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingMarkupAirline", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingMarkupAirline_tblAirline_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "tblAirline",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblWingMarkupAirline_tblWingMarkupMaster_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingMarkupMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingMarkupCustomerType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    customerType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingMarkupCustomerType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingMarkupCustomerType_tblWingMarkupMaster_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingMarkupMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingMarkupFlightClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    classid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingMarkupFlightClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingMarkupFlightClass_tblWingMarkupMaster_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingMarkupMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "tblUserRole",
                columns: new[] { "Id", "CreatedBy", "CreatedDt", "IsDeleted", "ModifiedBy", "ModifiedDt", "Role", "UserId" },
                values: new object[] { 15, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 111, 1 });

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomerMaster_WalletBalence",
                table: "tblCustomerMaster",
                sql: "WalletBalence >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingMarkupAirline_AirlineId",
                table: "tblWingMarkupAirline",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingMarkupAirline_MarkupId",
                table: "tblWingMarkupAirline",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingMarkupCustomerType_MarkupId",
                table: "tblWingMarkupCustomerType",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingMarkupFlightClass_MarkupId",
                table: "tblWingMarkupFlightClass",
                column: "MarkupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblWingMarkupAirline");

            migrationBuilder.DropTable(
                name: "tblWingMarkupCustomerType");

            migrationBuilder.DropTable(
                name: "tblWingMarkupFlightClass");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomerMaster_WalletBalence",
                table: "tblCustomerMaster");

            migrationBuilder.DeleteData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DropColumn(
                name: "Applicability",
                table: "tblWingMarkupMaster");

            migrationBuilder.DropColumn(
                name: "IsAllAirline",
                table: "tblWingMarkupMaster");

            migrationBuilder.RenameColumn(
                name: "IsAllPessengerType",
                table: "tblWingMarkupMaster",
                newName: "IsApplicableOnEachPasenger");

            migrationBuilder.RenameColumn(
                name: "IsAllCustomerType",
                table: "tblWingMarkupMaster",
                newName: "IsAllPessenger");
        }
    }
}
