using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.DB
{
    public class MasterContext : DbContext
    {
        public MasterContext(DbContextOptions<MasterContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //DefaultData defaultData = new DefaultData(modelBuilder);
            //defaultData.InsertCategory();
            //defaultData.InserSubCategory();
            //modelBuilder.Entity<tblCountryMaster>().HasIndex(c => new { c.CountryName }).IsUnique();
        }

        #region ***************************** Master Context ***************************
            public DbSet<tblBankMaster> tblBankMaster { get; set; }
            public DbSet<tblCurrency> tblCurrency { get; set; }
            public DbSet<tblTaxMaster> tblTaxMaster { get; set; }
            public DbSet<tblCompanyMaster> tblCompanyMaster { get; set; }
            public DbSet<tblUserIdentity> tblUserIdentity { get; set; }
            public DbSet<tblBookingIdentity> tblBookingIdentity { get; set; }
            public DbSet<tblRefundIdentity> tblRefundIdentity { get; set; }
            public DbSet<tblInvoiceIdentity> tblInvoiceIdentity { get; set; }
            public DbSet<tblPaymentRequestIdentity> tblPaymentRequestIdentity { get; set; }
        #endregion

        #region ****************************  User Context ********************************
        public DbSet<tblUsers> tblUsers { get; set; }
        public DbSet<tblCustomer> tblCustomer { get; set; }
        public DbSet<tblUserOTPValidation> tblUserOTPValidation { get; set; }
        public DbSet<tblDistributorMaster> tblDistributorMaster { get; set; }
        public DbSet<tblDistributorKycStatus> tblDistributorKycStatus { get; set; }
        public DbSet<tblDistributorTree> tblDistributorTree { get; set; }
        public DbSet<tblDistributorAddress> tblDistributorAddress { get; set; }
        public DbSet<tblDistributorCurrency> tblDistributorCurrency { get; set; }
        public DbSet<tblDistributorCultureInfo> tblDistributorCultureInfo { get; set; }
        public DbSet<tblDistributorBanks> tblDistributorBanks { get; set; }
        public DbSet<tblPan> tblPan { get; set; }
        public DbSet<tblIdentityProof> tblIdentityProof { get; set; }

        #endregion

        #region ********************************* Roles ***********************
        public DbSet<tblRoles> tblRoles { get; set; }
        public DbSet<tblRoleClaim> tblRoleClaim { get; set; }
        public DbSet<tblUserRole> tblUserRole { get; set; }
        public DbSet<tblUserClaim> tblUserClaim { get; set; }
        #endregion

        #region ********************************* Notification ***********************
        public DbSet<tblNotificationContent> tblNotificationContent { get; set; }
        public DbSet<tblNotificationMessage> tblNotificationMessage { get; set; }        
        #endregion


        #region ******************************* Customer Setting ***************************
        public DbSet<tblCustomerIPFilter> tblCustomerIPFilter { get; set; }
        public DbSet<tblCustomerIPFilterDetails> tblCustomerIPFilterDetails { get; set; }
        public DbSet<tblCustomerMarkup> tblCustomerMarkup { get; set; }
        public DbSet<tblWalletBalanceAlert> tblWalletBalanceAlert { get; set; }
        public DbSet<tblCustomerWalletAmount> tblCustomerWalletAmount { get; set; }
        public DbSet<tblWalletDetailLedger> tblWalletDetailLedger { get; set; }
        public DbSet<tblPaymentRequest> tblPaymentRequest { get; set; }
        public DbSet<tblCustomerNotification> tblCustomerNotification { get; set; }
        public DbSet<tblCustomerFlightClassOfBooking> tblCustomerFlightClassOfBooking { get; set; }
        public DbSet<tblCustomerFlightClassOfBookingDetails> tblCustomerFlightClassOfBookingDetails { get; set; }
        #endregion


        #region ********************************** Settings  ********************************
        public DbSet<tblFlightSerivceProvider> tblFlightSerivceProvider { get; set; }
        public DbSet<tblSchduleDisableServiceProvider> tblSchduleDisableServiceProvider { get; set; }
        public DbSet<tblPaymentGatewayProvider> tblPaymentGatewayProvider { get; set; }
        public DbSet<tblSchduleDisablePaymentGatewayProvider> tblSchduleDisablePaymentGatewayProvider { get; set; }
        #endregion


        #region ************************************ MLM ************************************
        public DbSet<tblProcessMaster> tblProcessMaster { get; set; }
        public DbSet<tblProcessExecution> tblProcessExecution { get; set; }
        public DbSet<tblProcessDependency> tblProcessDependency { get; set; }
        public DbSet<tblSaleSummary> tblSaleSummary { get; set; }
        public DbSet<tblSaleSummaryDayDetails> tblSaleSummaryDayDetails { get; set; }
        public DbSet<tblFixedRankMaster> tblFixedRankMaster { get; set; }
        public DbSet<tblFixedRank> tblFixedRank { get; set; }
        public DbSet<tblFixedRankDetails> tblFixedRankDetails { get; set; }
        public DbSet<tblClubQualifer> tblClubQualifer { get; set; }
        public DbSet<tblIncentiveMaster> tblIncentiveMaster { get; set; }
        public DbSet<tblIncentiveDetails> tblIncentiveDetails { get; set; }
        public DbSet<tblRptSummary> tblRptSummary { get; set; }
        public DbSet<tblEnableIncenitve> tblEnableIncenitve { get; set; }
        public DbSet<tblDispatchList> tblDispatchList { get; set; }
        public DbSet<tblTdsDeduction> tblTdsDeduction { get; set; }
        public DbSet<tblFutureHoldIncentive> tblFutureHoldIncentive { get; set; }
        public DbSet<tblHoldIncentive> tblHoldIncentive { get; set; }
        public DbSet<tblReleaseIncentive> tblReleaseIncentive { get; set; }

        #endregion


        #region *********************************** Air Services *****************************
        public DbSet<tblAirline> tblAirline { get; set; }
        public DbSet<tblFlightClassOfBooking> tblFlightClassOfBooking { get; set; }
        public DbSet<tblAirport> tblAirport { get; set; }
        public DbSet<tblFlightMarkupMaster> tblFlightMarkupMaster { get; set; }
        public DbSet<tblFlightMarkupServiceProvider> tblFlightMarkupServiceProvider { get; set; }
        public DbSet<tblFlightMarkupCustomerType> tblFlightMarkupCustomerType { get; set; }
        public DbSet<tblFlightMarkupCustomerDetails> tblFlightMarkupCustomerDetails { get; set; }
        public DbSet<tblFlightMarkupPassengerType> tblFlightMarkupPassengerType { get; set; }
        public DbSet<tblFlightMarkupFlightClass> tblFlightMarkupFlightClass { get; set; }
        public DbSet<tblFlightMarkupAirline> tblFlightMarkupAirline { get; set; }
        public DbSet<tblFlightConvenience> tblFlightConvenience { get; set; }
        public DbSet<tblFlightConvenienceServiceProvider> tblFlightConvenienceServiceProvider { get; set; }
        public DbSet<tblFlightConvenienceCustomerType> tblFlightConvenienceCustomerType { get; set; }
        public DbSet<tblFlightConvenienceCustomerDetails> tblFlightConvenienceCustomerDetails { get; set; }
        public DbSet<tblFlightConveniencePassengerType> tblFlightConveniencePassengerType { get; set; }
        public DbSet<tblFlightConvenienceFlightClass> tblFlightConvenienceFlightClass { get; set; }
        public DbSet<tblFlightConvenienceAirline> tblFlightConvenienceAirline { get; set; }
        public DbSet<tblFlightDiscount> tblFlightDiscount { get; set; }
        public DbSet<tblFlightDiscountServiceProvider> tblFlightDiscountServiceProvider { get; set; }
        public DbSet<tblFlightDiscountCustomerType> tblFlightDiscountCustomerType { get; set; }
        public DbSet<tblFlightDiscountCustomerDetails> tblFlightDiscountCustomerDetails { get; set; }
        public DbSet<tblFlightDiscountPassengerType> tblFlightDiscountPassengerType { get; set; }
        public DbSet<tblFlightDiscountFlightClass> tblFlightDiscountFlightClass { get; set; }
        public DbSet<tblFlightDiscountAirline> tblFlightDiscountAirline { get; set; }

        #region ************************* Caching **************************
        public DbSet<tblFlightSearchRequest_Caching> tblFlightSearchRequest_Caching { get; set; }
        public DbSet<tblFlightSearchResponse_Caching> tblFlightSearchResponse_Caching { get; set; }
        public DbSet<tblFlightSearchResponses_Caching> tblFlightSearchResponses_Caching { get; set; }
        public DbSet<tblFlightSearchSegment_Caching> tblFlightSearchSegment_Caching { get; set; }
        public DbSet<tblFlightFare_Caching> tblFlightFare_Caching { get; set; }
        public DbSet<tblFlightFareDetail_Caching> tblFlightFareDetail_Caching { get; set; }
        #endregion

        #region ************************* Flight Booking **************************
        public DbSet<tblFlightBookingMaster> tblFlightBookingMaster { get; set; }
        public DbSet<tblFlilghtBookingSearchDetails> tblFlilghtBookingSearchDetails { get; set; }
        public DbSet<tblFlightRefundStatusDetails> tblFlightRefundStatusDetails { get; set; }
        public DbSet<tblFlightPurchaseDetails> tblFlightPurchaseDetails { get; set; }
        public DbSet<tblFlighBookingPassengerDetails> tblFlighBookingPassengerDetails { get; set; }
        public DbSet<tblFlighRefundPassengerDetails> tblFlighRefundPassengerDetails { get; set; }
        #endregion
    #endregion




    }

}
