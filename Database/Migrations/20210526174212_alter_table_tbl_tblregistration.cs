using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class alter_table_tbl_tblregistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblRegistration_tblRegistration_SpNid",
                table: "tblRegistration");

            migrationBuilder.AddColumn<int>(
                name: "tblRegistrationSponsorNid",
                table: "tblRegistration",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblRegistration_tblRegistrationSponsorNid",
                table: "tblRegistration",
                column: "tblRegistrationSponsorNid");

            migrationBuilder.AddForeignKey(
                name: "FK_tblRegistration_tblRegistration_tblRegistrationSponsorNid",
                table: "tblRegistration",
                column: "tblRegistrationSponsorNid",
                principalTable: "tblRegistration",
                principalColumn: "Nid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblRegistration_tblRegistration_tblRegistrationSponsorNid",
                table: "tblRegistration");

            migrationBuilder.DropIndex(
                name: "IX_tblRegistration_tblRegistrationSponsorNid",
                table: "tblRegistration");

            migrationBuilder.DropColumn(
                name: "tblRegistrationSponsorNid",
                table: "tblRegistration");

            migrationBuilder.AddForeignKey(
                name: "FK_tblRegistration_tblRegistration_SpNid",
                table: "tblRegistration",
                column: "SpNid",
                principalTable: "tblRegistration",
                principalColumn: "Nid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
