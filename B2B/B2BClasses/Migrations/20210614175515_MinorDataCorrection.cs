using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class MinorDataCorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MPin",
                table: "tblCustomerBalence",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 1,
                column: "ModifyDt",
                value: new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 2,
                column: "ModifyDt",
                value: new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 3,
                column: "ModifyDt",
                value: new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 4,
                column: "ModifyDt",
                value: new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

            migrationBuilder.InsertData(
                table: "tblRoleClaim",
                columns: new[] { "Id", "ClaimId", "CreatedBy", "CreatedDt", "IsDeleted", "ModifiedBy", "ModifiedDt", "Role" },
                values: new object[] { 35, 10013, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.UpdateData(
                table: "tblUserMaster",
                keyColumn: "Id",
                keyValue: 11,
                column: "IsPrimary",
                value: true);

            migrationBuilder.UpdateData(
                table: "tblUserMaster",
                keyColumn: "Id",
                keyValue: 21,
                column: "IsPrimary",
                value: true);

            migrationBuilder.UpdateData(
                table: "tblUserMaster",
                keyColumn: "Id",
                keyValue: 31,
                column: "IsPrimary",
                value: true);

            migrationBuilder.UpdateData(
                table: "tblUserMaster",
                keyColumn: "Id",
                keyValue: 41,
                column: "IsPrimary",
                value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.AlterColumn<string>(
                name: "MPin",
                table: "tblCustomerBalence",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

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

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 22,
                column: "ClaimId",
                value: 10001);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 23,
                column: "ClaimId",
                value: 10002);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 24,
                column: "ClaimId",
                value: 10003);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 25,
                column: "ClaimId",
                value: 10004);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 26,
                column: "ClaimId",
                value: 10005);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 27,
                column: "ClaimId",
                value: 10006);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 28,
                column: "ClaimId",
                value: 10007);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 29,
                column: "ClaimId",
                value: 10008);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 30,
                column: "ClaimId",
                value: 10009);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 31,
                column: "ClaimId",
                value: 10010);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 32,
                column: "ClaimId",
                value: 10011);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 33,
                column: "ClaimId",
                value: 10012);

            migrationBuilder.UpdateData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 34,
                column: "ClaimId",
                value: 10013);

            migrationBuilder.UpdateData(
                table: "tblUserMaster",
                keyColumn: "Id",
                keyValue: 11,
                column: "IsPrimary",
                value: false);

            migrationBuilder.UpdateData(
                table: "tblUserMaster",
                keyColumn: "Id",
                keyValue: 21,
                column: "IsPrimary",
                value: false);

            migrationBuilder.UpdateData(
                table: "tblUserMaster",
                keyColumn: "Id",
                keyValue: 31,
                column: "IsPrimary",
                value: false);

            migrationBuilder.UpdateData(
                table: "tblUserMaster",
                keyColumn: "Id",
                keyValue: 41,
                column: "IsPrimary",
                value: false);
        }
    }
}
