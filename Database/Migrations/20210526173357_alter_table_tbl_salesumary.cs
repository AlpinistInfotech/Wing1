using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class alter_table_tbl_salesumary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblTCinvoice",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    inv_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    inv_amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    pv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    bv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    createddatetime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTCinvoice", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_tblTCinvoice_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblTCmemberrank",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    levelid = table.Column<int>(type: "int", nullable: false),
                    displayid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTCmemberrank", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_tblTCmemberrank_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblTCmemberranklog",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    levelid = table.Column<int>(type: "int", nullable: false),
                    qualifieddate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTCmemberranklog", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_tblTCmemberranklog_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblTCsalesummary",
                columns: table => new
                {
                    salesummaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    monthyear = table.Column<int>(type: "int", nullable: false),
                    TcSpNid = table.Column<int>(type: "int", nullable: true),
                    tblRegistrationSpNid = table.Column<int>(type: "int", nullable: true),
                    TcNidlevelid = table.Column<int>(type: "int", nullable: false),
                    TcSpNidlevelid = table.Column<int>(type: "int", nullable: false),
                    incentive = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTCsalesummary", x => x.salesummaryId);
                    table.ForeignKey(
                        name: "FK_tblTCsalesummary_tblRegistration_tblRegistrationSpNid",
                        column: x => x.tblRegistrationSpNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblTCsalesummary_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblTCstatement",
                columns: table => new
                {
                    inentiveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcNid = table.Column<int>(type: "int", nullable: true),
                    monthyear = table.Column<int>(type: "int", nullable: false),
                    incentive = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    deduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tds = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    netincentive = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    incentivestatus = table.Column<int>(type: "int", nullable: false),
                    createddatetime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTCstatement", x => x.inentiveId);
                    table.ForeignKey(
                        name: "FK_tblTCstatement_tblRegistration_TcNid",
                        column: x => x.TcNid,
                        principalTable: "tblRegistration",
                        principalColumn: "Nid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblTCstatementlog",
                columns: table => new
                {
                    DetailedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    inentiveId = table.Column<int>(type: "int", nullable: true),
                    incentive_status = table.Column<int>(type: "int", nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createddatetime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTCstatementlog", x => x.DetailedId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblTCinvoice_TcNid",
                table: "tblTCinvoice",
                column: "TcNid");

            migrationBuilder.CreateIndex(
                name: "IX_tblTCmemberrank_TcNid",
                table: "tblTCmemberrank",
                column: "TcNid");

            migrationBuilder.CreateIndex(
                name: "IX_tblTCmemberranklog_TcNid",
                table: "tblTCmemberranklog",
                column: "TcNid");

            migrationBuilder.CreateIndex(
                name: "IX_tblTCsalesummary_tblRegistrationSpNid",
                table: "tblTCsalesummary",
                column: "tblRegistrationSpNid");

            migrationBuilder.CreateIndex(
                name: "IX_tblTCsalesummary_TcNid",
                table: "tblTCsalesummary",
                column: "TcNid");

            migrationBuilder.CreateIndex(
                name: "IX_tblTCstatement_TcNid",
                table: "tblTCstatement",
                column: "TcNid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblTCinvoice");

            migrationBuilder.DropTable(
                name: "tblTCmemberrank");

            migrationBuilder.DropTable(
                name: "tblTCmemberranklog");

            migrationBuilder.DropTable(
                name: "tblTCsalesummary");

            migrationBuilder.DropTable(
                name: "tblTCstatement");

            migrationBuilder.DropTable(
                name: "tblTCstatementlog");
        }
    }
}
