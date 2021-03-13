using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WingApi.Classes
{
    public enum enmTaxandFeeType
    {
        OtherCharges,//OT
        ManagementFee,//MF
        ManagementFeeTax,//MFT
        AirLineGSTComponent,//AGST
        FuelSurcharge,//YQ
        CarrierMiscFee,//YR

    }

    public enum enmPassengerType
    {
        Adult =1,
        Child =2,
        Infant = 3,
    }

    public enum enmRefundableType
    {
        NonRefundable=0,
        Refundable=1,
        PartialRefundable=2
    }

    public enum enmCabinClass
    {
        //ALL=1,
        Economy=2,
        PremiumEconomy=3,
        Business=4,
        //PremiumBusiness=5,
        First=6
    }
    public enum enmPreferredDepartureTime
    {
        AnyTime=1,
        Morning=2,
        AfterNoon=3,
        Evening=4,
        Night=5
    }

    public class AuthenticateRequestModel
    {
        public string ClinetId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EndUserIp { get; set; }
    }

    public class ApiError
    { 
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class AuthenticateResponse
    {
        public string  UserName { get; set; }
        public ApiError Error { get; set; }
        public string TokenId { get; set; }
        public int Status { get; set; }
    }

    public class mdlRouteInfo
    { 
        public string FromClity { get; set; }
        public string ToClity { get; set; }
        /// <summary>
        /// In case of Multiple City/ or Return Type We can Display Mark 1 , and for Return the Segment will be 2 and 
        /// if multiple City thenIt could be Marked as 1 and 2
        /// </summary>
        public int SegmentId { get; set; }
        public DateTime TravelDate { get; set; }
        
    }

    public class mdlSearchRequest
    {   
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }
        public bool DirectFlight { get; set; }
        public enmCabinClass CabinClass { get; set; }
        public List<mdlRouteInfo> RouteInfo { get; set; }
        public enmPreferredDepartureTime PreferredDepartureTime { get; set; }
        public List<string> PreferredAirlines { get; set; }
    }

    public class mdlAirport 
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        public string City{ get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
    }

    public class mdlAirline
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsLCC { get; set; }
    }


    public class mdlFareDetails
    {
        public enmPassengerType PassengerType { get; set; }
        public int PassengerCount { get; set; }
        public double BaseFare { get; set; }
        public double Tax{ get; set; }
        public List<mdlFareBreakup> TaxBreakup { get; set; }
    }

    public class mdlFareBreakup
    {
        public enmTaxandFeeType FbKey { get; set; }
        public double FbValue { get; set; }
    }

    public class mdlFare { 
        public string CurrencyCode { get; set; }
        public double BaseFare { get; set; }
        public double Taxes { get; set; }
        public List<mdlFareBreakup> TaxBreakup { get; set; }        
        public double OtherCharges { get; set; }
        public List<mdlFareBreakup> OChargeBu { get; set; }//Other Charges Breakup
    }

    public class mdlFlightDetail 
    {
        public mdlAirline Airline { get; set; }
        public string FlightNumber{ get; set; }
        public DateTime DepTime { get; set; }        
        public mdlAirport Origin { get; set; }
        public string OrTerminal { get; set; }//Origin Terminal
        public mdlAirport Destination { get; set; }
        public string DesTerminal { get; set; }//Destination Terminal
        public DateTime ArrTime { get; set; }
        public int Duration { get; set; }
        public int SegmentId { get; set; }
        public int TripId { get; set; }        
        public enmCabinClass CabinClass { get; set; }
        public string Baggage { get; set; }
        public string CabinBaggage { get; set; }
        
    }


    public class mdlSearchResponse
    {
        public string TraceId { get; set; }
        public ApiError Error { get; set; }
    }

    public interface IWing
    {
        public AuthenticateResponse Login();
        
    }
}
