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
        }
        #region ******************** Masters Db ***********************
        public DbSet<tblCaptcha> tblCaptcha { get;set;}
        public DbSet<tblCountryMaster> tblCountryMaster { get; set; }
        public DbSet<tblStateMaster> tblStateMaster { get; set; }
        #endregion

        #region ********************** Consolidator Profile*************************
        public DbSet<tblRegistration> tblRegistration { get; set; }
        public DbSet<tblTree> tblTree { get; set; }
        public DbSet<tblTcRanksDetails> tblTcRanksDetails { get; set; }
        public DbSet<tblTcAddressDetail> tblTcAddressDetail { get; set; }
        
        #endregion
    }
}
