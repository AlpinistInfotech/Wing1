using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Services.Air;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2bApplication.Models
{
    public class mdlFlightSearch
    {
        public mdlSearchRequest searchRequest { get; set; }
        public mdlSearchResponse searchResponse { get; set; }
        public List<tblAirport> Airports { get; set; }
        public async Task LoadAirportAsync(IBooking booking)
        {
            this.Airports = (await booking.GetAirportAsync());
        }
    }
}
