using B2BClasses.Database;
using B2BClasses.Services.Air;
using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace B2BClasses.Models
{
    public class mdlPackageBook : mdlError
    {
        public int currentStep { get; set; }
        public string BookingId { get; set; }        
        public int PackageId { get; set; }        
        public int? CustomerId { get; set; }
        public enmBookingStatus BookingStatus { get; set; }
        public DateTime BookingDate { get; set; }
        public double AdultPrice { get; set; }
        public double ChildPrice { get; set; }
        public double InfantPrice { get; set; }
        public double AdultCount { get; set; }
        public double ChildCount { get; set; }
        public double InfantCount { get; set; }
        public double TotalAdultPrice { get; set; }
        public double TotalChildPrice { get; set; }
        public double TotalInfantPrice { get; set; }
        public double TotalPrice { get; set; }
        public double Discount { get; set; }
        public double NetPrice { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string Email { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 10)]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Password)]
        public string Mpin { get; set; }
        public tblPackageMaster packageData { get; set; }
        public List<mdlPackageBookingPassengerDetails> PassengerDetails { get; set; }
        public List<tblPackageBookingDiscussionDetails> tblPackageBookingDiscussionDetails { get; set; }
    }
    public class mdlPackageBookingPassengerDetails
    {
        public int Pid { get; set; }
        [Required]
        [Display(Name = "Title")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string Title { get; set; }
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }
        public enmPassengerType passengerType { get; set; }
        [Display(Name = "Birth Date")]
        public DateTime? dob { get; set; }
        [Display(Name = "Passport Number")]
        public string pNum { get; set; }
        public DateTime? PassportExpiryDate { get; set; }
        public DateTime? PassportIssueDate { get; set; }
    }

    



}
