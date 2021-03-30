using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class add_tc_status_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblTCStatus",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    status_type = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<byte>(type: "tinyint", nullable: false),
                    action_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    action_by = table.Column<int>(type: "int", nullable: false),
                    action_datetime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTCStatus", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_tblTCStatus_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblTCStatus_TcNid",
                table: "tblTCStatus",
                column: "TcNid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblTCStatus");
        }
    }
}
