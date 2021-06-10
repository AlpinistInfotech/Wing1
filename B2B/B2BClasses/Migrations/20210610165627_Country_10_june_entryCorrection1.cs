using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class Country_10_june_entryCorrection1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 1,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 22, 26, 24, 37, DateTimeKind.Local).AddTicks(2022));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 2,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 22, 26, 24, 37, DateTimeKind.Local).AddTicks(5908));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 3,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 22, 26, 24, 37, DateTimeKind.Local).AddTicks(5912));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 4,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 22, 26, 24, 37, DateTimeKind.Local).AddTicks(5916));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 1,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 22, 10, 24, 705, DateTimeKind.Local).AddTicks(5153));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 2,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 22, 10, 24, 705, DateTimeKind.Local).AddTicks(9829));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 3,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 22, 10, 24, 705, DateTimeKind.Local).AddTicks(9835));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 4,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 22, 10, 24, 705, DateTimeKind.Local).AddTicks(9837));
        }
    }
}
