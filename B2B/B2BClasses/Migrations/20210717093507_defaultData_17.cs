using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class defaultData_17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tblRoleClaim",
                columns: new[] { "Id", "ClaimId", "CreatedBy", "CreatedDt", "IsDeleted", "ModifiedBy", "ModifiedDt", "Role" },
                values: new object[,]
                {
                    { 39, 10102, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 40, 10103, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 128, 10102, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 129, 10103, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "tblRoleClaim",
                keyColumn: "Id",
                keyValue: 129);
        }
    }
}
