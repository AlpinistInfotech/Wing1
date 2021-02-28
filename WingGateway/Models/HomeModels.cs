using Database;
using Database.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WingGateway.Models
{
    public class mdlLogin
    {
        [Required]
        [Display(Name = "UserName")]        
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Username{ get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Password { get; set; }
        [Required]
        public mdlCaptcha CaptchaData { get; set; }
    }

    public class mdlCaptcha
    {
        [Required]
        [Display(Name = "Captcha")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "This Captcha must be 4 characters")]
        public string CaptchaCode { get; set; }
        [Required]
        [Display(Name = "Salt")]
        public string SaltId { get; set; }
        public string CaptchaImage { get; set; }

        public void GenrateCaptcha(ICaptchaGenratorBase icgb)
        {
            var temp=icgb.getCaptcha();
            this.SaltId = temp.SaltId;
            this.CaptchaCode = temp.CaptchaCode;
            CaptchaImage =Convert.ToString( icgb.textToImageConversion(this.CaptchaCode));
        }
        public bool ValidateCaptcha(ICaptchaGenratorBase icgb)
        {
            return icgb.verifyCaptch(this.SaltId, this.CaptchaCode);
        }
    }


    public class mdlRegistration
    {
        [Required]
        [Display(Name = "Gender")]
        public enmGender gender { get; set; }
        [Required]
        [MaxLength(30)]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        [RegularExpression("[a-zA-Z]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string first_name { get; set; }
        [MaxLength(30)]
        [StringLength(30, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Middle Name")]
        [RegularExpression("[a-zA-Z]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string middle_name { get; set; } = string.Empty;
        [Required]
        [MaxLength(30)]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        [RegularExpression("[a-zA-Z]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string last_name { get; set; }

        [Required]
        
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Husband/Father Name")]
        [RegularExpression("[a-zA-Z]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string husband_father_name { get; set; }
        [Required]
        [MaxLength(30)]
        [Display(Name = "Phone No")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNo { get; set; }
        [Required]
        
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "DOB")]
        public DateTime Dob{ get; set; }

        [Required]
        [Display(Name = "Address Line1")]        
        [StringLength(200, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string address_line1 { get; set; }

        [Display(Name = "Address Line2")]        
        [StringLength(200, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string address_line2 { get; set; }

        [Required]
        [Display(Name = "State")]
        public int state_id { get; set; }

        [Required]
        [Display(Name = "Country")]
        public int country_id { get; set; }

        [Required]
        [Display(Name = "Pincode")]
        [MaxLength(20)]
        [StringLength(20, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [DataType(DataType.PostalCode)]
        public string Pincode { get; set; }

        [Required]
        public mdlCaptcha CaptchaData { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="Password & confirm Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
