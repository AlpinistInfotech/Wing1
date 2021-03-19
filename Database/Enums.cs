using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public enum enmGender
    {
        Male = 1,
        Female = 2,
        Trans = 3,
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
    public enum enmUserType
    {
        Consolidator=1,
        Employee=2
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
        Approved = 1,
        Rejected = 2,
        InProcessing = 4,
        Pending = 8,
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


    public enum enmAddressProof
    {
        Aadhar = 1,
        Passport = 2,
        Passbook = 3,
        RationCard = 4,
        Waterbill = 5,
        ElectricityBill = 6,
    }
    public enum enmLoadData
    {
        ByID=1,
        ByNid=2,
        ByDateFilter=3
    }
}
