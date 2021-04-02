using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WingApi.Migrations
{
    public partial class farerule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "segmentId",
                table: "tblTripJackTravelDetailResult",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "MinPublishFareReturn",
                table: "tblTripJackTravelDetail",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "segmentId",
                table: "tblTboTravelDetailResult",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "MinPublishFareReturn",
                table: "tblTboTravelDetail",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "tblAirlineFareRule",
                columns: table => new
                {
                    AirlineFareRuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceProvider = table.Column<int>(type: "int", nullable: false),
                    traceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resultIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReissueCharge = table.Column<double>(type: "float", nullable: false),
                    ReissueAdditionalCharge = table.Column<double>(type: "float", nullable: false),
                    ReissuePolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancellationCharge = table.Column<double>(type: "float", nullable: false),
                    CancellationAdditionalCharge = table.Column<double>(type: "float", nullable: false),
                    CancellationPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckingBaggage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CabinBaggage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAirlineFareRule", x => x.AirlineFareRuleId);
                });

            migrationBuilder.CreateTable(
                name: "tblTboFareRule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TraceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenrationDt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTboFareRule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblTripJackFareRule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TraceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenrationDt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTripJackFareRule", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblAirlineFareRule");

            migrationBuilder.DropTable(
                name: "tblTboFareRule");

            migrationBuilder.DropTable(
                name: "tblTripJackFareRule");

            migrationBuilder.DropColumn(
                name: "segmentId",
                table: "tblTripJackTravelDetailResult");

            migrationBuilder.DropColumn(
                name: "MinPublishFareReturn",
                table: "tblTripJackTravelDetail");

            migrationBuilder.DropColumn(
                name: "segmentId",
                table: "tblTboTravelDetailResult");

            migrationBuilder.DropColumn(
                name: "MinPublishFareReturn",
                table: "tblTboTravelDetail");
        }
    }
}
