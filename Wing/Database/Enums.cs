using System;
using System.Collections.Generic;
using System.Text;

namespace Database
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
        Refund = 2,
        PartialBooked = 3,
        Failed = 4,
        All = 100,
    }



    public enum enmCustomerType
    {
        Admin = 1,
        MLM = 2,
        B2B = 3,
        B2C = 4,
        InHouse = 5
    }


    public enum enmServiceProvider
    {
        None = 0,
        TBO = 1,
        TripJack = 2,
        Kafila = 3
    }

    public enum enmPassengerType
    {
        Adult = 1,
        Child = 2,
        Infant = 3,
    }
    public enum enmGender
    {
        Male = 1,
        Female = 2,
        Trans = 4,
    }

    public enum enmPackageCustomerType
    {
        Solo = 1,
        Couple = 2,
        Family=4,
        Friends=8,
        Cooperate=16,
    }
    public enum enmStatus
    {
        Active = 0,
        Deactive = 1,
    }

    public enum enmTCStatus
    {
        Active=1,
        Block = 2,
        Terminate = 3,

    }
    public enum enmTCRanks
    {
        Level1=1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5,
        Level6 = 6,
        Level7 = 7,
        Level8 = 8,
    }

    public enum enmAddressType
    {
        Permanent=1,
        Contact=2
    }

    public enum enmBookingType
    {
        Flight = 1,
        Bus = 2,
        Train =3,
        Hotel=4
    }

    public enum enmUserType
    {
        Consolidator=1,
        Employee=2,
        B2B=4,
        B2C=8
    }

    public enum enmMessageType
    {
        Success=1,
        Error=2,
        Warning = 3,
        Info = 4,
    }
    public enum enmApprovalType : byte
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        InProcessing = 3,        
    }
    public enum enmProcessStatus : byte
    {
        Pending = 0,
        Completed = 1,
        InProcessing = 2,
        Hold = 3,
        
    }

    public enum enmIsKycUpdated
    {
        No=0,
        Yes=1,
        Partial=2
    }
   
    public enum enmIdentityProof
    {   
        Aadhar =1,
        Passport=2,
        PANcard=3,
        DrivingLicense=4,
        VoterID=5,
    }

    public enum enmNomineeRelation
    {
        Father = 1,
        Mother = 2,
        Husband = 3,
        Wife = 4,
        Son = 5,
        Daughter=6,
    }


    public enum enmMemberRank
    {
        Diamond = 1,
        Turquoise = 2,
        Amber = 3,
        Zircon = 4,
        Onyx = 5,
        lolite = 6,
        Ivory = 7,
        Amethyst=8,
    }

    public enum enmIncentiveStatus
    {
        Pending = 1,
        Processing = 2,
        Hold = 3,
        Release = 4,
        Cancelled = 5,
        Adjusted = 6,
    }

    public enum enmAddressProof
    {
        Aadhar = 1,
        Passport = 2,
        Passbook = 3,
        RationCard = 4,
        Waterbill = 5,
        ElectricityBill = 6,
    }
    public enum enmWalletTransactiontype
    {
        Credit= 1,
        Debit= 2,
    }


    public enum enmLoadData
    {
        ByID=1,
        ByNid=2,
        ByDateFilter=3,
        ByApproved=4,
        ByReject=5
    }

    public enum enmIncentiveTransactionType
    { 
        PushIncentive=1,
        DipatchIncentive=2,
        HoldIncenitve=3,
        UnHoldIncentive=4,
        AdjustIncentive=5,
        TransferToWallet=6,
        ReturnFromWallet = 7,
        ReturnIncentive=8

    }
}
