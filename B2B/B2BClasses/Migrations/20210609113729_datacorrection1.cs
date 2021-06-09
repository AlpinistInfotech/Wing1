using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class datacorrection1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "tblCustomerPanDetails",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "tblCustomerBankDetails",
                newName: "IsDeleted");

            migrationBuilder.AddColumn<int>(
                name: "ModifyBy",
                table: "tblCustomerMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDt",
                table: "tblCustomerMaster",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ModifyBy", "ModifyDt" },
                values: new object[] { 1, new DateTime(2021, 6, 9, 17, 7, 25, 235, DateTimeKind.Local).AddTicks(4150) });

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ModifyBy", "ModifyDt" },
                values: new object[] { 1, new DateTime(2021, 6, 9, 17, 7, 25, 235, DateTimeKind.Local).AddTicks(8500) });

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ModifyBy", "ModifyDt" },
                values: new object[] { 1, new DateTime(2021, 6, 9, 17, 7, 25, 235, DateTimeKind.Local).AddTicks(8505) });

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ModifyBy", "ModifyDt" },
                values: new object[] { 1, new DateTime(2021, 6, 9, 17, 7, 25, 235, DateTimeKind.Local).AddTicks(8507) });

            migrationBuilder.InsertData(
                table: "tblRoleClaim",
                columns: new[] { "Id", "ClaimId", "CreatedBy", "CreatedDt", "IsDeleted", "ModifiedBy", "ModifiedDt", "Role" },
                values: new object[,]
                {
                    { 125, 10013, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 124, 10012, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 123, 10011, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 122, 10010, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 121, 10009, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 120, 10008, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 119, 10007, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 118, 10006, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 117, 10005, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 116, 10004, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 115, 10003, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 114, 10002, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 34, 10013, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 33, 10012, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 32, 10011, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 31, 10010, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 30, 10009, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 29, 10008, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 28, 10007, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 27, 10006, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 26, 10005, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 25, 10004, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 24, 10003, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 23, 10002, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 113, 10001, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 22, 10001, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DropColumn(
                name: "ModifyBy",
                table: "tblCustomerMaster");

            migrationBuilder.DropColumn(
                name: "ModifyDt",
                table: "tblCustomerMaster");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "tblCustomerPanDetails",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "tblCustomerBankDetails",
                newName: "Isdeleted");
        }
    }
}
