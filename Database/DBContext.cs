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

        //#region Get product by ID store procedure method.  
        //SqlConnection sqlconnection;
        //SqlCommand sqlcommand;

        ///// <summary>  
        ///// Get product by ID store procedure method.  
        ///// </summary>  
        ///// <param name="productId">Product ID value parameter</param>  
        ///// <returns>Returns - List of product by ID</returns>  
        //public async Task<List<ProcRegistrtaionSearch>> GetRegistrtaionSearchAsync(DateTime fromdate,DateTime todate,string Tcid,int status,int sessionid,int spmode)
        //{
        //    // Initialization.  
        //    List<ProcRegistrtaionSearch> lst = new List<ProcRegistrtaionSearch>();

        //    try
        //    {
        //        sqlcommand = new SqlCommand();
        //        sqlcommand.Connection = sqlconnection;
        //        sqlcommand.CommandText = "proc_registration_search";
        //        // Settings.  
        //        SqlParameter sqlParameter_fromdate = new SqlParameter("@fromdate", fromdate);
        //        SqlParameter sqlParameter_todate = new SqlParameter("@todate", todate);
        //        SqlParameter sqlParameter_tcid = new SqlParameter("@tcid", Tcid);
        //        SqlParameter sqlParameter_status = new SqlParameter("@status", status);
        //        SqlParameter sqlParameter_sessionid = new SqlParameter("@sessionid", sessionid);
        //        SqlParameter sqlParameter_spmode = new SqlParameter("@spmode", spmode);





        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    // Info.  
        //    return lst;
        //}

        //#endregion


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
        public DbSet<tblTree> tblTree { get; set; }
        public DbSet<tblTcRanksDetails> tblTcRanksDetails { get; set; }
        public DbSet<tblTcAddressDetail> tblTcAddressDetail { get; set; }
        public DbSet<tblKycMaster> tblKycMaster { get; set; }
        public DbSet<tblTcBankDetails> tblTcBankDetails { get; set; }
        public DbSet<tblTcPanDetails> TblTcPanDetails { get; set; }

        public DbSet<tblTcNominee> TblTcNominee { get; set; }

        public DbSet<tblTcContact> TblTcContact { get; set; }

        public DbSet<tblTcEmail> TblTcEmail { get; set; }

        #endregion

        public void SetBasicData(ModelBuilder modelBuilder)
        {
            
        }
    }
}
