using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class creditrequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblCreditRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CreditAmt = table.Column<double>(type: "float", nullable: false),
                    IsApproved = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCreditRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblCreditRequest_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblCreditRequest_CustomerId",
                table: "tblCreditRequest",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblCreditRequest");
        }
    }
}
