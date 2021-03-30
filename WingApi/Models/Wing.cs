using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WingApi.Models
{
    public class mdlSearchRequest
    {
        [Required]
        public int AdultCount { get; set; } = 1;
        public int ChildCount { get; set; } = 0;
        public int InfantCount { get; set; } = 0;
        public bool DirectFlight { get; set; }        
        public Classes.enmJourneyType JourneyType { get; set; }
        public string[] PreferredAirlines { get; set; }
        [Required]
        public mdlSegmentRequest[] Segments { get; set; }        
    }

    public class mdlSegmentRequest
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public Classes.enmCabinClass FlightCabinClass { get; set; }
        public DateTime TravelDt { get; set; }
        public Classes.enmPreferredDepartureTime PreferredDeparture { get; set; }
        public Classes.enmPreferredDepartureTime PreferredArrival { get; set; }
    }

    public class mdlError
    {
        public int Code { get; set; }        
        public string Message { get; set; }
    }

    public class mdlSearchResponse
    {
        public int ResponseStatus { get; set; }
        public mdlError Error { get; set; }
        public Classes.enmServiceProvider ServiceProvider { get; set; }
        public string TraceId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public mdlSearchResult[][] Results { get; set; }
    }

    public class mdlSearchResult
    {
        public mdlSegment[] TripInfos { get; set; }
        public mdlTotalpricelist []TotalPriceList { get; set; }
    }

    public class mdlSegment
    {
        public mdlAirline Airline { get; set; }
        public mdlAirport Origin { get; set; }
        public mdlAirport Destination { get; set; }
        
        public int TripIndicator { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int Mile { get; set; }
        public int Duration { get; set; }
    }
    public class mdlAirline
    {
        public bool isLcc { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FlightNumber { get; set; }        
        public string OperatingCarrier { get; set; }
    }

    public class mdlAirport
    {
        public string AirportCode { get; set; }
        public string AirportName { get; set; }
        public string Terminal { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
    }
    public class mdlTotalpricelist
    {
        public string fareIdentifier { get; set; }
        public mdlPassenger ADULT { get; set; }
        public mdlPassenger CHILD { get; set; }
        public mdlPassenger INFANT { get; set; }
        public string ResultIndex { get; set; }
        public string sri { get; set; }
        public string [] msri { get; set; }
        
    }

    public class mdlPassenger
    {
        public mdlFareComponent FareComponent { get; set; }
        public mdlAdditionalFareComponent AdditionalFareComponent { get; set; }
        public int SeatRemaing { get; set; }
        public mdlBaggageInformation BaggageInformation { get; set; }
        public int RefundableType { get; set; }//0 Non Refundable,1 - Refundable,2 - Partial Refundable
        public Classes.enmCabinClass CabinClass { get; set; }//ECONOMY,PREMIUM_ECONOMY, BUSINESS,FIRST
        public string ClassOfBooking{ get; set; }
        public string FareBasis { get; set; }
        public bool IsFreeMeel { get; set; }
    }

    public class mdlBaggageInformation
    { 
        public string CheckingBaggage { get; set; }
        public string CabinBaggage { get; set; }
    }

    public class mdlFareComponent
    {
        public double TaxAndFees { get; set; }
        public double NetFare { get; set; }
        public double BaseFare { get; set; }
        public double TotalFare { get; set; }
        public double IGST { get; set; }
        public double NetCommission { get; set; }
        public double SSRP { get; set; }
    }

    public class mdlSSRP
    {
        public double SeatPrice { get; set; }
        public double MealPrice { get; set; }
        public double BaggagePrice { get; set; }
    }

    public class mdlAdditionalFareComponent
    { 
        public string Key { get; set; }
        public string Value { get; set; }
    }

}
