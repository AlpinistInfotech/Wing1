using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class add_status_column_in_holiday_master : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "is_active",
                table: "tblHolidayPackageMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "tblHolidayPackageMaster");
        }
    }
}
