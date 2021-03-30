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


    public class tblAirlineFareRule
    {
        public int AirlineFareRuleId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public double ReissueCharge { get; set; }
        public double ReissueAdditionalCharge { get; set; }
        public string ReissuePolicy { get; set; }
        public double CancellationCharge { get; set; }
        public double CancellationAdditionalCharge { get; set; }
        public string CancellationPolicy { get; set; }
        public string CheckingBaggage { get; set; }
        public string CabinBaggage { get; set; }
    }

    public class tblAirlineFare
    {
        public int AirlineFareId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime TravelDate { get; set; }        
        public DateTime GenrationDt { get; set; }
        public DateTime ExpireDt { get; set; }
        public double PublishedFare { get; set; }
    }
    
    public class tblAirlineFareDetails
    {
        public int AirlineFareDetailId { get; set; }
        public bool isLcc { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string FlightNumber { get; set; }
        public string OperatingCarrier { get; set; }
        public enmCabinClass CabinClass { get; set; }
    }

}
