using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations.LogDB
{
    public partial class firsLogFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblCustomerBankDetailsLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    BankId = table.Column<int>(type: "int", nullable: true),
                    IFSC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AccountNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BranchAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NameasonBank = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UpiId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UploadImages = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsApproved = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ApprovedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomerBankDetailsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblCustomerGSTDetailsLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GstNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegisteredName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    PinCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomerGSTDetailsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblCustomerIPFilterLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AllowedAllIp = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IPDetails = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomerIPFilterLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblCustomerMarkupLog",
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
                    table.PrimaryKey("PK_tblCustomerMarkupLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblCustomerMasterLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HaveGST = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    PinCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AlternateNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CustomerType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyBy = table.Column<int>(type: "int", nullable: false),
                    ModifyDt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomerMasterLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblCustomerPanDetailsLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    PANName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PANNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    UploadImages = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsApproved = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ApprovedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomerPanDetailsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblUserMasterLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ForcePasswordChange = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsMailVarified = table.Column<bool>(type: "bit", nullable: false),
                    MailVerficationTokken = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TokkenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    BlockStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlockEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoginFailCount = table.Column<int>(type: "int", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserMasterLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblWalletBalanceAlertLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    MinBalance = table.Column<double>(type: "float", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblWalletBalanceAlertLog", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblCustomerBankDetailsLog");

            migrationBuilder.DropTable(
                name: "tblCustomerGSTDetailsLog");

            migrationBuilder.DropTable(
                name: "tblCustomerIPFilterLog");

            migrationBuilder.DropTable(
                name: "tblCustomerMarkupLog");

            migrationBuilder.DropTable(
                name: "tblCustomerMasterLog");

            migrationBuilder.DropTable(
                name: "tblCustomerPanDetailsLog");

            migrationBuilder.DropTable(
                name: "tblUserMasterLog");

            migrationBuilder.DropTable(
                name: "tblWalletBalanceAlertLog");
        }
    }
}
