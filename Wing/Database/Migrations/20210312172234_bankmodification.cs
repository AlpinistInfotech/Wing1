using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class bankmodification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameasonBank",
                table: "tblTcBankDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TblTcPanDetails",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    PANName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PANNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    UploadImages = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsApproved = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ApprovedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Isdeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTcPanDetails", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_TblTcPanDetails_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblTcPanDetails_TcNid",
                table: "TblTcPanDetails",
                column: "TcNid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblTcPanDetails");

            migrationBuilder.DropColumn(
                name: "NameasonBank",
                table: "tblTcBankDetails");
        }
    }
}
