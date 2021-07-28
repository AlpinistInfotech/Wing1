using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace B2BClasses.Database
{
    public class tblBookingNumberMaster
    {
        [Key]
        public enmWingSearvices ServiceId { get; set; }
        [ConcurrencyCheck]
        public int BookingNumber { get; set; }
        [Required]
        [MaxLength(5)]
        public string  Prefix { get; set; }
        [Required]
        [MaxLength(4)]
        public string YearMonth { get; set; }
        public byte TotalDigit { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public class tblPackageMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PackageId { get; set; }
        [Required]
        [MaxLength(50)]
        public string PackageName { get; set; }
        [Required]
        [MaxLength(200)]
        public string LocationName { get; set; }
        public bool IsDomestic { get; set; }
        [Required]
        [MaxLength(500)]
        public string ShortDescription { get; set; }
        [Required]
        [MaxLength(4000)]
        public string LongDescription { get; set; }
        [Required]
        [MaxLength(200)]
        public string ThumbnailImage { get; set; }
        [Required]
        [MaxLength(200)]
        public string AllImage { get; set; }      
        public int NumberOfDay { get; set; }
        public int NumberOfNight { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public double AdultPrice { get; set; }
        public double ChildPrice { get; set; }
        public double InfantPrice { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        [NotMapped]
        public string ModifiedByName { get; set; }
    }


    public class tblPackageRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pid { get; set; }
        [ForeignKey("tblPackageMaster")] // Foreign Key here
        public int? PackageId { get; set; }
        public tblPackageMaster tblPackageMaster { get; set; }
        [MaxLength(200)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string ContactNo { get; set; }
        public byte Rating { get; set; }
        [MaxLength(2000)]
        public string Remarks { get; set; }
    }

    public class tblPackageBooking
    {
        [Key]
        public string BookingId { get; set; }
        [ForeignKey("tblPackageMaster")] // Foreign Key here
        public int? PackageId { get; set; }
        public tblPackageMaster tblPackageMaster { get; set; }
        [ForeignKey("tblCustomerMaster")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }
        public double AdultPrice { get; set; }
        public double ChildPrice { get; set; }
        public double InfantPrice { get; set; }
        public double TotalPrice { get; set; }
        public double Discount { get; set; }
        public double NetPrice { get; set; }
        public string Email { get; set; }
        public string ContactNo{ get; set; }

        public enmBookingStatus BookingStatus { get; set; }
        public DateTime BookingDate { get; set; }
        [InverseProperty("tblPackageBooking")]
        public ICollection<tblPackageBookingPassengerDetails> tblPackageBookingPassengerDetails { get; set; }
        public ICollection<tblPackageBookingDiscussionDetails> tblPackageBookingDiscussionDetails { get; set; }

    }
    public class tblPackageBookingPassengerDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pid { get; set; }
        [ForeignKey("tblPackageBooking")] // Foreign Key here
        public string BookingId { get; set; }
        public tblPackageBooking tblPackageBooking { get; set; }
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

    }
    public class tblPackageBookingDiscussionDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Discussionid { get; set; }
        public int UserId { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        [ForeignKey("tblPackageBooking")] // Foreign Key here
        public string BookingId { get; set; }
        public tblPackageBooking tblPackageBooking { get; set; }
    }
}
