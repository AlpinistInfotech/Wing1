using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class tal_adress_alter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "tblTcAddressDetail",
                newName: "landMark");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "tblTcAddressDetail",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tblCityMaster",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    IsUT = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCityMaster", x => x.CityId);
                    table.ForeignKey(
                        name: "FK_tblCityMaster_tblCountryMaster_CountryId",
                        column: x => x.CountryId,
                        principalTable: "tblCountryMaster",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblCityMaster_tblStateMaster_StateId",
                        column: x => x.StateId,
                        principalTable: "tblStateMaster",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblTcAddressDetail_CityId",
                table: "tblTcAddressDetail",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_tblCityMaster_CountryId",
                table: "tblCityMaster",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblCityMaster_StateId",
                table: "tblCityMaster",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblTcAddressDetail_tblCityMaster_CityId",
                table: "tblTcAddressDetail",
                column: "CityId",
                principalTable: "tblCityMaster",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblTcAddressDetail_tblCityMaster_CityId",
                table: "tblTcAddressDetail");

            migrationBuilder.DropTable(
                name: "tblCityMaster");

            migrationBuilder.DropIndex(
                name: "IX_tblTcAddressDetail_CityId",
                table: "tblTcAddressDetail");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "tblTcAddressDetail");

            migrationBuilder.RenameColumn(
                name: "landMark",
                table: "tblTcAddressDetail",
                newName: "Remarks");
        }
    }
}
