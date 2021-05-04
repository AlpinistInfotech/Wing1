using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class markupdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 15,
                column: "Role",
                value: 104);

            migrationBuilder.InsertData(
                table: "tblUserRole",
                columns: new[] { "Id", "CreatedBy", "CreatedDt", "IsDeleted", "ModifiedBy", "ModifiedDt", "Role", "UserId" },
                values: new object[] { 16, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 111, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.UpdateData(
                table: "tblUserRole",
                keyColumn: "Id",
                keyValue: 15,
                column: "Role",
                value: 111);
        }
    }
}
