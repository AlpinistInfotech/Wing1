using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Services.Air;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2BClasses.Services.Enums;
namespace B2bApplication.Models
{
    public class mdlFlightSearch
    {
        public mdlSearchRequest searchRequest { get; set; }
        public List<mdlSearchResponse> searchResponse { get; set; }
        public List<tblAirport> Airports { get; set; }
        public async Task LoadAirportAsync(IBooking booking)
        {
            this.Airports = (await booking.GetAirportAsync());
        }
        public async Task LoadDefaultSearchRequestAsync(IBooking booking)
        {
            if (searchRequest == null)
            {
                searchRequest = new mdlSearchRequest();
            }
            if (searchRequest.Segments == null)
            {
                searchRequest.Segments = new List<mdlSegmentRequest>();
            }
            searchRequest.Segments.Add(new mdlSegmentRequest()
            {
                Destination = "BOM",
                Origin = "DEL",
                FlightCabinClass = enmCabinClass.ECONOMY,
                TravelDt=DateTime.Now.AddDays(7),
                PreferredArrival = enmPreferredDepartureTime.AnyTime,
                PreferredDeparture= enmPreferredDepartureTime.AnyTime
            });
        }
    }
}
