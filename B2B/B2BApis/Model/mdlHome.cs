using B2BClasses.Database;
using B2BClasses.Services.Air;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BApis.Model
{
    public class mdlBookingSearchApi 
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public IEnumerable<mdlSearchResponse> mdlSearches { get; set; }
    }

    public class mdlAirportApi
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string DefaultFromAirport { get; set; }
        public string DefaultToAirport { get; set; }

        public List<tblAirport> mdlSearches { get; set; }
    }


    public class mdlfarequoteApi
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<mdlFareQuotResponse> mdlQuote { get; set; }
    }


    public class mdlfareruleApi
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<mdlFareRuleResponse> mdlFare { get; set; }
    }



    public class mdlbooking
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public mdlBookingResponse mdlBooking { get; set; }
    }
}
