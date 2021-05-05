
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
            DefaultData defaultData = new DefaultData(modelBuilder);

            modelBuilder.Entity<tblCustomerMaster>(entity =>entity.HasCheckConstraint("CK_tblCustomerMaster_WalletBalence", "WalletBalence >= 0"));

            defaultData.InsertCustomerMaster();
            defaultData.InsertUser();
            defaultData.InsertAirport();
            defaultData.InsertAirline();
            defaultData.InsertServiceProvider();
        }

        public DbSet<tblAirline> tblAirline { get; set; }
        public DbSet<tblAirport> tblAirport { get; set; }


        public DbSet<tblTboTokenDetails> tblTboTokenDetails { get; set; }
        public DbSet<tblTboTravelDetail> tblTboTravelDetail { get;set;}
        public DbSet<tblTboTravelDetailResult> tblTboTravelDetailResult { get; set; }
        public DbSet<tblTboFareRule> tblTboFareRule { get; set; }
        
        public DbSet<tblTripJackTravelDetail> tblTripJackTravelDetail { get; set; }
        public DbSet<tblTripJackTravelDetailResult> tblTripJackTravelDetailResult { get; set; }
        public DbSet<tblTripJackFareRule> tblTripJackFareRule { get; set; }
        public DbSet<tblAirlineFareRule> tblAirlineFareRule { get; set; }


        #region*************** Masters ***************************
        public DbSet<tblMinBalenceAlert> tblMinBalenceAlert { get; set; }
        public DbSet<tblMinBalenceAlertDetails> tblMinBalenceAlertDetails { get; set; }
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
        #endregion

        #region ******************* Customer Master ***************************
        public DbSet<tblCustomerMaster> tblCustomerMaster { get; set; }
        public DbSet<tblUserMaster> tblUserMaster { get; set; }
        public DbSet<tblUserRole> tblUserRole { get; set; }
        public DbSet<tblCustomerIPFilter> tblCustomerIPFilter { get; set; }
        public DbSet<tblCustomerIPFilterDetails> tblCustomerIPFilterDetails { get; set; }
        public DbSet<tblWalletDetailLedger> tblWalletDetailLedger { get; set; }
        public DbSet<tblWalletDetailLedgerLog> tblWalletDetailLedgerLog { get; set; }
        public DbSet<tblWalletBalanceAlert> tblWalletBalanceAlert { get; set; }
        public DbSet<tblCustomerMarkup> tblCustomerMarkup { get; set; }
        #endregion

    }


    

}
