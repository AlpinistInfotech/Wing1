using Microsoft.EntityFrameworkCore.Migrations;

namespace B2BClasses.Migrations
{
    public partial class defaulbank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tblBankMaster",
                columns: new[] { "BankId", "BankName", "IsActive" },
                values: new object[,]
                {
                    { 1, "American Express", true },
                    { 21, "Indian Overseas Bank", true },
                    { 22, "Punjab National Bank", true },
                    { 23, "State Bank of India", true },
                    { 24, "State Bank of Bikaner & Jaipur", true },
                    { 25, "State Bank of Mysore", true },
                    { 26, "State Bank of Hyderabad", true },
                    { 27, "UCO Bank", true },
                    { 28, "United Bank of India", true },
                    { 29, "Vijaya Bank", true },
                    { 30, "Private Banks in Delhi NCR:-", true },
                    { 31, "HDFC Bank", true },
                    { 32, "ICICI Bank", true },
                    { 33, "IDBI Bank", true },
                    { 34, "Axis Bank", true },
                    { 35, "Syndicate Bank", true },
                    { 20, "Dena Bank", true },
                    { 36, "Lord Krishna Bank", true },
                    { 19, "Central Bank of India", true },
                    { 17, "Bank of Maharashtra", true },
                    { 2, "ANZ Grindlays", true },
                    { 3, "Bank of America", true },
                    { 4, "Bank of Nova Scotia", true },
                    { 5, "Bank of Tokyo", true },
                    { 6, "Banque Nationale de Paris", true },
                    { 7, "Citibank", true },
                    { 8, "Credit Lyonnais", true },
                    { 9, "Deutsche bank", true },
                    { 10, "Hong Kong & Shanghai", true },
                    { 11, "Standard Chartered", true },
                    { 12, "Societe Generale", true },
                    { 13, "Sanwa Bank", true },
                    { 14, "Allahabad Bank", true },
                    { 15, "Andhra Bank", true },
                    { 16, "Bank of Baroda", true },
                    { 18, "Canara Bank", true },
                    { 37, "IndusInd Bank", true }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "tblBankMaster",
                keyColumn: "BankId",
                keyValue: 37);
        }
    }
}
