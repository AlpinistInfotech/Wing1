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
    public class FlightSearchGUI
    {
        public string From { get; set; } = "DEL";
        public string To { get; set; } = "BOM";
        public enmJourneyType JourneyType { get; set; } = enmJourneyType.OneWay;
        public enmCabinClass CabinClass { get; set; } = enmCabinClass.ECONOMY;
        public DateTime DepartureDt { get; set; } = DateTime.Now.AddDays(7);
        public DateTime? ReturnDt { get; set; } = DateTime.Now.AddDays(14);
        public int AdultCount { get; set; } = 1;
        public int ChildCount { get; set; } = 0;
        public int InfantCount { get; set; } = 0;
    }

    public class mdlFlightSearch
    {
        public FlightSearchGUI flightSearchGUI { get; set; } =new FlightSearchGUI();
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
            if (flightSearchGUI != null)
            {
                searchRequest.AdultCount = flightSearchGUI.AdultCount;
                searchRequest.ChildCount = flightSearchGUI.ChildCount;
                searchRequest.InfantCount = flightSearchGUI.InfantCount;
                searchRequest.DirectFlight = true;
                searchRequest.JourneyType = flightSearchGUI.JourneyType;
                searchRequest.Segments.Add(new mdlSegmentRequest()
                {
                    Destination = flightSearchGUI.To,
                    Origin = flightSearchGUI.From,
                    FlightCabinClass = flightSearchGUI.CabinClass,
                    TravelDt = flightSearchGUI.DepartureDt,
                    PreferredArrival = enmPreferredDepartureTime.AnyTime,
                    PreferredDeparture = enmPreferredDepartureTime.AnyTime
                });
                if (flightSearchGUI.JourneyType == enmJourneyType.Return)
                {
                    searchRequest.Segments.Add(new mdlSegmentRequest()
                    {
                        Destination = flightSearchGUI.From,
                        Origin = flightSearchGUI.To,
                        FlightCabinClass = flightSearchGUI.CabinClass,
                        TravelDt = flightSearchGUI.ReturnDt?? flightSearchGUI.DepartureDt,
                        PreferredArrival = enmPreferredDepartureTime.AnyTime,
                        PreferredDeparture = enmPreferredDepartureTime.AnyTime
                    });

                }

                
            }

            
        }
    }
}
