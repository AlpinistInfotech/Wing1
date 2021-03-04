using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tblStateMaster_CountryId",
                table: "tblStateMaster");

            migrationBuilder.AlterColumn<string>(
                name: "StateName",
                table: "tblStateMaster",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StateCode",
                table: "tblStateMaster",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "latitude",
                table: "tblStateMaster",
                type: "decimal(18,14)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "longitude",
                table: "tblStateMaster",
                type: "decimal(18,14)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "tblCountryMaster",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Capital",
                table: "tblCountryMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCodeIso2",
                table: "tblCountryMaster",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencySymbol",
                table: "tblCountryMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "tblCountryMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Native",
                table: "tblCountryMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneCode",
                table: "tblCountryMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "tblCountryMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubRegion",
                table: "tblCountryMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "tblCountryMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblStateMaster_CountryId_StateName",
                table: "tblStateMaster",
                columns: new[] { "CountryId", "StateName" },
                unique: true,
                filter: "[CountryId] IS NOT NULL AND [StateName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tblCountryMaster_CountryName",
                table: "tblCountryMaster",
                column: "CountryName",
                unique: true,
                filter: "[CountryName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tblStateMaster_CountryId_StateName",
                table: "tblStateMaster");

            migrationBuilder.DropIndex(
                name: "IX_tblCountryMaster_CountryName",
                table: "tblCountryMaster");

            migrationBuilder.DropColumn(
                name: "latitude",
                table: "tblStateMaster");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "tblStateMaster");

            migrationBuilder.DropColumn(
                name: "Capital",
                table: "tblCountryMaster");

            migrationBuilder.DropColumn(
                name: "CountryCodeIso2",
                table: "tblCountryMaster");

            migrationBuilder.DropColumn(
                name: "CurrencySymbol",
                table: "tblCountryMaster");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "tblCountryMaster");

            migrationBuilder.DropColumn(
                name: "Native",
                table: "tblCountryMaster");

            migrationBuilder.DropColumn(
                name: "PhoneCode",
                table: "tblCountryMaster");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "tblCountryMaster");

            migrationBuilder.DropColumn(
                name: "SubRegion",
                table: "tblCountryMaster");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "tblCountryMaster");

            migrationBuilder.AlterColumn<string>(
                name: "StateName",
                table: "tblStateMaster",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StateCode",
                table: "tblStateMaster",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "tblCountryMaster",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblStateMaster_CountryId",
                table: "tblStateMaster",
                column: "CountryId");
        }
    }
}
