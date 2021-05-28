using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class alter_markupdiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayCount",
                table: "tblWingMarkupMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DayCount",
                table: "tblWingDiscount",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DayCount",
                table: "tblWingConvenience",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayCount",
                table: "tblWingMarkupMaster");

            migrationBuilder.DropColumn(
                name: "DayCount",
                table: "tblWingDiscount");

            migrationBuilder.DropColumn(
                name: "DayCount",
                table: "tblWingConvenience");
        }
    }
}
