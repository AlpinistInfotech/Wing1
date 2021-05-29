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
        public enmDirectFlight DirectFlight { get; set; }
        public bool IsAllProvider { get; set; }
        public bool IsAllCustomerType { get; set; }
        public bool IsAllCustomer { get; set; }
        public bool IsAllPessengerType { get; set; }//Applicable For All Pasenger
        public bool IsAllFlightClass { get; set; }
        public bool IsAllAirline { get; set; }
        public enmGender Gender { get; set; }
        public double Amount{ get; set; }
        public int DayCount { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        [MaxLength(500)]
        public string Remarks { get; set; }
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
        public enmCabinClass CabinClass { get; set; }
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
        [MaxLength(200)]
        public string traceId { get; set; }
        [MaxLength(200)]
        public string resultIndex { get; set; }
        [MaxLength(20)]
        public string Origin { get; set; }
        [MaxLength(20)]
        public string Destination { get; set; }
        public double ReissueCharge { get; set; }
        public double ReissueAdditionalCharge { get; set; }
        [MaxLength(2000)]
        public string ReissuePolicy { get; set; }
        public double CancellationCharge { get; set; }
        public double CancellationAdditionalCharge { get; set; }
        [MaxLength(200)]
        public string CancellationPolicy { get; set; }
        [MaxLength(200)]
        public string CheckingBaggage { get; set; }
        [MaxLength(200)]
        public string CabinBaggage { get; set; }
    }
    public class tblTravelMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TravelDetailId { get; set; }
        public DateTime TravelDate { get; set; }
        public enmCabinClass CabinClass { get; set; }
        [MaxLength(20)]
        public string Origin { get; set; }
        [MaxLength(20)]
        public string Destination { get; set; }
        [MaxLength(200)]
        public string TokenId { get; set; }
        [MaxLength(200)]
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
        [MaxLength(200)]
        public string ResultIndex { get; set; }
        [MaxLength(200)]
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
        [MaxLength(200)]
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
        [MaxLength(200)]
        public string Remarks { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public class tblWingMarkupMaster : DbWingMarkupMaster
    {
        [InverseProperty("tblWingMarkupMaster")]
        public ICollection<tblWingMarkupServiceProvider> tblWingMarkupServiceProvider { get; set; }
        [InverseProperty("tblWingMarkupMaster")]
        public ICollection<tblWingMarkupCustomerType> tblWingMarkupCustomerType { get; set; }
        [InverseProperty("tblWingMarkupMaster")]
        public ICollection<tblWingMarkupCustomerDetails> tblWingMarkupCustomerDetails { get; set; }
        [InverseProperty("tblWingMarkupMaster")]
        public ICollection<tblWingMarkupPassengerType> tblWingMarkupPassengerType { get; set; }
        [InverseProperty("tblWingMarkupMaster")]
        public ICollection<tblWingMarkupFlightClass> tblWingMarkupFlightClass { get; set; }
        [InverseProperty("tblWingMarkupMaster")]
        public ICollection<tblWingMarkupAirline> tblWingMarkupAirline { get; set; }
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
    




    public class tblWingConvenience : DbWingMarkupMaster
    {
        [InverseProperty("tblWingConvenience")]
        public ICollection<tblWingConvenienceServiceProvider> tblWingConvenienceServiceProvider { get; set; }
        [InverseProperty("tblWingConvenience")]
        public ICollection<tblWingConvenienceCustomerType> tblWingConvenienceCustomerType { get; set; }
        [InverseProperty("tblWingConvenience")]
        public ICollection<tblWingConvenienceCustomerDetails> tblWingConvenienceCustomerDetails { get; set; }
        [InverseProperty("tblWingConvenience")]
        public ICollection<tblWingConveniencePassengerType> tblWingConveniencePassengerType { get; set; }
        [InverseProperty("tblWingConvenience")]
        public ICollection<tblWingConvenienceFlightClass> tblWingConvenienceFlightClass { get; set; }
        [InverseProperty("tblWingConvenience")]
        public ICollection<tblWingConvenienceAirline> tblWingConvenienceAirline { get; set; }
    }


    public class tblWingConvenienceServiceProvider : DbWingMarkupServiceProvider
    {
        [ForeignKey("tblWingConvenience")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingConvenience tblWingConvenience { get; set; }
    }

    public class tblWingConvenienceCustomerType : DbWingMarkupCustomerType
    {
        [ForeignKey("tblWingConvenience")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingConvenience tblWingConvenience { get; set; }
    }
    public class tblWingConvenienceCustomerDetails : DbWingMarkupCustomerDetails
    {
        [ForeignKey("tblWingConvenience")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingConvenience tblWingConvenience { get; set; }

    }
    public class tblWingConveniencePassengerType : DbWingMarkupPassengerType
    {
        [ForeignKey("tblWingConvenience")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingConvenience tblWingConvenience { get; set; }
    }
    public class tblWingConvenienceFlightClass : DbWingMarkupFlightClass
    {
        [ForeignKey("tblWingConvenience")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingConvenience tblWingConvenience { get; set; }
    }
    public class tblWingConvenienceAirline : DbWingMarkupAirline
    {
        [ForeignKey("tblWingConvenience")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingConvenience tblWingConvenience { get; set; }
    }



    public class tblWingDiscount : DbWingMarkupMaster
    {
        [InverseProperty("tblWingDiscount")]
        public ICollection<tblWingDiscountServiceProvider> tblWingDiscountServiceProvider { get; set; }
        [InverseProperty("tblWingDiscount")]
        public ICollection<tblWingDiscountCustomerType> tblWingDiscountCustomerType { get; set; }
        [InverseProperty("tblWingDiscount")]
        public ICollection<tblWingDiscountCustomerDetails> tblWingDiscountCustomerDetails { get; set; }
        [InverseProperty("tblWingDiscount")]
        public ICollection<tblWingDiscountPassengerType> tblWingDiscountPassengerType { get; set; }
        [InverseProperty("tblWingDiscount")]
        public ICollection<tblWingDiscountFlightClass> tblWingDiscountFlightClass { get; set; }
        [InverseProperty("tblWingDiscount")]
        public ICollection<tblWingDiscountAirline> tblWingDiscountAirline { get; set; }
    }
    public class tblWingDiscountServiceProvider : DbWingMarkupServiceProvider
    {
        [ForeignKey("tblWingDiscount")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingDiscount tblWingDiscount { get; set; }
    }
    public class tblWingDiscountCustomerType : DbWingMarkupCustomerType
    {
        [ForeignKey("tblWingDiscount")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingDiscount tblWingDiscount { get; set; }
    }
    public class tblWingDiscountCustomerDetails : DbWingMarkupCustomerDetails
    {
        [ForeignKey("tblWingDiscount")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingDiscount tblWingDiscount { get; set; }

    }
    public class tblWingDiscountPassengerType : DbWingMarkupPassengerType
    {
        [ForeignKey("tblWingDiscount")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingDiscount tblWingDiscount { get; set; }
    }
    public class tblWingDiscountFlightClass : DbWingMarkupFlightClass
    {
        [ForeignKey("tblWingDiscount")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingDiscount tblWingDiscount { get; set; }
    }
    public class tblWingDiscountAirline : DbWingMarkupAirline
    {
        [ForeignKey("tblWingDiscount")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingDiscount tblWingDiscount { get; set; }
    }

    public class tblWingCustomerFlightAPI : DbWingMarkupMaster
    {
        [InverseProperty("tblWingCustomerFlightAPI")]
        public ICollection<tblWingCustomerFlightAPIServiceProvider> tblWingCustomerFlightAPIServiceProvider { get; set; }
        [InverseProperty("tblWingCustomerFlightAPI")]
        public ICollection<tblWingCustomerFlightAPICustomerType> tblWingCustomerFlightAPICustomerType { get; set; }
        [InverseProperty("tblWingCustomerFlightAPI")]
        public ICollection<tblWingCustomerFlightAPICustomerDetails> tblWingCustomerFlightAPICustomerDetails { get; set; }
         
        [InverseProperty("tblWingCustomerFlightAPI")]
        public ICollection<tblWingCustomerFlightAPIFlightClass> tblWingCustomerFlightAPIFlightClass { get; set; }
        [InverseProperty("tblWingCustomerFlightAPI")]
        public ICollection<tblWingCustomerFlightAPIAirline> tblWingCustomerFlightAPIAirline { get; set; }
    }
    public class tblWingCustomerFlightAPIServiceProvider : DbWingMarkupServiceProvider
    {
        [ForeignKey("tblWingCustomerFlightAPI")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingCustomerFlightAPI tblWingCustomerFlightAPI { get; set; }
    }
    public class tblWingCustomerFlightAPICustomerType : DbWingMarkupCustomerType
    {
        [ForeignKey("tblWingCustomerFlightAPI")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingCustomerFlightAPI tblWingCustomerFlightAPI { get; set; }
    }
    public class tblWingCustomerFlightAPICustomerDetails : DbWingMarkupCustomerDetails
    {
        [ForeignKey("tblWingCustomerFlightAPI")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingCustomerFlightAPI tblWingCustomerFlightAPI { get; set; }

    }
 
    public class tblWingCustomerFlightAPIFlightClass : DbWingMarkupFlightClass
    {
        [ForeignKey("tblWingCustomerFlightAPI")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingCustomerFlightAPI tblWingCustomerFlightAPI { get; set; }
    }
    public class tblWingCustomerFlightAPIAirline : DbWingMarkupAirline
    {
        [ForeignKey("tblWingCustomerFlightAPI")] // Foreign Key here
        public override int? MarkupId { get; set; }
        public tblWingCustomerFlightAPI tblWingCustomerFlightAPI { get; set; }
    }




    public class tblAirline
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Code { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public bool isLcc { get; set; }
        [MaxLength(500)]
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
