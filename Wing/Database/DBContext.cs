using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database
{
    public class DBContext : IdentityDbContext<ApplicationUser>
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<tblTcSequcence>().HasIndex(p => new { p.Monthyear, p.StateId }).IsUnique();
            modelBuilder.Entity<tblTcSequcence>().Property(p => p.RowVersion).IsConcurrencyToken();
            modelBuilder.Entity<tblCountryMaster>().HasIndex(c => new { c.CountryName }).IsUnique();
            modelBuilder.Entity<tblStateMaster>().HasIndex(c => new { c.CountryId, c.StateName }).IsUnique();
            modelBuilder.Entity<tblStateMaster>().Property(o => o.longitude).HasColumnType("decimal(18,14)");
            modelBuilder.Entity<tblStateMaster>().Property(o => o.latitude).HasColumnType("decimal(18,14)");


            ///modelBuilder.Query //<SpGetProductByID>();
        }

        
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        #region ******************** Masters Db ***********************
        public DbSet<tblCaptcha> tblCaptcha { get;set;}
        public DbSet<tblCountryMaster> tblCountryMaster { get; set; }
        public DbSet<tblStateMaster> tblStateMaster { get; set; }
        public DbSet<tblTcSequcence> tblTcSequcence { get; set; }
        public DbSet<tblBankMaster> tblBankMaster { get; set; }
        public DbSet<tblEmpMaster> tblEmpMaster { get; set; }
        


        #endregion

        #region ********************** Consolidator Profile*************************
        public DbSet<tblRegistration> tblRegistration { get; set; }
        public DbSet<tblTCStatus> tblTCStatus { get; set; }

        public DbSet<tblTree> tblTree { get; set; }
        public DbSet<tblTcRanksDetails> tblTcRanksDetails { get; set; }
        public DbSet<tblTcAddressDetail> tblTcAddressDetail { get; set; }
        public DbSet<tblKycMaster> tblKycMaster { get; set; }
        public DbSet<tblTcBankDetails> tblTcBankDetails { get; set; }
        public DbSet<tblTcPanDetails> TblTcPanDetails { get; set; }

        public DbSet<tblTcNominee> TblTcNominee { get; set; }

        public DbSet<tblTcContact> TblTcContact { get; set; }

        public DbSet<tblTcEmail> TblTcEmail { get; set; }

        public DbSet<tblTcMarkUp> TblTcMarkUp { get; set; }
        public DbSet<tblHolidayPackageMaster> tblHolidayPackageMaster { get; set; }

        public DbSet<tblTCWallet> tblTCwallet { get; set; }
        public DbSet<tblTCWalletLog> tblTCwalletlog { get; set; }
        public DbSet<tblTCMemberRank> tblTCmemberrank { get; set; }
        public DbSet<tblTCMemberRankLog> tblTCmemberranklog { get; set; }
        public DbSet<tblTCInvoice> tblTCinvoice { get; set; }
        public DbSet<tblTCStatement> tblTCstatement { get; set; }
        public DbSet<tblTCStatementLog> tblTCstatementlog { get; set; }
        public DbSet<tblTCSaleSummary> tblTCsalesummary { get; set; }

        #endregion

        public void SetBasicData(ModelBuilder modelBuilder)
        {
            
        }
    }
}
