using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WingApi.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblTripJackTravelDetail",
                columns: table => new
                {
                    TravelDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CabinClass = table.Column<int>(type: "int", nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TraceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenrationDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MinPublishFare = table.Column<double>(type: "float", nullable: false),
                    JourneyType = table.Column<int>(type: "int", nullable: false),
                    AdultCount = table.Column<int>(type: "int", nullable: false),
                    ChildCount = table.Column<int>(type: "int", nullable: false),
                    InfantCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTripJackTravelDetail", x => x.TravelDetailId);
                });

            migrationBuilder.CreateTable(
                name: "tblTripJackTravelDetailResult",
                columns: table => new
                {
                    ResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelDetailId = table.Column<int>(type: "int", nullable: true),
                    ResultIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishedFare = table.Column<double>(type: "float", nullable: false),
                    OfferedFare = table.Column<double>(type: "float", nullable: false),
                    JsonData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTripJackTravelDetailResult", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_tblTripJackTravelDetailResult_tblTripJackTravelDetail_TravelDetailId",
                        column: x => x.TravelDetailId,
                        principalTable: "tblTripJackTravelDetail",
                        principalColumn: "TravelDetailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblTripJackTravelDetailResult_TravelDetailId",
                table: "tblTripJackTravelDetailResult",
                column: "TravelDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblTripJackTravelDetailResult");

            migrationBuilder.DropTable(
                name: "tblTripJackTravelDetail");
        }
    }
}
