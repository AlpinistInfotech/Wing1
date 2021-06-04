using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace B2BClasses.Models
{
    public class mdlCustomer
    {
    }

    public class mdlCustomerMaster
    {
        
        public int Id { get; set; }
        [Required]
        [Display(Name = "Code")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Customer Name")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        [RegularExpression("[a-zA-Z0-9/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string CustomerName { get; set; }
        [Required]
        [MaxLength(500)]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Have GST")]
        public bool HaveGST { get; set; }
        [Required]
        [Display(Name = "Address")]
        [RegularExpression("[a-zA-Z0-9/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string Address { get; set; }
        [Display(Name = "Country")]
        public int? CountryId { get; set; }
        [Display(Name = "State")]
        public int? StateId { get; set; }
        [DataType(DataType.PostalCode)]
        [Display(Name = "Pincode")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string PinCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Contact No")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 10)]
        [Required]
        public string ContactNo { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Alternate No")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 10)]        
        public string AlternateNo { get; set; }

        [Display(Name = "Customer Type")]
        public enmCustomerType CustomerType { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
    }
}
