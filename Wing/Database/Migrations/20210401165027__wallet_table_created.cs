using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class _wallet_table_created : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status_type",
                table: "tblTCStatus",
                newName: "action_type");

            migrationBuilder.CreateTable(
                name: "tblTCwallet",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    walletamt = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTCwallet", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_tblTCwallet_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblTCwalletlog",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    groupid = table.Column<int>(type: "int", nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reqno = table.Column<int>(type: "int", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createddatetime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTCwalletlog", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_tblTCwalletlog_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblTCwallet_TcNid",
                table: "tblTCwallet",
                column: "TcNid");

            migrationBuilder.CreateIndex(
                name: "IX_tblTCwalletlog_TcNid",
                table: "tblTCwalletlog",
                column: "TcNid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblTCwallet");

            migrationBuilder.DropTable(
                name: "tblTCwalletlog");

            migrationBuilder.RenameColumn(
                name: "action_type",
                table: "tblTCStatus",
                newName: "status_type");
        }
    }
}
