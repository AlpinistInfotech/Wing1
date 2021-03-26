using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WingApi.Classes.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }
        public DbSet<tblTboTokenDetails> tblTboTokenDetails { get; set; }
        public DbSet<tblTboTravelDetail> tblTboTravelDetail { get;set;}
        public DbSet<tblTboTravelDetailResult> tblTboTravelDetailResult { get; set; }
        public DbSet<tblTboFareRule> tblTboFareRule { get; set; }
        
        public DbSet<tblTripJackTravelDetail> tblTripJackTravelDetail { get; set; }
        public DbSet<tblTripJackTravelDetailResult> tblTripJackTravelDetailResult { get; set; }
        public DbSet<tblTripJackFareRule> tblTripJackFareRule { get; set; }
        
    }
}
