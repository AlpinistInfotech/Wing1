using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class markupchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisabledFromDt",
                table: "tblActiveSerivceProvider");

            migrationBuilder.RenameColumn(
                name: "IsDisabled",
                table: "tblActiveSerivceProvider",
                newName: "IsEnabled");

            migrationBuilder.AddColumn<bool>(
                name: "IsAllFlightClass",
                table: "tblWingMarkupMaster",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAllFlightClass",
                table: "tblWingMarkupMaster");

            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "tblActiveSerivceProvider",
                newName: "IsDisabled");

            migrationBuilder.AddColumn<DateTime>(
                name: "DisabledFromDt",
                table: "tblActiveSerivceProvider",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
