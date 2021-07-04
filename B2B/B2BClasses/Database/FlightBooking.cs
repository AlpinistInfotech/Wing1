using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace B2BClasses.Database
{
    [Index(nameof(ContactNo))]
    [Index(nameof(CustomerId))]
    public class tblFlightBookingMaster
    {
        [Key]
        [MaxLength(200)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int CustomerId { get; set; }
        public int AdultCount { get; set; } = 1;
        public int ChildCount { get; set; } = 0;
        public int InfantCount { get; set; } = 0;
        public bool DirectFlight { get; set; }
        public enmJourneyType JourneyType { get; set; } = enmJourneyType.OneWay;                        
        public enmBookingStatus BookingStatus { get; set; } = enmBookingStatus.Pending;
        [MaxLength(20)]
        public string ContactNo { get; set; }
        [MaxLength(100)]
        public string Email{ get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        
        [InverseProperty("tblFlightBookingMaster")]
        public ICollection<tblFlightBookingSegmentMaster> tblFlightBookingSegmentMaster { get; set; }
        [InverseProperty("tblFlightBookingMaster")]
        public ICollection<tblFlightBookingSegment> tblFlightBookingSegments { get; set; }
        [InverseProperty("tblFlightBookingMaster")]
        public ICollection<tblFlightBookingProviderTraceId> tblFlightBookingProviderTraceIds { get; set; }
        [InverseProperty("tblFlightBookingMaster")]
        public ICollection<tblFlightBookingPassengerDetails> tblFlightBookingPassengerDetails { get; set; }
        [InverseProperty("tblFlightBookingMaster")]
        public ICollection<tblFlightBookingGSTDetails> tblFlightBookingGSTDetails { get; set; }
        [InverseProperty("tblFlightBookingMaster")]
        public ICollection<tblFlightBookingFareDetails> tblFlightBookingFareDetails { get; set; }
        public ICollection<tblFlightBookingFarePurchaseDetails> tblFlightBookingFarePurchaseDetails { get; set; }
        [InverseProperty("tblFlightBookingMaster")]
        public ICollection<tblFlightCancelation> tblFlightCancelation { get; set; }
        
        [NotMapped]
        public string CustomerName { get; set; }

    }

    

    public class tblFlightBookingProviderTraceId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SegmentDisplayOrder { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
        public string ProviderTraceId { get; set; }
        [ForeignKey("tblFlightBookingMaster")] // Foreign Key here
        public string TraceId { get; set; }
        public tblFlightBookingMaster tblFlightBookingMaster { get; set; }
        
    }


    public class tblFlightBookingSegmentMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SegmentDisplayOrder { get; set; }
        [Required]
        [MaxLength(20)]
        public string Origin { get; set; }
        [Required]
        [MaxLength(20)]
        public string Destination { get; set; }
        public DateTime TravelDt { get; set; }
        [ForeignKey("tblFlightBookingMaster")] // Foreign Key here
        public string TraceId { get; set; }
        public tblFlightBookingMaster tblFlightBookingMaster { get; set; }
        [MaxLength(200)]
        public string BookingId { get; set; }
        public enmBookingStatus BookingStatus { get; set; } = enmBookingStatus.Pending;
        [MaxLength(200)]
        public string BookingMessage { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
        [MaxLength(200)]
        public string CancelationId { get; set; }
        [MaxLength(2000)]
        public string CancelationRemarks { get; set; }
    }

    public class tblFlightBookingSegment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Origin { get; set; }
        [Required]
        [MaxLength(20)]
        public string Destination { get; set; }
        public int TripIndicator { get; set; }
        public int SegmentDisplayOrder { get; set; }
        public enmCabinClass CabinClass { get; set; }
        [MaxLength(20)]
        public string ClassOfBooking { get; set; }
        public DateTime TravelDt { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        [MaxLength(200)]
        public string Airline { get; set; }
        [MaxLength(20)]
        public string AirlineCode { get; set; }
        [MaxLength(20)]
        public string FlightNumber { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
        [MaxLength(200)]
        public string ProviderResultIndex { get; set; }
        [MaxLength(200)]
        [ForeignKey("tblFlightBookingMaster")] // Foreign Key here
        public string TraceId { get; set; }
        public tblFlightBookingMaster tblFlightBookingMaster { get; set; }
        
    }

    public class tblFlightBookingPassengerDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Title { get; set; }
        [MaxLength(80)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(80)]
        public string LastName { get; set; }
        public enmPassengerType passengerType { get; set; }
        public DateTime? dob { get; set; }
        [MaxLength(30)]
        public string pNum { get; set; }
        public DateTime? PassportExpiryDate { get; set; }
        public DateTime? PassportIssueDate { get; set; }
        [MaxLength(200)]
        [ForeignKey("tblFlightBookingMaster")] // Foreign Key here
        public string TraceId { get; set; }
        public tblFlightBookingMaster tblFlightBookingMaster { get; set; }        
    }

    public class tblFlightBookingGSTDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(40)]
        public string gstNumber { get; set; }
        [MaxLength(200)]
        public string email { get; set; }
        [MaxLength(200)]
        public string registeredName { get; set; }
        [MaxLength(20)]
        public string mobile { get; set; }
        [MaxLength(200)]
        public string address { get; set; }
        [ForeignKey("tblFlightBookingMaster")] // Foreign Key here
        public string TraceId { get; set; }
        public tblFlightBookingMaster tblFlightBookingMaster { get; set; }
    }


    public class tblFlightBookingServices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public enmFlightBookingServiceType ServiceType { get; set; }
        public int SegmentDisplayOrder { get; set; }
        [MaxLength(200)]
        public string Code{ get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public double Amount { get; set; }
        [ForeignKey("tblFlightBookingPassengerDetails")] // Foreign Key here
        public int? PassengerId { get; set; }
        public tblFlightBookingPassengerDetails tblFlightBookingPassengerDetails { get; set; }
    }

    public class tblFlightBookingFareDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SegmentDisplayOrder { get; set; }
        [ForeignKey("tblFlightBookingMaster")] // Foreign Key here
        [MaxLength(200)]
        public string TraceId { get; set; }
        public tblFlightBookingMaster tblFlightBookingMaster { get; set; }
        public double WingAdultMarkup { get; set; }
        public double WingChildMarkup { get; set; }
        public double WingInfantMarkup { get; set; }
        public double WingTotalMarkup { get; set; }
        public double CustomerMarkup { get; set; }
        public double TotalFare { get; set; }
        public double Discount { get; set; }
        public double convenience { get; set; }        
        public double NetFare { get; set; }
        [NotMapped]
        public string SegmentName { get; set; }
        [NotMapped]
        public double AdultBaseFare { get; set; }
        [NotMapped]
        public double ChildBaseFare { get; set; }
        [NotMapped]
        public double InfantBaseFare { get; set; }

    }
    
    public class tblFlightBookingFarePurchaseDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SegmentDisplayOrder { get; set; }
        [ForeignKey("tblFlightBookingMaster")] // Foreign Key here
        [MaxLength(200)]
        public string TraceId { get; set; }
        public tblFlightBookingMaster tblFlightBookingMaster { get; set; }
        public enmServiceProvider Provider { get; set; }
        public double AdultBaseFare { get; set; }
        public double ChildBaseFare { get; set; }
        public double InfantBaseFare { get; set; }
        public double AdultTotalFare { get; set; }
        public double ChildTotalFare { get; set; }
        public double InfantTotalFare { get; set; }
        public double AdultNetFare { get; set; }
        public double ChildNetFare { get; set; }
        public double InfantNetFare { get; set; }
        public double TotalFare { get; set; }
        public double NetFare { get; set; }
    }

    public class tblFlightCancelation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SegmentDisplayOrder { get; set; }
        public string src { get; set; }
        public string dest { get; set; }
        public string date { get; set; }
        public string flightNumbers { get; set; }
        public string airlines { get; set; }
        public string fn { get; set; }
        public string ln { get; set; }
        public double amendmentCharges { get; set; }
        public double refundableamount { get; set; }
        public double totalFare { get; set; }
        public string bookingId { get; set; }
        public string amendmentId { get; set; }
        [ForeignKey("tblFlightBookingMaster")] // Foreign Key here
        [MaxLength(200)]
        public string TraceId { get; set; }
        public tblFlightBookingMaster tblFlightBookingMaster { get; set; }
        public DateTime CancelDate { get; set; }
        public string CancelRemarks { get; set; }

    }


}
