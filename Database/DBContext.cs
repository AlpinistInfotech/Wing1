using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

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
        }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        #region ******************** Masters Db ***********************
        public DbSet<tblCaptcha> tblCaptcha { get;set;}
        public DbSet<tblCountryMaster> tblCountryMaster { get; set; }
        public DbSet<tblStateMaster> tblStateMaster { get; set; }
        public DbSet<tblTcSequcence> tblTcSequcence { get; set; }
        public DbSet<tblBankMaster> tblBankMaster { get; set; }
        #endregion

        #region ********************** Consolidator Profile*************************
        public DbSet<tblRegistration> tblRegistration { get; set; }
        public DbSet<tblTree> tblTree { get; set; }
        public DbSet<tblTcRanksDetails> tblTcRanksDetails { get; set; }
        public DbSet<tblTcAddressDetail> tblTcAddressDetail { get; set; }
        public DbSet<tblKycMaster> tblKycMaster { get; set; }
        public DbSet<tblTcBankDetails> tblTcBankDetails { get; set; }
        #endregion

        public void SetBasicData(ModelBuilder modelBuilder)
        {
            
        }
    }
}
