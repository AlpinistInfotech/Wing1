using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2C.Classes
{
    public enum enmJourneyType
    {
        OneWay = 1, Return = 2, MultiStop = 3, AdvanceSearch = 4, SpecialReturn = 5
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

    public enum enmFlighWingCharge
    {
        CustomerMarkup = 1,
        WingMarkup = 2,
        Discount = 3,
        Convenience = 4,
        PaymentGateway = 5,
        MLMCharge = 6,
        BaggageServices = 4,
        MealServices = 8,
        SeatServices = 16,
    }

    public enum enmGender
    {
        None = 0,
        Male = 1,
        Female = 2,
        Trans = 4,
        ALL = 8
    }
    public enum enmMessageType
    {
        None = 0,
        Success = 1,
        Error = 2,
        Warning = 3,
        Info = 4,
    }
    public enum enmFlightSearvices
    {
        OnTicket = 1,
        OnPassenger = 2,
        OnBaggageServices = 4,
        OnMealServices = 8,
        OnSeatServices = 16,
        OnExtraService = 32
    }

    public enum enmFlightType
    {
        Direct = 1,
        Connected = 2,
        All = 4

    }

    public enum enmPreferredDepartureTime
    {
        AnyTime = 1,
        Morning = 2,
        AfterNoon = 3,
        Evening = 4,
        Night = 5
    }

    public enum enmBookingStatus
    {
        Pending = 0,
        Booked = 1,
        Cancel = 2,
        Partial = 4,
        WingBooked = 8,
        Hold = 16
    }
    public enum enmPaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Initiated = 4,
        Partial = 8
    }


    public enum enmRefundStatus
    {
        Pending = 0,
        Settled = 1,
        Initiated = 2,
        Cancel = 4,
        RefundReturned = 8,
    }


    public enum enmBankTransactionType
    {
        None = 0,
        UPI = 1,
        NEFT = 2,
        RTGS = 3,
        CHEQUE = 4
    }


    public enum enmServiceProvider
    {
        None = 0,
        TripJack = 1,
        TBO = 2,
        Kafila = 3
    }

    public enum enmPassengerType
    {
        Adult = 1,
        Child = 2,
        Infant = 3,
    }


    public enum enmCustomerType
    {
        None = 1,
        MLM = 2,
        B2B = 3,
        B2C = 4,
        InHouse = 5
    }



}
