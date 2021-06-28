using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class DiscountColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "tblTicket",
                newName: "TicketMasterId");

            migrationBuilder.AddColumn<int>(
                name: "ActualTime",
                table: "tblTicket",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "tblFlightBookingFareDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 19,
                column: "ClaimId",
                value: 107);

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

            migrationBuilder.InsertData(
                table: "tblRoleClaim",
                columns: new[] { "Id", "ClaimId", "CreatedBy", "CreatedDt", "IsDeleted", "ModifiedBy", "ModifiedDt", "Role" },
                values: new object[,]
                {
                    { 126, 10100, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 38, 10101, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 37, 10100, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 127, 10101, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 36, 10013, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DropColumn(
                name: "ActualTime",
                table: "tblTicket");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "tblFlightBookingFareDetails");

            migrationBuilder.RenameColumn(
                name: "TicketMasterId",
                table: "tblTicket",
                newName: "TicketId");

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 19,
                column: "ClaimId",
                value: 111);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 20,
                column: "ClaimId",
                value: 112);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 21,
                column: "ClaimId",
                value: 113);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 22,
                column: "ClaimId",
                value: 114);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 23,
                column: "ClaimId",
                value: 10001);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 24,
                column: "ClaimId",
                value: 10002);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 25,
                column: "ClaimId",
                value: 10003);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 26,
                column: "ClaimId",
                value: 10004);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 27,
                column: "ClaimId",
                value: 10005);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 28,
                column: "ClaimId",
                value: 10006);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 29,
                column: "ClaimId",
                value: 10007);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 30,
                column: "ClaimId",
                value: 10008);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 31,
                column: "ClaimId",
                value: 10009);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 32,
                column: "ClaimId",
                value: 10010);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 33,
                column: "ClaimId",
                value: 10011);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 34,
                column: "ClaimId",
                value: 10012);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 35,
                column: "ClaimId",
                value: 10013);
        }
    }
}
