using System;
using System.Collections.Generic;
using System.Text;

namespace B2BClasses.Services.Enums
{
    public enum enmRole
    {
        SearchFlight,
        BookTicket,
        HoldTicket,
        WalletReport,
        WalletRequest,
        Markup,
        IPFilteration,
        CheckPassengerDetails
    }

    public enum enmCustomerType
    {
        Admin,
        MLM,
        B2B,
        B2C,
        InHouse
    }

    public enum enmTransactionType
    {
        TicketBook,
        WalletAmountUpdate,
        PaymentGatewayAmountUpdate,
        OnCreditUpdate,
    }

    public enum enmJourneyType
    {
        OneWay = 1, Return = 2, MultiStop = 3, AdvanceSearch = 4, SpecialReturn = 5
    }

    public enum enmGender
    {
        Male = 1,
        Female = 2,
        Other = 4

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

}
