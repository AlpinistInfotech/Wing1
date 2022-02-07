using B2C.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2C.Models
{
    public class mdlFlightSearchRequest
    {
        public string From { get; set; } = "DEL";
        public string To{ get; set; } = "BOM";
        public DateTime DepartureDt { get; set; }
        public DateTime ReturnDt { get; set; }
        public enmJourneyType JourneyType { get; set; } = enmJourneyType.OneWay;
        public enmCabinClass CabinClass { get; set; } = enmCabinClass.ECONOMY;
        public int AdultCount { get; set; } = 1;
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }
        public bool DirectFlight { get; set; }
    }

    #region *************flight Search Response *********************
    public class mdlError
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
    public class mdlSearchResponseWraper
    {
        public enmMessageType messageType { get; set; }
        public string  message{ get; set; }
        public mdlSearchResponse returnId { get; set; }

    }

    public class mdlSearchResponse
    {
        public enmMessageType ResponseStatus { get; set; }
        public mdlError Error { get; set; }
        public string From { get; set; } = "DEL";
        public string To { get; set; } = "BOM";
        public DateTime DepartureDt { get; set; }
        public DateTime ReturnDt { get; set; }
        public enmJourneyType JourneyType { get; set; } = enmJourneyType.OneWay;
        public enmCabinClass CabinClass { get; set; } = enmCabinClass.ECONOMY;
        public int AdultCount { get; set; } = 1;
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }
        public List<List<mdlSearchResult>> Results { get; set; }
    }

    public class mdlSearchResult
    {
        public enmServiceProvider ServiceProvider { get; set; }
        public string TraceId { get; set; }
        public List<mdlSegment> Segment { get; set; }
        public List<mdlTotalpricelist> TotalPriceList { get; set; }
    }

    public class mdlSegment
    {
        public string Id { get; set; }
        public mdlAirline Airline { get; set; }
        public mdlAirport Origin { get; set; }
        public mdlAirport Destination { get; set; }
        public int TripIndicator { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int Stops { get; set; }
        public List<mdlAirport> StopDetail { get; set; }
        public int Mile { get; set; }
        public int Duration { get; set; }
        public int Layover { get; set; }
        public mdlSsrInfo sinfo { get; set; }
    }

    public class mdlSsrInfo
    {
        public SsrInformation[] SEAT { get; set; }
        public SsrInformation[] BAGGAGE { get; set; }
        public SsrInformation[] MEAL { get; set; }
        public SsrInformation[] EXTRASERVICES { get; set; }
    }
    public class SsrInformation
    {
        public string code { get; set; }
        public double amount { get; set; }
        public string desc { get; set; }
    }

    public class mdlAirline
    {
        public bool isLcc { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FlightNumber { get; set; }
        public string OperatingCarrier { get; set; }
    }

    public class mdlAirportWraper
    { 
        public enmMessageType messageType { get; set; }
        public string message{ get; set; }
        public List<mdlAirport> returnId { get; set; }
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
        public double BaseFare { get; set; }
        public double CustomerMarkup { get; set; }
        public double WingMarkup { get; set; }
        public double MLMMarkup { get; set; }
        public double Convenience { get; set; }
        public double TotalFare { get; set; }
        public double Discount { get; set; }
        public double NetFare { get; set; }
        public mdlTotalpriceDetail ADULT { get; set; }
        public mdlTotalpriceDetail CHILD { get; set; }
        public mdlTotalpriceDetail INFANT { get; set; }
        //public mdlFareRuleResponse FareRule { get; set; }
        public string ResultIndex { get; set; }
        public string sri { get; set; }
        public List<string> msri { get; set; }
        public int SeatRemaning { get; set; }
        public string Identifier { get; set; }//Corepreate, Publish, SME
        public enmCabinClass CabinClass { get; set; }
        public string ClassOfBooking { get; set; }
        public string AlterResultIndex { get; set; }
        public string alterIdentifier { get; set; }//Corepreate, Publish, SME        
        public string ProviderBookingId { get; set; }
        public bool IncludeBaggageServices { get; set; }
        public bool IncludeMealServices { get; set; }
        public bool IncludeSeatServices { get; set; }
        public double PurchaseAmount { get; set; }//Price at Which wing Purchase the ticket
        public double PassengerIncentiveAmount { get; set; }
        public double PassengerMarkupAmount { get; set; }
        public double PassengerDiscountAmount { get; set; }
        public double PassengerConvenienceAmount { get; set; }
        public double SaleAmount { get; set; }

    }

    public class mdlTotalpriceDetail
    {
        public double YQTax { get; set; }
        public double BaseFare { get; set; }
        public double Tax { get; set; }        
        public double WingMarkup { get; set; }
        public double MLMMarkup { get; set; }
        public double TotalFare { get; set; }
        public double Discount { get; set; }
        public double NetFare { get; set; }
        public double Convenience { get; set; }
        public string CheckingBaggage { get; set; }
        public string CabinBaggage { get; set; }
        public bool IsFreeMeal { get; set; }
        public byte IsRefundable { get; set; }
        
    }

    public class mdlWingFaredetails
    {
        public int ID { get; set; }
        public enmFlighWingCharge type { get; set; }
        public double amount { get; set; }
        public enmGender OnGender { get; set; }
    }
    #endregion

}
