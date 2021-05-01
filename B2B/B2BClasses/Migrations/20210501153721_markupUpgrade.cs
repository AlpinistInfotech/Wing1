using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class markupUpgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "classid",
                table: "tblWingMarkupFlightClass");

            migrationBuilder.RenameColumn(
                name: "MarkupAmt",
                table: "tblWingMarkupMaster",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "ConvenienceAmt",
                table: "tblWingConvenience",
                newName: "Amount");

            migrationBuilder.AddColumn<int>(
                name: "DirectFlight",
                table: "tblWingMarkupMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "tblWingMarkupMaster",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CabinClass",
                table: "tblWingMarkupFlightClass",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Applicability",
                table: "tblWingConvenience",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DirectFlight",
                table: "tblWingConvenience",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "tblWingConvenience",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllAirline",
                table: "tblWingConvenience",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllCustomer",
                table: "tblWingConvenience",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllCustomerType",
                table: "tblWingConvenience",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllFlightClass",
                table: "tblWingConvenience",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllPessengerType",
                table: "tblWingConvenience",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllProvider",
                table: "tblWingConvenience",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "tblWingConvenience",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tblWingConvenienceAirline",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    AirlineId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingConvenienceAirline", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingConvenienceAirline_tblAirline_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "tblAirline",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblWingConvenienceAirline_tblWingConvenience_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingConvenience",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingConvenienceCustomerDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingConvenienceCustomerDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingConvenienceCustomerDetails_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblWingConvenienceCustomerDetails_tblWingConvenience_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingConvenience",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingConvenienceCustomerType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    customerType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingConvenienceCustomerType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingConvenienceCustomerType_tblWingConvenience_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingConvenience",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingConvenienceFlightClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    CabinClass = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingConvenienceFlightClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingConvenienceFlightClass_tblWingConvenience_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingConvenience",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingConveniencePassengerType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    PassengerType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingConveniencePassengerType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingConveniencePassengerType_tblWingConvenience_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingConvenience",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingConvenienceServiceProvider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    ServiceProvider = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingConvenienceServiceProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingConvenienceServiceProvider_tblWingConvenience_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingConvenience",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblWingConvenienceAirline_AirlineId",
                table: "tblWingConvenienceAirline",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingConvenienceAirline_MarkupId",
                table: "tblWingConvenienceAirline",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingConvenienceCustomerDetails_CustomerId",
                table: "tblWingConvenienceCustomerDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingConvenienceCustomerDetails_MarkupId",
                table: "tblWingConvenienceCustomerDetails",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingConvenienceCustomerType_MarkupId",
                table: "tblWingConvenienceCustomerType",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingConvenienceFlightClass_MarkupId",
                table: "tblWingConvenienceFlightClass",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingConveniencePassengerType_MarkupId",
                table: "tblWingConveniencePassengerType",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingConvenienceServiceProvider_MarkupId",
                table: "tblWingConvenienceServiceProvider",
                column: "MarkupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblWingConvenienceAirline");

            migrationBuilder.DropTable(
                name: "tblWingConvenienceCustomerDetails");

            migrationBuilder.DropTable(
                name: "tblWingConvenienceCustomerType");

            migrationBuilder.DropTable(
                name: "tblWingConvenienceFlightClass");

            migrationBuilder.DropTable(
                name: "tblWingConveniencePassengerType");

            migrationBuilder.DropTable(
                name: "tblWingConvenienceServiceProvider");

            migrationBuilder.DropColumn(
                name: "DirectFlight",
                table: "tblWingMarkupMaster");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "tblWingMarkupMaster");

            migrationBuilder.DropColumn(
                name: "CabinClass",
                table: "tblWingMarkupFlightClass");

            migrationBuilder.DropColumn(
                name: "Applicability",
                table: "tblWingConvenience");

            migrationBuilder.DropColumn(
                name: "DirectFlight",
                table: "tblWingConvenience");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "tblWingConvenience");

            migrationBuilder.DropColumn(
                name: "IsAllAirline",
                table: "tblWingConvenience");

            migrationBuilder.DropColumn(
                name: "IsAllCustomer",
                table: "tblWingConvenience");

            migrationBuilder.DropColumn(
                name: "IsAllCustomerType",
                table: "tblWingConvenience");

            migrationBuilder.DropColumn(
                name: "IsAllFlightClass",
                table: "tblWingConvenience");

            migrationBuilder.DropColumn(
                name: "IsAllPessengerType",
                table: "tblWingConvenience");

            migrationBuilder.DropColumn(
                name: "IsAllProvider",
                table: "tblWingConvenience");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "tblWingConvenience");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "tblWingMarkupMaster",
                newName: "MarkupAmt");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "tblWingConvenience",
                newName: "ConvenienceAmt");

            migrationBuilder.AddColumn<string>(
                name: "classid",
                table: "tblWingMarkupFlightClass",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
