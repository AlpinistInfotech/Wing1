using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class NOMINEE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblTcNominee",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    NomineeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NomineeRelation = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Isdeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblTcNominee", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_TblTcNominee_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblTcNominee_TcNid",
                table: "TblTcNominee",
                column: "TcNid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblTcNominee");
        }
    }
}
