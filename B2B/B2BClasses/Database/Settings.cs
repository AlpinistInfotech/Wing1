using B2BClasses.Services.Air;
using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace B2BClasses.Database
{

    #region  ****************** Base Classes ***************************

    public class DbWingMarkupMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public enmMarkupApplicability Applicability { get; set; }
        public bool IsAllProvider { get; set; }
        public bool IsAllCustomerType { get; set; }
        public bool IsAllCustomer { get; set; }
        public bool IsAllPessengerType { get; set; }//Applicable For All Pasenger
        public bool IsAllFlightClass { get; set; }
        public bool IsAllAirline { get; set; }
        public enmGender Gender { get; set; }
        public double MarkupAmt { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public class DbWingMarkupServiceProvider
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public virtual int? MarkupId { get; set; }        
        public enmServiceProvider ServiceProvider { get; set; }
    }

    public class DbWingMarkupCustomerType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public virtual int? MarkupId { get; set; }        
        public enmCustomerType customerType { get; set; }
    }

    public class DbWingMarkupCustomerDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public virtual int? MarkupId { get; set; }        
        [ForeignKey("tblCustomerMaster")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }
    }

    public class DbWingMarkupPassengerType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public virtual int? MarkupId { get; set; }        
        public enmPassengerType PassengerType { get; set; }
    }


    public class DbWingMarkupFlightClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public virtual int? MarkupId { get; set; }
        public string classid { get; set; }
    }

    public class DbWingMarkupAirline
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public virtual int? MarkupId { get; set; }        
        [ForeignKey("tblAirline")] // Foreign Key here
        public int? AirlineId { get; set; }
        public tblAirline tblAirline { get; set; }
    }

    #endregion


    public class tblAirlineFareRule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AirlineFareRuleId { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
        public string traceId { get; set; }
        public string resultIndex { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public double ReissueCharge { get; set; }
        public double ReissueAdditionalCharge { get; set; }
        public string ReissuePolicy { get; set; }
        public double CancellationCharge { get; set; }
        public double CancellationAdditionalCharge { get; set; }
        public string CancellationPolicy { get; set; }
        public string CheckingBaggage { get; set; }
        public string CabinBaggage { get; set; }
    }
    public class tblTravelMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TravelDetailId { get; set; }
        public DateTime TravelDate { get; set; }
        public enmCabinClass CabinClass { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string TokenId { get; set; }
        public string TraceId { get; set; }
        public DateTime GenrationDt { get; set; }
        public DateTime ExpireDt { get; set; }
        public double MinPublishFare { get; set; }
        public double MinPublishFareReturn { get; set; }
        public enmJourneyType JourneyType { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }
    }
    public class tblTravelDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResultId { get; set; }
        public int segmentId { get; set; }
        public string ResultIndex { get; set; }
        public string ResultType { get; set; }
        public double PublishedFare { get; set; }
        public double OfferedFare { get; set; }
        public string JsonData { get; set; }
    }

    public class tblMinBalenceAlert
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool ForAllServiceProvider { get; set; }
        public double AmountForSendMail { get; set; }
        public double AmountForDisableProvider { get; set; }// If Amount is Below then This Amount the Service Will be Disabled
        public DateTime EffectiveFromDt { get; set; }
        public string Remarks { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }
    public class tblMinBalenceAlertDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblMinBalenceAlert")] // Foreign Key here
        public int? AlertId { get; set; }
        public tblMinBalenceAlert tblMinBalenceAlert { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
    }
    public class tblActiveSerivceProvider
    {
        [Key]
        public enmServiceProvider ServiceProvider { get; set; }
        public bool IsEnabled { get; set; }        
        public string Remarks { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }
    public class tblActiveSerivceProviderLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDisabledBySystem { get; set; }
        public DateTime DisabledFromDt { get; set; }
        public string Remarks { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public class tblWingMarkupMaster: DbWingMarkupMaster
    {
        
    }
    public class tblWingMarkupServiceProvider:DbWingMarkupServiceProvider
    {   
        [ForeignKey("tblWingMarkupMaster")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingMarkupMaster tblWingMarkupMaster { get; set; }        
    }
    public class tblWingMarkupCustomerType: DbWingMarkupCustomerType
    {
        [ForeignKey("tblWingMarkupMaster")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingMarkupMaster tblWingMarkupMaster { get; set; }        
    }
    public class tblWingMarkupCustomerDetails: DbWingMarkupCustomerDetails
    {
        [ForeignKey("tblWingMarkupMaster")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingMarkupMaster tblWingMarkupMaster { get; set; }
        
    }
    public class tblWingMarkupPassengerType : DbWingMarkupPassengerType
    {   
        [ForeignKey("tblWingMarkupMaster")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingMarkupMaster tblWingMarkupMaster { get; set; }        
    }

    public class tblWingMarkupFlightClass: DbWingMarkupFlightClass
    {
        [ForeignKey("tblWingMarkupMaster")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingMarkupMaster tblWingMarkupMaster { get; set; }
    }
    public class tblWingMarkupAirline :DbWingMarkupAirline
    {
        [ForeignKey("tblWingMarkupMaster")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingMarkupMaster tblWingMarkupMaster { get; set; }    
    }
    




    public class tblWingConvenience
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double ConvenienceAmt { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public class tblAirline
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool isLcc { get; set; }
        public string ImagePath { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }

    }
    public class tblAirport: mdlAirport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsDomestic { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

}
