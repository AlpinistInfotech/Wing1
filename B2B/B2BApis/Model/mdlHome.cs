using B2BClasses.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BApis.Model
{
    public class mdlBookingSearchApi: B2BClasses.Database.tblFlightBookingMaster
    {
        public string TokenID { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }
}
