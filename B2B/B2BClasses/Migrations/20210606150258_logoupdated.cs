using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class logoupdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedAPI",
                table: "tblCustomerIPFilter");

            migrationBuilder.DropColumn(
                name: "AllowedGUI",
                table: "tblCustomerIPFilter");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "tblUserMaster",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "tblUserMaster",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "tblCustomerMaster",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "tblUserMaster");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "tblUserMaster");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "tblCustomerMaster");

            migrationBuilder.AddColumn<bool>(
                name: "AllowedAPI",
                table: "tblCustomerIPFilter",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowedGUI",
                table: "tblCustomerIPFilter",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
