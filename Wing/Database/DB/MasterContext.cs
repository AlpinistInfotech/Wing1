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

        #region ********************************* Master Context ***************************
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

        #region ******************************** 

        #endregion

    }

}
