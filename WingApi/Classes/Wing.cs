using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WingApi.Classes
{


    public enum enmJourneyType
    {
        OneWay = 1, Return = 2, MultiStop = 3, AdvanceSearch = 4, SpecialReturn = 5
    }

    public enum enmServiceProvider
    {
        TBO = 1,
        TripJack = 2,
        Kafila = 3
    }

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
        Adult = 1,
        Child = 2,
        Infant = 3,
    }

    public enum enmRefundableType
    {
        NonRefundable = 0,
        Refundable = 1,
        PartialRefundable = 2
    }

    public enum enmCabinClass
    {
        //ALL=1,
        ECONOMY = 2,
        PREMIUM_ECONOMY = 3,
        BUSINESS = 4,
        //PremiumBusiness=5,
        FIRST = 6
    }

    public enum enmPreferredDepartureTime
    {
        AnyTime = 1,
        Morning = 2,
        AfterNoon = 3,
        Evening = 4,
        Night = 5
    }

    public class mdlAuthenticateRequest
    {
        public string ClientId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EndUserIp { get; set; }
    }


    public class mdlAuthenticateResponse
    {
        public int Status { get; set; }
        public string TokenId { get; set; }
        public Error Error { get; set; }
        public Member Member { get; set; }
    }

    public class Error
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Member
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int MemberId { get; set; }
        public int AgencyId { get; set; }
        public string LoginName { get; set; }
        public string LoginDetails { get; set; }
        public bool isPrimaryAgent { get; set; }
    }



    #region ************************* Search Classes ***************************




    public class mdlSearchRequest
    {
        public string EndUserIp { get; set; }
        public string TokenId { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }
        public bool DirectFlight { get; set; }
        //public string OneStopFlight { get; set; }
        public enmJourneyType JourneyType { get; set; }
        public string[] PreferredAirlines { get; set; }
        [Required]
        public mdlSegmentRequest[] Segments { get; set; }
        public string[] Sources { get; set; }
    }

    public class mdlSegmentRequest
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public enmCabinClass FlightCabinClass { get; set; }
        public DateTime TravelDt { get; set; }
        public enmPreferredDepartureTime PreferredDeparture { get; set; }
        public enmPreferredDepartureTime PreferredArrival { get; set; }
    }


    public class mdlSearchResponse
    {
        public int ResponseStatus { get; set; }
        public mdlError Error { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
        public string TraceId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public mdlSearchResult[][] Results { get; set; }
    }

    public class mdlError
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
    }

    public class mdlSearchResult
    {
        public bool IsHoldAllowedWithSSR { get; set; }
        public string ResultIndex { get; set; }
        public string ResultType { get; set; }
        public int Source { get; set; }
        public bool IsLCC { get; set; }
        public bool IsRefundable { get; set; }
        public bool IsPanRequiredAtBook { get; set; }
        public bool IsPanRequiredAtTicket { get; set; }
        public bool IsPassportRequiredAtBook { get; set; }
        public bool IsPassportRequiredAtTicket { get; set; }
        public bool GSTAllowed { get; set; }
        public bool IsCouponAppilcable { get; set; }
        public bool IsGSTMandatory { get; set; }
        public string AirlineRemark { get; set; }
        public string ResultFareType { get; set; }
        public mdlFare Fare { get; set; }
        public mdlFarebreakdown[] FareBreakdown { get; set; }
        public mdlSegmentResponse[][] Segments { get; set; }
        //public DateTime LastTicketDate { get; set; }
        public string TicketAdvisory { get; set; }
        public mdlFarerule[] FareRules { get; set; }
        public string AirlineCode { get; set; }
        public string ValidatingAirline { get; set; }
        public bool IsUpsellAllowed { get; set; }
        public mdlPenaltycharges PenaltyCharges { get; set; }
    }

    public class mdlFare
    {
        public string Currency { get; set; }
        public double BaseFare { get; set; }
        public double Tax { get; set; }
        public mdlTaxbreakup[] TaxBreakup { get; set; }
        public double YQTax { get; set; }
        public double AdditionalTxnFeeOfrd { get; set; }
        public double AdditionalTxnFeePub { get; set; }
        public double PGCharge { get; set; }
        public double OtherCharges { get; set; }
        public mdlChargebu[] ChargeBU { get; set; }
        public double Discount { get; set; }
        public double PublishedFare { get; set; }
        public double CommissionEarned { get; set; }
        public double PLBEarned { get; set; }
        public double IncentiveEarned { get; set; }
        public double OfferedFare { get; set; }
        public double NetFare { get; set; }
        public double TdsOnCommission { get; set; }
        public double TdsOnPLB { get; set; }
        public double TdsOnIncentive { get; set; }
        public double ServiceFee { get; set; }
        public double TotalBaggageCharges { get; set; }
        public double TotalMealCharges { get; set; }
        public double TotalSeatCharges { get; set; }
        public double TotalSpecialServiceCharges { get; set; }
    }

    public class mdlTaxbreakup
    {
        public string key { get; set; }
        public double value { get; set; }
    }

    public class mdlChargebu
    {
        public string key { get; set; }
        public double value { get; set; }
    }

    public class mdlPenaltycharges
    {
        public dynamic ReissueCharge { get; set; }
        public dynamic CancellationCharge { get; set; }
    }

    public class mdlFarebreakdown
    {
        public string Currency { get; set; }
        public enmPassengerType PassengerType { get; set; }
        public int PassengerCount { get; set; }
        public double BaseFare { get; set; }
        public double Tax { get; set; }
        public mdlTaxbreakup[] TaxBreakUp { get; set; }
        public double YQTax { get; set; }
        public double AdditionalTxnFeeOfrd { get; set; }
        public double AdditionalTxnFeePub { get; set; }
        public double PGCharge { get; set; }
        public double SupplierReissueCharges { get; set; }
    }


    public class mdlSegmentResponse
    {
        public string Baggage { get; set; }
        public string CabinBaggage { get; set; }
        public enmCabinClass CabinClass { get; set; }
        public int TripIndicator { get; set; }
        public int SegmentIndicator { get; set; }
        public mdlAirline Airline { get; set; }
        public int NoOfSeatAvailable { get; set; }
        public mdlOrigin Origin { get; set; }
        public mdlDestination Destination { get; set; }
        public int Duration { get; set; }
        public int GroundTime { get; set; }
        public int Mile { get; set; }
        public bool StopOver { get; set; }
        public string FlightInfoIndex { get; set; }
        public string StopPoint { get; set; }
        public DateTime? StopPointArrivalTime { get; set; }
        public DateTime? StopPointDepartureTime { get; set; }
        public string Craft { get; set; }
        public string Remark { get; set; }
        public bool IsETicketEligible { get; set; }
        public string FlightStatus { get; set; }
        public string Status { get; set; }
        public int AccumulatedDuration { get; set; }
    }

    public class mdlAirline
    {
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string FlightNumber { get; set; }
        public string FareClass { get; set; }
        public string OperatingCarrier { get; set; }
    }

    public class mdlOrigin
    {
        public mdlAirport Airport { get; set; }
        public DateTime DepTime { get; set; }
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

    public class mdlDestination
    {
        public mdlAirport Airport { get; set; }
        public DateTime ArrTime { get; set; }
    }



    public class mdlFarerule
    {
        public int FlightId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string FareBasisCode { get; set; }
        public string FareRuleDetail { get; set; }
        public string FareRestriction { get; set; }
        public string FareFamilyCode { get; set; }
        public string FareRuleIndex { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ReturnDate { get; set; }
    }

    #endregion


    #region *********************** Fare Quotation **********************

    public class mdlFareQuotRequest
    {
        public string EndUserIp { get; set; }
        public string TraceId { get; set; }
        public string[] ResultIndex { get; set; }
        public enmServiceProvider Provider { get; set; }
    }

    public class mdlFareQuotResponseWraper
    {
        public mdlFareQuotResponse Response { get; set; }
    }
    public class mdlFareQuotResponse
    {
        public int ResponseStatus { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
        public bool IsPriceChanged { get; set; }
        public mdlError Error { get; set; }
        public string TraceId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public mdlSearchResult[][] Results { get; set; }
    }

    #endregion

    #region ***************** Fare Rule ***************************
    public class mdlFareRuleRequest : mdlFareQuotRequest
    {

    }
    public class mdlFareRuleResponseWraper
    {
        public mdlFareRuleResponse Response { get; set; }
    }

    public class mdlFareRuleResponse
    {
        public mdlError Error { get; set; }
        public mdlFarerule[] FareRules { get; set; }
        public int ResponseStatus { get; set; }
        public string TraceId { get; set; }
    }
    #endregion


    #region *********************  Raju Response **************************



    public class Rootobject
    {
        public Item[] Items { get; set; }
    }

    public class Item
    {
        public string TraceID { get; set; }
        public string TokenID { get; set; }
        public string Inbound { get; set; }
        public float BaseFare { get; set; }
        public float Taxes { get; set; }
        public float YQTax { get; set; }
        public float ServiceFee { get; set; }
        public float BaseFare_outbound { get; set; }
        public float Taxes_outbound { get; set; }
        public float YQTax_outbound { get; set; }
        public float ExtraCharges_outbound { get; set; }
        public float BaseFare_inbound { get; set; }
        public float Taxes_inbound { get; set; }
        public float YQTax_inbound { get; set; }
        public float ExtraCharges_inbound { get; set; }
        public float AdditionalTxnFeeOfrd { get; set; }
        public float TotalPrice { get; set; }
        public float ExtraCharges { get; set; }
        public float OfferedFare { get; set; }
        public float PublishedFare { get; set; }
        public string isCache { get; set; }
        public bool IstResult { get; set; }
        public float MarkUp { get; set; }
        public bool IsMarkup { get; set; }
        public float MarkupPlus { get; set; }
        public float MarkupMinus { get; set; }
        public float DiscountOnResult { get; set; }
        public float FareIncrease { get; set; }
        public float Commission { get; set; }
        public float Safi { get; set; }
        public float Conveniencefee_oneway { get; set; }
        public float Conveniencefee_roundway { get; set; }
        public int Conveniencefee_NoDay { get; set; }
        public float GrandTotal { get; set; }
        public string Currency { get; set; }
        public string Currency_new { get; set; }
        public string FareType { get; set; }
        public string FareRestriction { get; set; }
        public int IndexNumber { get; set; }
        public string ResultIndex { get; set; }
        public string ResultIndexOutbound { get; set; }
        public string ResultIndexInbound { get; set; }
        public string IsRefundable { get; set; }
        public string IsLccOutbound { get; set; }
        public string IsLccInbound { get; set; }
        public string Provider { get; set; }
        public string ValCarrier { get; set; }
        public string LastTicketingDate { get; set; }
        public string Fltcheck { get; set; }
        public string Fltcheck_Flt { get; set; }
        public Adultinfo AdultInfo { get; set; }
        public Childinfo ChildInfo { get; set; }
        public Infantinfo InfantInfo { get; set; }
        public object[] FareBasisCodes { get; set; }
        public Sector[] Sectors { get; set; }
        public object[] Baggages { get; set; }
        public string Key { get; set; }
        public string KeyPt { get; set; }
        public string CffCharge { get; set; }
        public int Priority_air { get; set; }
        public int Priority_stop { get; set; }
    }

    public class Adultinfo
    {
        public int NoAdult { get; set; }
        public float AdTax { get; set; }
        public float AdtBFare { get; set; }
        public float AdtPricePP { get; set; }
        public float MarkUp { get; set; }
        public float Commission { get; set; }
        public float Safi { get; set; }
        public float OtherCharges { get; set; }
    }

    public class Childinfo
    {
        public int NoChild { get; set; }
        public float CHTax { get; set; }
        public float ChdBFare { get; set; }
        public float MarkUp { get; set; }
        public float Commission { get; set; }
        public float Safi { get; set; }
        public float OtherCharges { get; set; }
    }

    public class Infantinfo
    {
        public int NoInfant { get; set; }
        public float InTax { get; set; }
        public float InfBFare { get; set; }
        public float MarkUp { get; set; }
        public float Commission { get; set; }
        public float Safi { get; set; }
        public float OtherCharges { get; set; }
    }

    public class Sector
    {
        public string Indicator { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string AirlineLogoPath { get; set; }
        public string Class { get; set; }
        public Cabinclass CabinClass { get; set; }
        public int NoSeats { get; set; }
        public string FltNum { get; set; }
        public string AmountAirCode { get; set; }
        public string EquipType { get; set; }
        public string ElapsedTime { get; set; }
        public string AirChange { get; set; }
        public string ActualTime { get; set; }
        public string TotalFlightDuration { get; set; }
        public object IsConnect { get; set; }
        public Techstopover TechStopOver { get; set; }
        public string Status { get; set; }
        public string IsReturn { get; set; }
        public Operatingcarrier OperatingCarrier { get; set; }
        public object MarketingCarrier { get; set; }
        public string BaggageInfo { get; set; }
        public object TransitTime { get; set; }
        public string Key { get; set; }
        public string SegmentIndex { get; set; }
        public string Distance { get; set; }
        public string ETicket { get; set; }
        public string ChangeOfPlane { get; set; }
        public string ParticipantLevel { get; set; }
        public bool OptionalServicesIndicator { get; set; }
        public string AvailabilitySource { get; set; }
        public string Group { get; set; }
        public Departure Departure { get; set; }
        public Arrival Arrival { get; set; }
    }

    public class Cabinclass
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Techstopover
    {
        public int TechStopOver { get; set; }
        public string DepartureAirport { get; set; }
        public string GroundTime { get; set; }
        public string DepartureAirportName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string ArrivalAirport { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string AirpName { get; set; }
    }

    public class Operatingcarrier
    {
        public string OptrCarrierCode { get; set; }
        public object OptrCarrierDes { get; set; }
    }

    public class Departure
    {
        public string AirportCode { get; set; }
        public string AirportName { get; set; }
        public string AirportCityCode { get; set; }
        public string AirportCityName { get; set; }
        public string AirportCountryCode { get; set; }
        public string AirportCountryName { get; set; }
        public string GeoLocation { get; set; }
        public string Terminal { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public DateTime FullDate { get; set; }
        public string Day { get; set; }
        public object DateTimeStamp { get; set; }
    }

    public class Arrival
    {
        public string AirportCode { get; set; }
        public DateTime FullDate { get; set; }
        public string AirportName { get; set; }
        public string AirportCityCode { get; set; }
        public string AirportCityName { get; set; }
        public string AirportCountryCode { get; set; }
        public string AirportCountryName { get; set; }
        public string GeoLocation { get; set; }
        public string Terminal { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Day { get; set; }
        public object DateTimeStamp { get; set; }
    }

    #endregion



    public interface IWing
    {
        Task<mdlSearchResponse> SearchAsync(mdlSearchRequest request);
        Task<mdlFareQuotResponse> FareQuoteAsync(mdlFareQuotRequest request);
        Task<mdlFareRuleResponse> FareRuleAsync(mdlFareRuleRequest request);

    }
}
