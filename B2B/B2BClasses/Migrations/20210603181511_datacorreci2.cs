using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class datacorreci2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 1,
                column: "CustomerType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 2,
                column: "CustomerType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 3,
                column: "CustomerType",
                value: 5);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 1,
                column: "CustomerType",
                value: 0);

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 2,
                column: "CustomerType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "tblCustomerMaster",
                keyColumn: "Id",
                keyValue: 3,
                column: "CustomerType",
                value: 4);
        }
    }
}
