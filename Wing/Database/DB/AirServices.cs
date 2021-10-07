﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.DB
{
    #region ********************* Common Class ******************************
    public class DbWingCharge :d_ApprovedBy
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
        public double Amount { get; set; }
        public int DayCount { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public DateTime BookingFromDt { get; set; }
        public DateTime BookingToDt { get; set; }
        public bool IsDeleted { get; set; }        
    }


    public class DbWingServiceProvider
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual int? ChargeId { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
    }

    public class DbWingCustomerType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual int? ChargeId { get; set; }
        public enmCustomerType customerType { get; set; }
    }

    public class DbWingCustomerDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual int? ChargeId { get; set; }        
        public int CustomerId { get; set; }
        
    }

    public class DbWingPassengerType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual int? ChargeId { get; set; }
        public enmPassengerType PassengerType { get; set; }
    }


    public class DbWingFlightClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual int? ChargeId { get; set; }
        public enmCabinClass CabinClass { get; set; }
    }

    public class DbWingAirline
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual int? ChargeId { get; set; }
        [ForeignKey("tblAirline")] // Foreign Key here
        public int? AirlineId { get; set; }
        public tblAirline tblAirline { get; set; }
    }

    public class DbWingFare
    {   
        public double BaseFare { get; set; }
        public double NetFare { get; set; }
        public double Tax { get; set; }
        public double YQTax { get; set; }

    }

    #endregion 


    public class tblAirline :d_ModifiedBy
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
        public bool IsDeleted { get; set; }
    }

    public class tblFlightClassOfBooking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingClassId { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
        [MaxLength(20)]
        public string GenerlizedName { get; set; }//SME, FlexPlue, Corporate
    }

    public class tblAirport :d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
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
        public bool IsDomestic { get; set; }        
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

    }


    #region *************** Markup, Discount, Convenience ***************************

    public class tblFlightMarkupMaster : DbWingCharge
    {
        [InverseProperty("tblFlightMarkupMaster")]
        public ICollection<tblFlightMarkupServiceProvider> tblFlightMarkupServiceProvider { get; set; }
        [InverseProperty("tblFlightMarkupMaster")]
        public ICollection<tblFlightMarkupCustomerType> tblFlightMarkupCustomerType { get; set; }
        [InverseProperty("tblFlightMarkupMaster")]
        public ICollection<tblFlightMarkupCustomerDetails> tblFlightMarkupCustomerDetails { get; set; }
        [InverseProperty("tblFlightMarkupMaster")]
        public ICollection<tblFlightMarkupPassengerType> tblFlightMarkupPassengerType { get; set; }
        [InverseProperty("tblFlightMarkupMaster")]
        public ICollection<tblFlightMarkupFlightClass> tblFlightMarkupFlightClass { get; set; }
        [InverseProperty("tblFlightMarkupMaster")]
        public ICollection<tblFlightMarkupAirline> tblFlightMarkupAirline { get; set; }
    }


    public class tblFlightMarkupServiceProvider : DbWingServiceProvider
    {
        [ForeignKey("tblFlightMarkupMaster")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightMarkupMaster tblFlightMarkupMaster { get; set; }
    }
    public class tblFlightMarkupCustomerType : DbWingCustomerType
    {
        [ForeignKey("tblFlightMarkupMaster")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightMarkupMaster tblFlightMarkupMaster { get; set; }
    }
    public class tblFlightMarkupCustomerDetails : DbWingCustomerDetails
    {
        [ForeignKey("tblFlightMarkupMaster")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightMarkupMaster tblFlightMarkupMaster { get; set; }

    }
    public class tblFlightMarkupPassengerType : DbWingPassengerType
    {
        [ForeignKey("tblFlightMarkupMaster")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightMarkupMaster tblFlightMarkupMaster { get; set; }
    }

    public class tblFlightMarkupFlightClass : DbWingFlightClass
    {
        [ForeignKey("tblFlightMarkupMaster")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightMarkupMaster tblFlightMarkupMaster { get; set; }
    }
    public class tblFlightMarkupAirline : DbWingAirline
    {
        [ForeignKey("tblFlightMarkupMaster")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightMarkupMaster tblFlightMarkupMaster { get; set; }
    }





    public class tblFlightConvenience : DbWingCharge
    {
        [InverseProperty("tblFlightConvenience")]
        public ICollection<tblFlightConvenienceServiceProvider> tblFlightConvenienceServiceProvider { get; set; }
        [InverseProperty("tblFlightConvenience")]
        public ICollection<tblFlightConvenienceCustomerType> tblFlightConvenienceCustomerType { get; set; }
        [InverseProperty("tblFlightConvenience")]
        public ICollection<tblFlightConvenienceCustomerDetails> tblFlightConvenienceCustomerDetails { get; set; }
        [InverseProperty("tblFlightConvenience")]
        public ICollection<tblFlightConveniencePassengerType> tblFlightConveniencePassengerType { get; set; }
        [InverseProperty("tblFlightConvenience")]
        public ICollection<tblFlightConvenienceFlightClass> tblFlightConvenienceFlightClass { get; set; }
        [InverseProperty("tblFlightConvenience")]
        public ICollection<tblFlightConvenienceAirline> tblFlightConvenienceAirline { get; set; }
    }


    public class tblFlightConvenienceServiceProvider : DbWingServiceProvider
    {
        [ForeignKey("tblFlightConvenience")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightConvenience tblFlightConvenience { get; set; }
    }
    public class tblFlightConvenienceCustomerType : DbWingCustomerType
    {
        [ForeignKey("tblFlightConvenience")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightConvenience tblFlightConvenience { get; set; }
    }
    public class tblFlightConvenienceCustomerDetails : DbWingCustomerDetails
    {
        [ForeignKey("tblFlightConvenience")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightConvenience tblFlightConvenience { get; set; }

    }
    public class tblFlightConveniencePassengerType : DbWingPassengerType
    {
        [ForeignKey("tblFlightConvenience")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightConvenience tblFlightConvenience { get; set; }
    }

    public class tblFlightConvenienceFlightClass : DbWingFlightClass
    {
        [ForeignKey("tblFlightConvenience")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightConvenience tblFlightConvenience { get; set; }
    }
    public class tblFlightConvenienceAirline : DbWingAirline
    {
        [ForeignKey("tblFlightConvenience")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightConvenience tblFlightConvenience { get; set; }
    }




    public class tblFlightDiscount : DbWingCharge
    {
        [InverseProperty("tblFlightDiscount")]
        public ICollection<tblFlightDiscountServiceProvider> tblFlightDiscountServiceProvider { get; set; }
        [InverseProperty("tblFlightDiscount")]
        public ICollection<tblFlightDiscountCustomerType> tblFlightDiscountCustomerType { get; set; }
        [InverseProperty("tblFlightDiscount")]
        public ICollection<tblFlightDiscountCustomerDetails> tblFlightDiscountCustomerDetails { get; set; }
        [InverseProperty("tblFlightDiscount")]
        public ICollection<tblFlightDiscountPassengerType> tblFlightDiscountPassengerType { get; set; }
        [InverseProperty("tblFlightDiscount")]
        public ICollection<tblFlightDiscountFlightClass> tblFlightDiscountFlightClass { get; set; }
        [InverseProperty("tblFlightDiscount")]
        public ICollection<tblFlightDiscountAirline> tblFlightDiscountAirline { get; set; }
    }


    public class tblFlightDiscountServiceProvider : DbWingServiceProvider
    {
        [ForeignKey("tblFlightDiscount")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightDiscount tblFlightDiscount { get; set; }
    }
    public class tblFlightDiscountCustomerType : DbWingCustomerType
    {
        [ForeignKey("tblFlightDiscount")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightDiscount tblFlightDiscount { get; set; }
    }
    public class tblFlightDiscountCustomerDetails : DbWingCustomerDetails
    {
        [ForeignKey("tblFlightDiscount")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightDiscount tblFlightDiscount { get; set; }

    }
    public class tblFlightDiscountPassengerType : DbWingPassengerType
    {
        [ForeignKey("tblFlightDiscount")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightDiscount tblFlightDiscount { get; set; }
    }

    public class tblFlightDiscountFlightClass : DbWingFlightClass
    {
        [ForeignKey("tblFlightDiscount")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightDiscount tblFlightDiscount { get; set; }
    }
    public class tblFlightDiscountAirline : DbWingAirline
    {
        [ForeignKey("tblFlightDiscount")] // Foreign Key here
        public override int? ChargeId { get; set; }
        public tblFlightDiscount tblFlightDiscount { get; set; }
    }


    #endregion



    #region **************************Flight Search Caching **************************

    public class tblFlightSearchRequest_Caching
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CachingId { get; set; }
        public enmJourneyType JourneyType { get; set; } = enmJourneyType.OneWay;        
        [MaxLength(24)]
        public string Origin { get; set; }
        [MaxLength(24)]
        public string Destination { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }        
        public enmCabinClass FlightCabinClass { get; set; }
        public double MinmumPrice { get; set; }        
        public DateTime TravelDt { get; set; }        
        public DateTime CreatedDt { get; set; }
        public DateTime ExpiredDt { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class tblFlightSearchResponse_Caching
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResponseId { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
        public int CachingId { get; set; }        
        public double MinmumPrice { get; set; }
        [MaxLength(64)]
        public string ProviderTraceId { get; set; }
    }

    public class tblFlightSearchResponses_Caching
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IndexId { get; set; }
        public int ResponseId { get; set; }        
    }


    public class tblFlightSearchSegment_Caching
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SegmentId { get; set; }
        public int SearchIndexId { get; set; }
        public int AirlineId { get; set; }
        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }
        public int TripIndicator { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        [MaxLength(30)]
        public string FlightNumber { get; set; }
        [MaxLength(30)]
        public string AirlineType { get; set; }
        public int Mile { get; set; }
        public int Duration { get; set; }
        public int Layover { get; set; }        
        public int SegmentNumber { get; set; }
    }
    public class tblFlightFare_Caching
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FareId { get; set; }
        public int SearchIndexId { get; set; }
        [MaxLength(64)]
        public string ProviderFareDetailId { get; set; }
        [MaxLength(64)]
        public string Identifier { get; set; }//Corepreate, Publish, SME
        public int SeatRemaning { get; set; }
        public enmCabinClass CabinClass { get; set; }
        public int ClassOfBooking { get; set; }
        

    }
    public class tblFlightFareDetail_Caching
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int FareDetailId { get; set; }
        public enmPassengerType PassengerType { get; set; }
        public double BaseFare { get; set; }
        public double Tax{ get; set; }        
        public double TotalFare { get; set; }        
        public double NetFare { get; set; }
        public string CheckingBaggage { get; set; }
        public string CabinBaggage { get; set; }
        public bool IsFreeMeal { get; set; }        
        public byte IsRefundable { get; set; }
    }



    #endregion

}