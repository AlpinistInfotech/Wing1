using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WingApi.Classes.Database
{
    public class tblTboTravelDetail
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
        public int MinPrice { get; set; }
        public DateTime GenrationDt { get; set; }
        public DateTime ExpireDt { get; set; }
        public double MinPublishFare { get; set; }
        public double MinOfferedFare{ get; set; }
        public string currency { get; set; }
    }

    public class tblTboTravelDetailResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResultId { get; set; }
        [ForeignKey("tblTboTravelDetail")] // Foreign Key here
        public int? TravelDetailId { get; set; }
        public tblTboTravelDetail tblTboTravelDetail { get; set; }
        public string ResultIndex { get; set; }
        public double PublishedFare { get; set; }
        public double OfferedFare { get; set; }
        public string JsonData { get; set; }
    }
}
