using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTerminated",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "enmTCRanks",
                table: "tblRegistration",
                newName: "TCRanks");

            migrationBuilder.AddColumn<int>(
                name: "IsKycUpdated",
                table: "tblRegistration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "tblCaptcha",
                columns: table => new
                {
                    SaltId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CaptchaCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCaptcha", x => x.SaltId);
                });

            migrationBuilder.CreateTable(
                name: "tblTcAddressDetail",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    AddressType = table.Column<int>(type: "int", nullable: false),
                    address_line1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    address_line2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    Pincode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTcAddressDetail", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_tblTcAddressDetail_tblCountryMaster_CountryId",
                        column: x => x.CountryId,
                        principalTable: "tblCountryMaster",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblTcAddressDetail_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblTcAddressDetail_tblStateMaster_StateId",
                        column: x => x.StateId,
                        principalTable: "tblStateMaster",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblTcRanksDetails",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    TCRanks = table.Column<int>(type: "int", nullable: false),
                    QualifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PPRequired = table.Column<double>(type: "float", nullable: false),
                    PPDone = table.Column<double>(type: "float", nullable: false),
                    Isdeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTcRanksDetails", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_tblTcRanksDetails_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblTcSequcence",
                columns: table => new
                {
                    TcSequcence = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Monthyear = table.Column<int>(type: "int", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    CurrentSeq = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTcSequcence", x => x.TcSequcence);
                });

            migrationBuilder.CreateTable(
                name: "tblTree",
                columns: table => new
                {
                    TreeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: false),
                    TcSpNid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTree", x => x.TreeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblTcAddressDetail_CountryId",
                table: "tblTcAddressDetail",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblTcAddressDetail_StateId",
                table: "tblTcAddressDetail",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_tblTcAddressDetail_TcNid",
                table: "tblTcAddressDetail",
                column: "TcNid");

            migrationBuilder.CreateIndex(
                name: "IX_tblTcRanksDetails_TcNid",
                table: "tblTcRanksDetails",
                column: "TcNid");

            migrationBuilder.CreateIndex(
                name: "IX_tblTcSequcence_Monthyear_StateId",
                table: "tblTcSequcence",
                columns: new[] { "Monthyear", "StateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblTree_TcNid",
                table: "tblTree",
                column: "TcNid");

            migrationBuilder.CreateIndex(
                name: "IX_tblTree_TcSpNid",
                table: "tblTree",
                column: "TcSpNid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblCaptcha");

            migrationBuilder.DropTable(
                name: "tblTcAddressDetail");

            migrationBuilder.DropTable(
                name: "tblTcRanksDetails");

            migrationBuilder.DropTable(
                name: "tblTcSequcence");

            migrationBuilder.DropTable(
                name: "tblTree");

            migrationBuilder.DropColumn(
                name: "IsKycUpdated",
                table: "tblRegistration");

            migrationBuilder.RenameColumn(
                name: "TCRanks",
                table: "tblRegistration",
                newName: "enmTCRanks");

            migrationBuilder.AddColumn<bool>(
                name: "IsTerminated",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
