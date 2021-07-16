using B2BClasses.Database;
using B2BClasses.Services.Air;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BApis.Model
{
    public class mdlHttpStatus
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }

    public class mdlBookingSearchApi : mdlHttpStatus
    {
        public IEnumerable<mdlSearchResponse> mdlSearches { get; set; }
    }

    public class mdlAirportApi : mdlHttpStatus
    {
        public string DefaultFromAirport { get; set; }
        public string DefaultToAirport { get; set; }
        public List<tblAirport> mdlSearches { get; set; }
    }


    public class mdlfarequoteApi : mdlHttpStatus
    {   
        public List<mdlFareQuotResponse> mdlQuote { get; set; }
    }


    public class mdlfareruleApi : mdlHttpStatus
    {   
        public List<mdlFareRuleResponse> mdlFare { get; set; }
    }

    public class mdlbooking : mdlHttpStatus
    {   
        public mdlBookingResponse mdlBooking { get; set; }
    }
}
