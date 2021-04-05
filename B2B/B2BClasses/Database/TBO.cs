using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace B2BClasses.Database
{
    public class tblTboTokenDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TokenId { get; set; }
        public string MemberId { get; set; }
        public string AgencyId { get; set; }
        public DateTime GenrationDt { get; set; }
    }

    public class tblTboTravelDetail: tblTravelMaster
    {
        
        [InverseProperty("tblTboTravelDetail")]
        public ICollection<tblTboTravelDetailResult> tblTboTravelDetailResult { get; set; }
    }

    

    public class tblTboTravelDetailResult: tblTravelDetail
    {   
        [ForeignKey("tblTboTravelDetail")] // Foreign Key here
        public int? TravelDetailId { get; set; }
        public tblTboTravelDetail tblTboTravelDetail { get; set; }
    }
    public class tblTboFareRule
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
