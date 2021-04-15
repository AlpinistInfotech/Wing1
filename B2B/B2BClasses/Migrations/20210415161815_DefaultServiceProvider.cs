using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class DefaultServiceProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tblActiveSerivceProvider",
                columns: new[] { "ServiceProvider", "IsEnabled", "ModifiedBy", "ModifiedDt", "Remarks" },
                values: new object[] { 2, true, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tblActiveSerivceProvider",
                keyColumn: "ServiceProvider",
                keyValue: 2);
        }
    }
}
