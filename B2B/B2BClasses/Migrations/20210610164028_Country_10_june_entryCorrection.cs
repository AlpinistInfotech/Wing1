using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class Country_10_june_entryCorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tblActiveSerivceProvider",
                keyColumn: "ServiceProvider",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "tblActiveSerivceProvider",
                columns: new[] { "ServiceProvider", "IsEnabled", "ModifiedBy", "ModifiedDt", "Remarks" },
                values: new object[] { 1, true, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tblActiveSerivceProvider",
                keyColumn: "ServiceProvider",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "tblActiveSerivceProvider",
                columns: new[] { "ServiceProvider", "IsEnabled", "ModifiedBy", "ModifiedDt", "Remarks" },
                values: new object[] { 2, true, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" });

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 1,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 21, 56, 18, 17, DateTimeKind.Local).AddTicks(9202));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 2,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 21, 56, 18, 18, DateTimeKind.Local).AddTicks(4005));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 3,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 21, 56, 18, 18, DateTimeKind.Local).AddTicks(4009));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 4,
                column: "ModifyDt",
                value: new DateTime(2021, 6, 10, 21, 56, 18, 18, DateTimeKind.Local).AddTicks(4014));
        }
    }
}
