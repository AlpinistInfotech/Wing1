using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblActiveSerivceProvider",
                columns: table => new
                {
                    ServiceProvider = table.Column<int>(type: "int", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    DisabledFromDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblActiveSerivceProvider", x => x.ServiceProvider);
                });

            migrationBuilder.CreateTable(
                name: "tblActiveSerivceProviderLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceProvider = table.Column<int>(type: "int", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    IsDisabledBySystem = table.Column<bool>(type: "bit", nullable: false),
                    DisabledFromDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblActiveSerivceProviderLog", x => x.Id);
                });

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
                name: "tblCustomerMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternateNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WalletBalence = table.Column<double>(type: "float", nullable: false),
                    CreditBalence = table.Column<double>(type: "float", nullable: false),
                    CustomerType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomerMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblMinBalenceAlert",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ForAllServiceProvider = table.Column<bool>(type: "bit", nullable: false),
                    AmountForSendMail = table.Column<double>(type: "float", nullable: false),
                    AmountForDisableProvider = table.Column<double>(type: "float", nullable: false),
                    EffectiveFromDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblMinBalenceAlert", x => x.Id);
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
                name: "tblTboTokenDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgencyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenrationDt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTboTokenDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblTboTravelDetail",
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
                    MinPublishFareReturn = table.Column<double>(type: "float", nullable: false),
                    JourneyType = table.Column<int>(type: "int", nullable: false),
                    AdultCount = table.Column<int>(type: "int", nullable: false),
                    ChildCount = table.Column<int>(type: "int", nullable: false),
                    InfantCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTboTravelDetail", x => x.TravelDetailId);
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
                    MinPublishFareReturn = table.Column<double>(type: "float", nullable: false),
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
                name: "tblWingConvenience",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConvenienceAmt = table.Column<double>(type: "float", nullable: false),
                    EffectiveFromDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveToDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingConvenience", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblWingMarkupMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAllProvider = table.Column<bool>(type: "bit", nullable: false),
                    IsAllCustomer = table.Column<bool>(type: "bit", nullable: false),
                    IsApplicableOnEachPasenger = table.Column<bool>(type: "bit", nullable: false),
                    IsAllPessenger = table.Column<bool>(type: "bit", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    MarkupAmt = table.Column<double>(type: "float", nullable: false),
                    EffectiveFromDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveToDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingMarkupMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblCustomerIPFilter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    AllowedAllIp = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomerIPFilter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblCustomerIPFilter_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblCustomerMarkup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    MarkupAmt = table.Column<double>(type: "float", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomerMarkup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblCustomerMarkup_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblUserMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblUserMaster_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWalletBalanceAlert",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    MinBalance = table.Column<double>(type: "float", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWalletBalanceAlert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWalletBalanceAlert_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWalletDetailLedger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    Credit = table.Column<double>(type: "float", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    TransactionDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWalletDetailLedger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWalletDetailLedger_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWalletDetailLedgerLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FiscalYear = table.Column<int>(type: "int", nullable: false),
                    TransactionDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    Credit = table.Column<double>(type: "float", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    TransactionDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWalletDetailLedgerLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWalletDetailLedgerLog_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblMinBalenceAlertDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlertId = table.Column<int>(type: "int", nullable: true),
                    ServiceProvider = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblMinBalenceAlertDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblMinBalenceAlertDetails_tblMinBalenceAlert_AlertId",
                        column: x => x.AlertId,
                        principalTable: "tblMinBalenceAlert",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblTboTravelDetailResult",
                columns: table => new
                {
                    ResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelDetailId = table.Column<int>(type: "int", nullable: true),
                    segmentId = table.Column<int>(type: "int", nullable: false),
                    ResultIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishedFare = table.Column<double>(type: "float", nullable: false),
                    OfferedFare = table.Column<double>(type: "float", nullable: false),
                    JsonData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTboTravelDetailResult", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_tblTboTravelDetailResult_tblTboTravelDetail_TravelDetailId",
                        column: x => x.TravelDetailId,
                        principalTable: "tblTboTravelDetail",
                        principalColumn: "TravelDetailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblTripJackTravelDetailResult",
                columns: table => new
                {
                    ResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelDetailId = table.Column<int>(type: "int", nullable: true),
                    segmentId = table.Column<int>(type: "int", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "tblWingMarkupCustomerDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingMarkupCustomerDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingMarkupCustomerDetails_tblCustomerMaster_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tblCustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblWingMarkupCustomerDetails_tblWingMarkupMaster_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingMarkupMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingMarkupPassengerType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    PassengerType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingMarkupPassengerType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingMarkupPassengerType_tblWingMarkupMaster_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingMarkupMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblWingMarkupServiceProvider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkupId = table.Column<int>(type: "int", nullable: true),
                    ServiceProvider = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWingMarkupServiceProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblWingMarkupServiceProvider_tblWingMarkupMaster_MarkupId",
                        column: x => x.MarkupId,
                        principalTable: "tblWingMarkupMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblCustomerIPFilterDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilterId = table.Column<int>(type: "int", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomerIPFilterDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblCustomerIPFilterDetails_tblCustomerIPFilter_FilterId",
                        column: x => x.FilterId,
                        principalTable: "tblCustomerIPFilter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblUserRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblUserRole_tblUserMaster_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUserMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomerIPFilter_CustomerId",
                table: "tblCustomerIPFilter",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomerIPFilterDetails_FilterId",
                table: "tblCustomerIPFilterDetails",
                column: "FilterId");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomerMarkup_CustomerId",
                table: "tblCustomerMarkup",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblMinBalenceAlertDetails_AlertId",
                table: "tblMinBalenceAlertDetails",
                column: "AlertId");

            migrationBuilder.CreateIndex(
                name: "IX_tblTboTravelDetailResult_TravelDetailId",
                table: "tblTboTravelDetailResult",
                column: "TravelDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_tblTripJackTravelDetailResult_TravelDetailId",
                table: "tblTripJackTravelDetailResult",
                column: "TravelDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserMaster_CustomerId",
                table: "tblUserMaster",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserRole_UserId",
                table: "tblUserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWalletBalanceAlert_CustomerId",
                table: "tblWalletBalanceAlert",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWalletDetailLedger_CustomerId",
                table: "tblWalletDetailLedger",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWalletDetailLedgerLog_CustomerId",
                table: "tblWalletDetailLedgerLog",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingMarkupCustomerDetails_CustomerId",
                table: "tblWingMarkupCustomerDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingMarkupCustomerDetails_MarkupId",
                table: "tblWingMarkupCustomerDetails",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingMarkupPassengerType_MarkupId",
                table: "tblWingMarkupPassengerType",
                column: "MarkupId");

            migrationBuilder.CreateIndex(
                name: "IX_tblWingMarkupServiceProvider_MarkupId",
                table: "tblWingMarkupServiceProvider",
                column: "MarkupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblActiveSerivceProvider");

            migrationBuilder.DropTable(
                name: "tblActiveSerivceProviderLog");

            migrationBuilder.DropTable(
                name: "tblAirlineFareRule");

            migrationBuilder.DropTable(
                name: "tblCustomerIPFilterDetails");

            migrationBuilder.DropTable(
                name: "tblCustomerMarkup");

            migrationBuilder.DropTable(
                name: "tblMinBalenceAlertDetails");

            migrationBuilder.DropTable(
                name: "tblTboFareRule");

            migrationBuilder.DropTable(
                name: "tblTboTokenDetails");

            migrationBuilder.DropTable(
                name: "tblTboTravelDetailResult");

            migrationBuilder.DropTable(
                name: "tblTripJackFareRule");

            migrationBuilder.DropTable(
                name: "tblTripJackTravelDetailResult");

            migrationBuilder.DropTable(
                name: "tblUserRole");

            migrationBuilder.DropTable(
                name: "tblWalletBalanceAlert");

            migrationBuilder.DropTable(
                name: "tblWalletDetailLedger");

            migrationBuilder.DropTable(
                name: "tblWalletDetailLedgerLog");

            migrationBuilder.DropTable(
                name: "tblWingConvenience");

            migrationBuilder.DropTable(
                name: "tblWingMarkupCustomerDetails");

            migrationBuilder.DropTable(
                name: "tblWingMarkupPassengerType");

            migrationBuilder.DropTable(
                name: "tblWingMarkupServiceProvider");

            migrationBuilder.DropTable(
                name: "tblCustomerIPFilter");

            migrationBuilder.DropTable(
                name: "tblMinBalenceAlert");

            migrationBuilder.DropTable(
                name: "tblTboTravelDetail");

            migrationBuilder.DropTable(
                name: "tblTripJackTravelDetail");

            migrationBuilder.DropTable(
                name: "tblUserMaster");

            migrationBuilder.DropTable(
                name: "tblWingMarkupMaster");

            migrationBuilder.DropTable(
                name: "tblCustomerMaster");
        }
    }
}
