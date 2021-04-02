using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WingApi.Classes.Database
{
    public class tblTripJackTravelDetail : tblTravelMaster
    {
        [InverseProperty("tblTripJackTravelDetail")]
        public ICollection<tblTripJackTravelDetailResult> tblTripJackTravelDetailResult { get; set; }
    }

    public class tblTripJackTravelDetailResult:tblTravelDetail
    {
        [ForeignKey("tblTripJackTravelDetail")] // Foreign Key here
        public int? TravelDetailId { get; set; }
        public tblTripJackTravelDetail tblTripJackTravelDetail { get; set; }
    }
    public class tblTripJackFareRule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TraceId { get; set; }
        public string ResultIndex { get; set; }
        public string JsonData { get; set; }
        public DateTime GenrationDt { get; set; }
    }
}


