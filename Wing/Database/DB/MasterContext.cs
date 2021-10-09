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






    }

}
