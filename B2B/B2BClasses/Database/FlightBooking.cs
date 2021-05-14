using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace B2BClasses.Database
{
    public class tblFlightBookingMaster
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int CustomerId { get; set; }
        public int AdultCount { get; set; } = 1;
        public int ChildCount { get; set; } = 0;
        public int InfantCount { get; set; } = 0;
        public bool DirectFlight { get; set; }
        public enmJourneyType JourneyType { get; set; } = enmJourneyType.OneWay;                        
        public enmBookingStatus BookingStatus { get; set; } = enmBookingStatus.Pending;
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        [InverseProperty("tblFlightBookingMaster")]
        public ICollection<tblFlightBookingSegment> tblFlightBookingSegments { get; set; }
    }

    public class tblFlightBookingSegment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public enmCabinClass CabinClass { get; set; }
        public string ClassOfBooking { get; set; }
        public DateTime TravelDt { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }        
        public string Airline { get; set; }
        public string AirlineCode { get; set; }
        public string FlightNumber { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
        public string ProviderTraceId{ get; set; }
        public string ProviderResultIndex { get; set; }
        [ForeignKey("tblFlightBookingMaster")] // Foreign Key here
        public string TraceId { get; set; }
        public tblFlightBookingMaster tblFlightBookingMaster { get; set; }
    }

    public class tblFlightBookingPassengerDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public enmPassengerType passengerType { get; set; }
        public DateTime? dob { get; set; }
        public string pNum { get; set; }
        public DateTime? PassportExpiryDate { get; set; }
        public DateTime? PassportIssueDate { get; set; }
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
        public string key { get; set; }
        public string value { get; set; }
        [ForeignKey("tblFlightBookingPassengerDetails")] // Foreign Key here
        public int? PassengerId { get; set; }
        public tblFlightBookingPassengerDetails tblFlightBookingPassengerDetails { get; set; }
    }

    public class tblFlightBookingFareDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblFlightBookingSegment")] // Foreign Key here
        public int? SegmentId { get; set; }
        public tblFlightBookingSegment tblFlightBookingSegment { get; set; }
        public double WingAdultMarkup { get; set; }
        public double WingChildMarkup { get; set; }
        public double WingInfantMarkup { get; set; }
        public double WingTotalMarkup { get; set; }
        public double CustomerMarkup { get; set; }
        public double TotalFare { get; set; }
        public double convenience { get; set; }        
        public double NetFare { get; set; }
    }
    
    public class tblFlightBookingFarePurchaseDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblFlightBookingSegment")] // Foreign Key here
        public int? SegmentId { get; set; }
        public tblFlightBookingSegment tblFlightBookingSegment { get; set; }
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


}
