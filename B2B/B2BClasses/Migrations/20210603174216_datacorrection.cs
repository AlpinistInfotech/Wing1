using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class datacorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tblCustomerBalence",
                keyColumn: "Id",
                keyValue: 2,
                column: "CustomerId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "tblCustomerBalence",
                keyColumn: "Id",
                keyValue: 3,
                column: "CustomerId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "tblCustomerBalence",
                keyColumn: "Id",
                keyValue: 4,
                column: "CustomerId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "tblCustomerIPFilter",
                keyColumn: "Id",
                keyValue: 2,
                column: "CustomerId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "tblCustomerIPFilter",
                keyColumn: "Id",
                keyValue: 3,
                column: "CustomerId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "tblCustomerIPFilter",
                keyColumn: "Id",
                keyValue: 4,
                column: "CustomerId",
                value: 4);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tblCustomerBalence",
                keyColumn: "Id",
                keyValue: 2,
                column: "CustomerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "tblCustomerBalence",
                keyColumn: "Id",
                keyValue: 3,
                column: "CustomerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "tblCustomerBalence",
                keyColumn: "Id",
                keyValue: 4,
                column: "CustomerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "tblCustomerIPFilter",
                keyColumn: "Id",
                keyValue: 2,
                column: "CustomerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "tblCustomerIPFilter",
                keyColumn: "Id",
                keyValue: 3,
                column: "CustomerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "tblCustomerIPFilter",
                keyColumn: "Id",
                keyValue: 4,
                column: "CustomerId",
                value: 1);
        }
    }
}
