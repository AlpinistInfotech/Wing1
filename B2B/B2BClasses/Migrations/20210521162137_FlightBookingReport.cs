using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class FlightBookingReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 12,
                column: "Role",
                value: 51);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 13,
                column: "Role",
                value: 101);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 14,
                column: "Role",
                value: 102);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 15,
                column: "Role",
                value: 103);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 16,
                column: "Role",
                value: 104);

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

            migrationBuilder.InsertData(
                table: "tblUserRole",
                columns: new[] { "Id", "CreatedBy", "CreatedDt", "IsDeleted", "ModifiedBy", "ModifiedDt", "Role", "UserId" },
                values: new object[] { 19, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 113, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 12,
                column: "Role",
                value: 101);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 13,
                column: "Role",
                value: 102);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 14,
                column: "Role",
                value: 103);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 15,
                column: "Role",
                value: 104);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 16,
                column: "Role",
                value: 111);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 17,
                column: "Role",
                value: 112);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 18,
                column: "Role",
                value: 113);
        }
    }
}
