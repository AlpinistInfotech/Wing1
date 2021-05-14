using B2BClasses.Models;
using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace B2BClasses.Services.Air
{
    #region ******************* Search Request **********************888

    public class mdlSearchRequest
    {
        [Required]
        public int AdultCount { get; set; } = 1;
        public int ChildCount { get; set; } = 0;
        public int InfantCount { get; set; } = 0;
        public bool DirectFlight { get; set; }
        public enmJourneyType JourneyType { get; set; } = enmJourneyType.OneWay;
        public string[] PreferredAirlines { get; set; }
        [Required]
        public List<mdlSegmentRequest> Segments { get; set; }

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

    public class mdlError
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class mdlSearchResponse
    {
        public int ResponseStatus { get; set; }
        public mdlError Error { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
        public string TraceId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public List<List<mdlSearchResult>> Results { get; set; }
    }

    public class mdlSearchResult
    {
        public List<mdlSegment> Segment { get; set; }
        public List<mdlTotalpricelist> TotalPriceList { get; set; }
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
        public double BaseFare { get; set; }
        public string fareIdentifier { get; set; }
        public double CustomerMarkup { get; set; }        
        public double WingMarkup { get; set; }        
        public mdlPassenger ADULT { get; set; }
        public mdlPassenger CHILD { get; set; }
        public mdlPassenger INFANT { get; set; }
        public string ResultIndex { get; set; }
        public string sri { get; set; }
        public List<string> msri { get; set; }
        public double Convenience { get; set; }
        public double TotalConvenience { get; set; }
        public double TotalPrice { get; set; }
        public double NetPrice { get; set; }
    }

    public class mdlPassenger
    {
        public mdlFareComponent FareComponent { get; set; }
        public mdlAdditionalFareComponent AdditionalFareComponent { get; set; }
        public int SeatRemaing { get; set; }
        public mdlBaggageInformation BaggageInformation { get; set; }
        public int RefundableType { get; set; }//0 Non Refundable,1 - Refundable,2 - Partial Refundable
        public enmCabinClass CabinClass { get; set; }//ECONOMY,PREMIUM_ECONOMY, BUSINESS,FIRST
        public string ClassOfBooking { get; set; }
        public string FareBasis { get; set; }
        public bool IsFreeMeel { get; set; }
        public double Convenience { get; set; }
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
        public double NewTotalFare { get; set; }
        public double IGST { get; set; }
        public double NetCommission { get; set; }
        public double SSRP { get; set; }
        public double WingMarkup { get; set; }
        
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

    #endregion


    #region ***************** Fare Quotation **************************

    public class mdlFareQuotRequest
    {
        public string TraceId { get; set; }
        public string[] ResultIndex { get; set; }
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
        public string BookingId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public List<List<mdlSearchResult>> Results { get; set; }
        public mdlTotalPriceInfo TotalPriceInfo { get; set; }
        public mdlFlightSearchWraper SearchQuery { get; set; }
        public mdlFareQuoteCondition FareQuoteCondition { get; set; }
    }

    public class mdlTotalPriceInfo
    {
        public double TaxAndFees { get; set; }        
        public double BaseFare { get; set; }
        public double TotalFare { get; set; }
    }

    public class mdlFareQuoteCondition
    { 
        public bool IsHoldApplicable { get; set; }        
        public mdlDobCondition dob { get; set; }
        public mdlPassportCondition PassportCondition { get; set; }
        public mdlGstCondition GstCondition { get; set; }
    }
    public class mdlDobCondition
    {
        public bool adobr { get; set; }
        public bool cdobr { get; set; }
        public bool idobr { get; set; }
    }
    public class mdlPassportCondition
    {
        public bool IsPassportExpiryDate { get; set; }
        public bool isPassportIssueDate { get; set; }
        public bool isPassportRequired{ get; set; }
    }

    public class mdlGstCondition
    {
        public bool IsGstMandatory { get; set; }
        public bool IsGstApplicable { get; set; }
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
        public int ResponseStatus { get; set; }
        public mdlFareRule FareRule { get; set; }
    }


    public class mdlFareRule
    {
        public bool isML { get; set; }
        public bool isHB { get; set; }
        public string rT { get; set; }
        public mdlPassengerBagege cB { get; set; }
        public mdlPassengerBagege hB { get; set; }
        public mdlFarePolicy fr { get; set; }
    }

    public class mdlPassengerBagege
    {
        public string ADT { get; set; }
        public string CNN { get; set; }
        public string INF { get; set; }
    }


    public class mdlFarePolicy
    {
        public mdlAllPOlicy DATECHANGE { get; set; }
        public mdlAllPOlicy CANCELLATION { get; set; }
        public mdlAllPOlicy SEAT_CHARGEABLE { get; set; }
    }
    public class mdlAllPOlicy
    {
        public double amount { get; set; }
        public double additionalFee { get; set; }
        public string policyInfo { get; set; }
    }

    #endregion


    #region *****************Booking Request *************************

    public class mdlBookingRequest
    {
        public string TraceId { get; set; }
        public string BookingId { get; set; }
        public List< mdlTravellerinfo> travellerInfo { get; set; }
        public mdlDeliveryinfo deliveryInfo { get; set; }
        public mdlGstInfo gstInfo { get; set; }
        public List<mdlPaymentInfos> paymentInfos { get; set; }

    }

    public class mdlDeliveryinfo
    {
        public List< string> emails { get; set; }
        public List<string> contacts { get; set; }
    }

    public class mdlTravellerinfo
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public enmPassengerType passengerType { get; set; }
        public DateTime? dob { get; set; }
        public string pNum { get; set; }
        public DateTime PassportExpiryDate { get; set; }
        public DateTime PassportIssueDate { get; set; }
        public mdlSSRS ssrBaggageInfos { get; set; }
        public mdlSSRS ssrMealInfos { get; set; }
        public mdlSSRS ssrSeatInfos { get; set; }
        public mdlSSRS ssrExtraServiceInfos { get; set; }
    }
    public class mdlPaymentInfos
    {
        public double amount { get; set; }
    }


    public class mdlGstInfo
    {
        public string gstNumber { get; set; }
        public string email { get; set; }
        public string registeredName { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }
    }

    public class mdlSSRS
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class mdlBookingResponse
    {
        public string bookingId { get; set; }
        public mdlError Error { get; set; }
        public mdlMetainfo metaInfo { get; set; }
        public int ResponseStatus { get; set; }

    }

    public class mdlStatus
    {
        public bool success { get; set; }
        public int httpStatus { get; set; }
    }

    public class mdlMetainfo
    {
    }



    #endregion
}
