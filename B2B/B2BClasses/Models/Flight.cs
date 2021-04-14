using B2BClasses.Database;
using B2BClasses.Services.Air;
using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace B2BClasses.Models
{
    public class mdlFlightSearchWraper
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
        public mdlFlightSearchWraper FlightSearchWraper { get; set; } = new mdlFlightSearchWraper();
        public mdlSearchRequest searchRequest { get; set; }
        public mdlSearchResponse searchResponse { get; set; }
        public List<tblAirport> Airports { get; set; }
        public async Task LoadAirportAsync(IBooking booking)
        {
            this.Airports = (await booking.GetAirportAsync());
        }
        public void LoadDefaultSearchRequestAsync(IBooking booking)
        {
            if (searchRequest == null)
            {
                searchRequest = new mdlSearchRequest();
            }
            if (searchRequest.Segments == null)
            {
                searchRequest.Segments = new List<mdlSegmentRequest>();
            }
            if (FlightSearchWraper != null)
            {
                searchRequest.AdultCount = FlightSearchWraper.AdultCount;
                searchRequest.ChildCount = FlightSearchWraper.ChildCount;
                searchRequest.InfantCount = FlightSearchWraper.InfantCount;
                searchRequest.DirectFlight = true;
                searchRequest.JourneyType = FlightSearchWraper.JourneyType;
                searchRequest.Segments.Add(new mdlSegmentRequest()
                {
                    Destination = FlightSearchWraper.To,
                    Origin = FlightSearchWraper.From,
                    FlightCabinClass = FlightSearchWraper.CabinClass,
                    TravelDt = FlightSearchWraper.DepartureDt,
                    PreferredArrival = enmPreferredDepartureTime.AnyTime,
                    PreferredDeparture = enmPreferredDepartureTime.AnyTime
                });
                if (FlightSearchWraper.JourneyType == enmJourneyType.Return)
                {
                    searchRequest.Segments.Add(new mdlSegmentRequest()
                    {
                        Destination = FlightSearchWraper.From,
                        Origin = FlightSearchWraper.To,
                        FlightCabinClass = FlightSearchWraper.CabinClass,
                        TravelDt = FlightSearchWraper.ReturnDt ?? FlightSearchWraper.DepartureDt,
                        PreferredArrival = enmPreferredDepartureTime.AnyTime,
                        PreferredDeparture = enmPreferredDepartureTime.AnyTime
                    });

                }


            }


        }
    }
}
