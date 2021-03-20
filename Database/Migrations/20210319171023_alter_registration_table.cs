using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class alter_registration_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "remarks",
                table: "tblTcAddressDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "is_active",
                table: "tblRegistration",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "remarks",
                table: "tblTcAddressDetail");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "tblRegistration");
        }
    }
}
