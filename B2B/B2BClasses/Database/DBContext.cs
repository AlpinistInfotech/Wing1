
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace B2BClasses.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblCountryMaster>().HasIndex(c => new { c.CountryName }).IsUnique();
            modelBuilder.Entity<tblStateMaster>().HasIndex(c => new { c.CountryId, c.StateName }).IsUnique();
            modelBuilder.Entity<tblStateMaster>().Property(o => o.longitude).HasColumnType("decimal(18,14)");
            modelBuilder.Entity<tblStateMaster>().Property(o => o.latitude).HasColumnType("decimal(18,14)");
            modelBuilder.Entity<tblPaymentRequest>().Property(p => p.Status).IsConcurrencyToken();

            DefaultData defaultData = new DefaultData(modelBuilder);
            modelBuilder.Entity<tblCustomerBalance>(entity =>entity.HasCheckConstraint("CK_tblCustomerMaster_WalletBalance", "WalletBalance >= 0"));
            modelBuilder.Entity<tblCustomerBalance>(entity => entity.HasCheckConstraint("CK_tblCustomerMaster_CreditBalance", "CreditBalance >= 0"));

            defaultData.InsertRoleClaim();
            defaultData.InsertCustomerMaster();
            defaultData.InsertUser();
            defaultData.InsertAirport();
            defaultData.InsertAirline();
            defaultData.InsertServiceProvider();
            defaultData.InsertBank();
            #region ************** Remove Default Identity Columns *******************
            modelBuilder.Entity<tblCustomerBalance>().Property(et => et.CustomerId).ValueGeneratedNever();
            modelBuilder.Entity<tblCustomerGSTDetails>().Property(et => et.CustomerId).ValueGeneratedNever();
            modelBuilder.Entity<tblCustomerIPFilter>().Property(et => et.CustomerId).ValueGeneratedNever();
            modelBuilder.Entity<tblCustomerBankDetails>().Property(et => et.CustomerId).ValueGeneratedNever();
            modelBuilder.Entity<tblCustomerPanDetails>().Property(et => et.CustomerId).ValueGeneratedNever();
            modelBuilder.Entity<tblCustomerMarkup>().Property(et => et.CustomerId).ValueGeneratedNever();
            modelBuilder.Entity<tblWalletBalanceAlert>().Property(et => et.CustomerId).ValueGeneratedNever();
            #endregion

        }




        public DbSet<tblTboTokenDetails> tblTboTokenDetails { get; set; }
        public DbSet<tblTboTravelDetail> tblTboTravelDetail { get;set;}
        public DbSet<tblTboTravelDetailResult> tblTboTravelDetailResult { get; set; }
        public DbSet<tblTboFareRule> tblTboFareRule { get; set; }
        
        public DbSet<tblTripJackTravelDetail> tblTripJackTravelDetail { get; set; }
        public DbSet<tblTripJackTravelDetailResult> tblTripJackTravelDetailResult { get; set; }
        public DbSet<tblTripJackFareRule> tblTripJackFareRule { get; set; }
        public DbSet<tblAirlineFareRule> tblAirlineFareRule { get; set; }


        #region ************************* Flight Booking **********************************
        public DbSet<tblFlightBookingMaster> tblFlightBookingMaster { get; set; }
        public DbSet<tblFlightBookingProviderTraceId> tblFlightBookingProviderTraceId{ get; set; }
        
        public DbSet<tblFlightBookingSegmentMaster> tblFlightBookingSegmentMaster { get; set; }
        public DbSet<tblFlightBookingSegment> tblFlightBookingSegment { get; set; }
        public DbSet<tblFlightBookingPassengerDetails> tblFlightBookingPassengerDetails { get; set; }
        public DbSet<tblFlightBookingGSTDetails> tblFlightBookingGSTDetails { get; set; }
        public DbSet<tblFlightBookingServices> tblFlightBookingServices { get; set; }
        public DbSet<tblFlightBookingFareDetails> tblFlightBookingFareDetails { get; set; }
        public DbSet<tblFlightBookingFarePurchaseDetails> tblFlightBookingFarePurchaseDetails { get; set; }
        public DbSet<tblFlightCancelation> tblFlightCancelation { get; set; }
        
        #endregion


        #region*************** Fare management ***************************
        public DbSet<tblMinBalanceAlert> tblMinBalanceAlert { get; set; }
        public DbSet<tblMinBalanceAlertDetails> tblMinBalanceAlertDetails { get; set; }
        public DbSet<tblActiveSerivceProvider> tblActiveSerivceProvider { get; set; }
        public DbSet<tblActiveSerivceProviderLog> tblActiveSerivceProviderLog { get; set; }

        public DbSet<tblWingMarkupMaster> tblWingMarkupMaster { get; set; }
        public DbSet<tblWingMarkupServiceProvider> tblWingMarkupServiceProvider { get; set; }
        public DbSet<tblWingMarkupCustomerType> tblWingMarkupCustomerType { get; set; }
        public DbSet<tblWingMarkupCustomerDetails> tblWingMarkupCustomerDetails { get; set; }
        public DbSet<tblWingMarkupPassengerType> tblWingMarkupPassengerType { get; set; }
        public DbSet<tblWingMarkupFlightClass> tblWingMarkupFlightClass { get; set; }
        public DbSet<tblWingMarkupAirline> tblWingMarkupAirline { get; set; }


        public DbSet<tblWingConvenience> tblWingConvenience { get; set; }
        public DbSet<tblWingConvenienceServiceProvider> tblWingConvenienceServiceProvider { get; set; }
        public DbSet<tblWingConvenienceCustomerType> tblWingConvenienceCustomerType { get; set; }
        public DbSet<tblWingConvenienceCustomerDetails> tblWingConvenienceCustomerDetails { get; set; }
        public DbSet<tblWingConveniencePassengerType> tblWingConveniencePassengerType { get; set; }
        public DbSet<tblWingConvenienceFlightClass> tblWingConvenienceFlightClass { get; set; }
        public DbSet<tblWingConvenienceAirline> tblWingConvenienceAirline { get; set; }

        public DbSet<tblWingDiscount> tblWingDiscount { get; set; }
        public DbSet<tblWingDiscountServiceProvider> tblWingDiscountServiceProvider { get; set; }
        public DbSet<tblWingDiscountCustomerType> tblWingDiscountCustomerType { get; set; }
        public DbSet<tblWingDiscountCustomerDetails> tblWingDiscountCustomerDetails { get; set; }
        public DbSet<tblWingDiscountPassengerType> tblWingDiscountPassengerType { get; set; }
        public DbSet<tblWingDiscountFlightClass> tblWingDiscountFlightClass { get; set; }
        public DbSet<tblWingDiscountAirline> tblWingDiscountAirline { get; set; }


        public DbSet<tblWingCustomerFlightAPI> tblWingCustomerFlightAPI { get; set; }
        public DbSet<tblWingCustomerFlightAPIServiceProvider> tblWingCustomerFlightAPIServiceProvider { get; set; }
        public DbSet<tblWingCustomerFlightAPICustomerType> tblWingCustomerFlightAPICustomerType { get; set; }
        public DbSet<tblWingCustomerFlightAPICustomerDetails> tblWingCustomerFlightAPICustomerDetails { get; set; }
    
        public DbSet<tblWingCustomerFlightAPIFlightClass> tblWingCustomerFlightAPIFlightClass { get; set; }
        public DbSet<tblWingCustomerFlightAPIAirline> tblWingCustomerFlightAPIAirline { get; set; }
        #endregion

        #region*************** Master ***************************
        public DbSet<tblBankMaster> tblBankMaster { get; set; }
        public DbSet<tblCountryMaster> tblCountryMaster { get; set; }
        public DbSet<tblStateMaster> tblStateMaster { get; set; }        
        public DbSet<tblAirline> tblAirline { get; set; }
        public DbSet<tblAirport> tblAirport { get; set; }

        #endregion

        #region ******************* Customer Master ***************************



        public DbSet<tblCustomerMaster> tblCustomerMaster { get; set; }
        public DbSet<tblCustomerBalance> tblCustomerBalance { get; set; }
        public DbSet<tblCustomerGSTDetails> tblCustomerGSTDetails { get; set; }
        public DbSet<tblUserMaster> tblUserMaster { get; set; }
        public DbSet<tblRoleMaster> tblRoleMaster { get; set; }
        public DbSet<tblUserRole> tblUserRole { get; set; }
        public DbSet<tblRoleClaim> tblRoleClaim { get; set; }
        public DbSet<tblCustomerIPFilter> tblCustomerIPFilter { get; set; }
        public DbSet<tblCustomerBankDetails> tblCustomerBankDetails { get; set; }
        public DbSet<tblCustomerPanDetails> tblCustomerPanDetails { get; set; }
        public DbSet<tblCustomerIPFilterDetails> tblCustomerIPFilterDetails { get; set; }
        public DbSet<tblWalletDetailLedger> tblWalletDetailLedger { get; set; }        
        public DbSet<tblWalletBalanceAlert> tblWalletBalanceAlert { get; set; }
        public DbSet<tblCustomerMarkup> tblCustomerMarkup { get; set; }        
        public DbSet<tblPaymentRequest> tblPaymentRequest { get; set; }
        #endregion


        #region *********************  Ticketing ****************************
        public DbSet<tblTicket> tblTicket { get; set; }
        public DbSet<tblTicketDetails> tblTicketDetails { get; set; }
        #endregion

    }




}

