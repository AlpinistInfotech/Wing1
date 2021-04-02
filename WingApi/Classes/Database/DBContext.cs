using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        
        
        public DbSet<tblAirlineFareRule> tblAirlineFareRule { get; set; }


    }


    public class tblAirlineFareRule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AirlineFareRuleId { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
        public string traceId { get; set; }
        public string resultIndex { get; set; }
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

    public class tblTravelMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TravelDetailId { get; set; }
        public DateTime TravelDate { get; set; }
        public enmCabinClass CabinClass { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string TokenId { get; set; }
        public string TraceId { get; set; }
        public DateTime GenrationDt { get; set; }
        public DateTime ExpireDt { get; set; }
        public double MinPublishFare { get; set; }
        public double MinPublishFareReturn { get; set; }
        public enmJourneyType JourneyType { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }
    }
    public class tblTravelDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResultId { get; set; }
        public int segmentId { get; set; }
        public string ResultIndex { get; set; }
        public string ResultType { get; set; }
        public double PublishedFare { get; set; }
        public double OfferedFare { get; set; }
        public string JsonData { get; set; }
    }
    
}
