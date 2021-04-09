using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class Airport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblAirline",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isLcc = table.Column<bool>(type: "bit", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAirline", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblAirport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDomestic = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AirportCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirportName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Terminal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAirport", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 6,
                column: "Role",
                value: 20);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 7,
                column: "Role",
                value: 21);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 8,
                column: "Role",
                value: 22);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 9,
                column: "Role",
                value: 23);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 10,
                column: "Role",
                value: 24);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 11,
                column: "Role",
                value: 25);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 12,
                column: "Role",
                value: 101);

            migrationBuilder.InsertData(
                table: "tblUserRole",
                columns: new[] { "Id", "CreatedBy", "CreatedDt", "IsDeleted", "ModifiedBy", "ModifiedDt", "Role", "UserId" },
                values: new object[,]
                {
                    { 13, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 102, 1 },
                    { 14, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 103, 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblAirline");

            migrationBuilder.DropTable(
                name: "tblAirport");

            migrationBuilder.DeleteData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 6,
                column: "Role",
                value: 21);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 7,
                column: "Role",
                value: 22);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 8,
                column: "Role",
                value: 23);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 9,
                column: "Role",
                value: 24);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 10,
                column: "Role",
                value: 25);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 11,
                column: "Role",
                value: 101);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 12,
                column: "Role",
                value: 102);
        }
    }
}
