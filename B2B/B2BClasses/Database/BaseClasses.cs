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
        public double Amount { get; set; }
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

    #endregion

}
