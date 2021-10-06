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

        public string TokenId { get; set; }
        public string EndUserIp { get; set; }
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
        public enmServiceProvider ServiceProvider { get; set; }
        public string traceid { get; set; }
        public List<mdlSegment> Segment { get; set; }
        public List<mdlTotalpricelist> TotalPriceList { get; set; }
    }

    public class mdlSegment
    {
        public int Id { get; set; }
        public mdlAirline Airline { get; set; }
        public mdlAirport Origin { get; set; }
        public mdlAirport Destination { get; set; }

        public int TripIndicator { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int Mile { get; set; }
        public int Duration { get; set; }

        //extra for tbo
        public int SeatRemaing { get; set; }
        public mdlBaggageInformation BaggageInformation { get; set; }
        public int RefundableType { get; set; }//0 Non Refundable,1 - Refundable,2 - Partial Refundable
        public enmCabinClass CabinClass { get; set; }//ECONOMY,PREMIUM_ECONOMY, BUSINESS,FIRST
        public string ClassOfBooking { get; set; }
        public string FareBasis { get; set; }
        public bool IsFreeMeel { get; set; }
        public double Convenience { get; set; }
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

    public class mdlAirport
    {
        [MaxLength(200)]
        public string AirportCode { get; set; }
        [MaxLength(200)]
        public string AirportName { get; set; }
        [MaxLength(200)]
        public string Terminal { get; set; }
        [MaxLength(200)]
        public string CityCode { get; set; }
        [MaxLength(200)]
        public string CityName { get; set; }
        [MaxLength(200)]
        public string CountryCode { get; set; }
        [MaxLength(200)]
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

        public mdlFareRuleResponse FareRule { get; set; }
        public string ResultIndex { get; set; }
        public string sri { get; set; }
        public List<string> msri { get; set; }
        public double Convenience { get; set; }
        public double TotalConvenience { get; set; }
        public double TotalPrice { get; set; }
        public double Discount { get; set; }
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
        public double WingDiscount { get; set; }
        //for tbo
        public string Currency { get; set; }
        public double Tax { get; set; }
        public double YQTax { get; set; }
        public double AdditionalTxnFeePub { get; set; }
        public double AdditionalTxnFeeOfrd { get; set; }
        public double OtherCharges { get; set; }
        public double Discount { get; set; }
        public double PublishedFare { get; set; }
        public double OfferedFare { get; set; }
        public double TdsOnCommission { get; set; }
        public double TdsOnPLB { get; set; }
        public double TdsOnIncentive { get; set; }
        public double ServiceFee { get; set; }

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
        public string TokenId { get; set; }
        public string BookingId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public List<List<mdlSearchResult>> Results { get; set; }
        public mdlTotalPriceInfo TotalPriceInfo { get; set; }
        public mdlFlightSearchWraper SearchQuery { get; set; }
        public mdlFareQuoteCondition FareQuoteCondition { get; set; }
        public string userip { get; set; }

    }

    public class mdlTotalPriceInfo
    {
        public double TaxAndFees { get; set; }
        public double BaseFare { get; set; }
        public double TotalFare { get; set; }
        public double othercharge { get; set; }
        public double convincecharge { get; set; }
        public double discount { get; set; }
        public double insurance { get; set; }
    }

    public class mdlFareQuoteCondition
    {
        public bool IsLCC { get; set; }
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
        public bool isPassportRequired { get; set; }
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
        public string id { get; set; }
        public string flowType { get; set; }
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
        public mdlFareRule[] tboFareRule { get; set; }
    }


    public class mdlFareRule
    {
        public bool isML { get; set; }
        public bool isHB { get; set; }
        public string rT { get; set; }
        public mdlPassengerBagege cB { get; set; }
        public mdlPassengerBagege hB { get; set; }
        public mdlFarePolicy fr { get; set; }

        public string Airline { get; set; }
        public string FlightId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string FareBasisCode { get; set; }
        public string FareRuleDetail { get; set; }
        public string FareRestriction { get; set; }
        public string FareFamilyCode { get; set; }
        public string FareRuleIndex { get; set; }
        public string DepartureTime { get; set; }
        public string ReturnDate { get; set; }
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
        public string TokenId { get; set; }
        public string userip { get; set; }
        public string resultindex { get; set; }
        public string BookingId { get; set; }
        public bool IsLCC { get; set; }
        public List<mdlTravellerinfo> travellerInfo { get; set; }
        public mdlDeliveryinfo deliveryInfo { get; set; }
        public mdlGstInfo gstInfo { get; set; }
        public List<mdlPaymentInfos> paymentInfos { get; set; }

    }

   
    public class mdlDeliveryinfo
    {
        public List<string> emails { get; set; }
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
        public string bookingclass { get; set; }
        public string cabinclass { get; set; }
        public string farebasis { get; set; }
        public DateTime PassportExpiryDate { get; set; }
        public DateTime PassportIssueDate { get; set; }
        public mdlSSRS Baggage { get; set; }
        public mdlSSRS ssrMealInfos { get; set; }
        public mdlSSRS ssrSeatInfos { get; set; }
        public mdlSSRS ssrExtraServiceInfos { get; set; }
        public List<mdlSSRS> ssrBaggageInfoslist { get; set; }
        public List<mdlSSRS> ssrMealInfoslist { get; set; }
        public List<mdlSSRS> ssrSeatInfoslist { get; set; }
        public List<mdlSSRS> ssrExtraServiceInfoslist { get; set; }
        //for tbo
        public string Gender { get; set; }
        public string passportNum { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string countrycode { get; set; }
        public string countryname { get; set; }
        public string cellcountrycode { get; set; }
        public string nationality { get; set; }
        public bool IsLeadPax { get; set; }
        public string FFAirlineCode { get; set; }
        public string FFNumber { get; set; }
        public int PaxType { get; set; }
        public mdlfare Fare { get; set; }
    }


    public class mdlfare
    {
        public double BaseFare { get; set; }
        public double Tax { get; set; }
        public double TransactionFee { get; set; }
        public double YQTax { get; set; }
        public double AdditionalTxnFeeOfrd { get; set; }
        public double AdditionalTxnFeePub { get; set; }
        public double AirTransFee { get; set; }
        public double OtherCharges { get; set; }
        public double Discount { get; set; }
        public double PublishedFare { get; set; }
        public double OfferedFare { get; set; }
        public double TdsOnCommission { get; set; }
        public double TdsOnPLB { get; set; }
        public double TdsOnIncentive { get; set; }
        public double ServiceFee { get; set; }
        public double TotalBaggageCharges { get; set; }
        public double TotalMealCharges { get; set; }
        public double TotalSeatCharges { get; set; }

        public double TotalSpecialServiceCharges { get; set; }
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
        public string code { get; set; }
        public string key { get; set; }
        public string desc { get; set; }
    }

    public class mdlBookingResponse
    {
        public string bookingId { get; set; }
        public string PNR { get; set; }
        public mdlError Error { get; set; }
        public mdlMetainfo metaInfo { get; set; }
        public int ResponseStatus { get; set; }
        public mdlStatus status { get; set; }
        
    }
    public class mdlTBOBookingResponse
    {

    }

    public class mdlStatus
    {
        public bool success { get; set; }
        public int httpStatus { get; set; }
        public string message { get; set; }
    }

    public class mdlMetainfo
    {
    }



    #endregion

    #region *****************************Booking Cancelation***********************


    public class mdlCancellationRequest
    {
        public string TraceId { get; set; }
        public string bookingId { get; set; }
        public string type { get; set; }
        public string remarks { get; set; }
        public mdlCancellationTripDetail[] trips { get; set; }
    }

    public class mdlCancellationTripDetail
    {
        public string srcAirport { get; set; }
        public string destAirport { get; set; }
        public DateTime departureDate { get; set; }
        public mdlTravellerBasicInfo[] travellers { get; set; }
    }

    public class mdlTravellerBasicInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }


    public class mdlFlightCancellationResponse : mdlBookingResponse
    {
        public string amendmentId { get; set; }
    }


    public class mdlFlightCancellationChargeResponse
    {
        public string bookingId { get; set; }
        public List<mdlCancelCharges> trips { get; set; }
        public mdlStatus status { get; set; }
        public mdlError Error { get; set; }
        public int ResponseStatus { get; set; }
    }

    public class mdlCancelCharges
    {

        public string srcAirport { get; set; }
        public string destAirport { get; set; }
        public DateTime departureDate { get; set; }
        public string[] flightNumbers { get; set; }
        public string[] airlines { get; set; }
        public mdlAmendmentinfo amendmentInfo { get; set; }
    }

    public class mdlAmendmentinfo : mdlTravellerBasicInfo
    {
        public mdlRefundAmount ADULT { get; set; }
        public mdlRefundAmount CHILD { get; set; }
        public mdlRefundAmount INFANT { get; set; }
    }

    public class mdlRefundAmount
    {
        public double amendmentCharges { get; set; }
        public double refundAmount { get; set; }
        public double totalFare { get; set; }
    }


    public class mdlCancelationDetails
    {
        public string bookingId { get; set; }
        public string amendmentId { get; set; }
        public string amendmentStatus { get; set; }
        public double amendmentCharges { get; set; }
        public double refundableamount { get; set; }
        public List<mdlCancelTrip> trips { get; set; }
        public mdlStatus status { get; set; }
        public mdlMetainfo metaInfo { get; set; }
        public mdlError Error { get; set; }
        public int ResponseStatus { get; set; }
    }


    public class mdlCancelTrip
    {
        public string src { get; set; }
        public string dest { get; set; }
        public string date { get; set; }
        public string[] flightNumbers { get; set; }
        public string[] airlines { get; set; }
        public List<mdlCancelTraveller> travellers { get; set; }
    }

    public class mdlCancelTraveller
    {
        public string fn { get; set; }
        public string ln { get; set; }
        public double amendmentCharges { get; set; }
        public double refundableamount { get; set; }
        public double totalFare { get; set; }
    }





    #endregion

}
