using Database;
using Database.Classes;
using Microsoft.AspNetCore.Mvc;
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

        public bool RememberMe { get; set; }
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
        public byte[] CaptchaImage { get; set; }

        public void GenrateCaptcha(ICaptchaGenratorBase icgb)
        {
            var temp=icgb.getCaptcha();
            this.SaltId = temp.SaltId;
            this.CaptchaCode = temp.CaptchaCode;
            CaptchaImage =icgb.textToImageConversion(this.CaptchaCode);
            this.CaptchaCode = string.Empty;
        }
        public bool ValidateCaptcha(ICaptchaGenratorBase icgb)
        {
            return icgb.verifyCaptch(this.SaltId, this.CaptchaCode);
        }
    }


    public class mdlRegistration
    {
        
        [StringLength(10,ErrorMessage = "The {0} must be at {2} characters long.", MinimumLength = 10)]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "SponsorId")]
        [Remote(action: "IsSponsorValid", controller: "Account", ErrorMessage ="Invalid Sponsor")]
        public string SpTcId { get; set; }


        public int TcNId { get; set; }
        public string TcId { get; set; }
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
        [RegularExpression("[a-zA-Z/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string husband_father_name { get; set; }
        [Required]
        [MaxLength(30)]
        [Display(Name = "Phone No")]
        [Remote(action: "IsMobileInUse", controller: "Account")]
        [DataType(DataType.PhoneNumber)]        
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string PhoneNo { get; set; }

        [Required]        
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "IsEmailInUse",controller: "Account")]
        public string EmailAddress { get; set; }

        [Classes.CustomValidation.MinimumAge(18,ErrorMessage ="Age Should be 18 year")]
        [Required]
        [Display(Name = "DOB")]
        public DateTime? Dob{ get; set; }

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

        public bool GenrateRegistration(ISequenceMaster sequenceMaster,IConsolidatorProfile consolidatorProfile, DBContext context)
        {
            DateTime CurrentDatetime = DateTime.Now;
            //Validate Sponsor
            int? SpNid=null;
            List<int> AllSpNid = new List<int>();
            if (this.SpTcId != null && this.SpTcId.Length > 0)
            {
                SpNid = consolidatorProfile.GetNId(this.SpTcId ,true);
                if (SpNid == 0)
                {
                    throw new Exception("Invalid Sponsor Nid");
                }
                AllSpNid = consolidatorProfile.GetAllSpNid(SpNid.Value);
                AllSpNid.Add(SpNid.Value);
            }
            
            this.TcId=sequenceMaster.GenrateSequence(this.state_id);
            tblRegistration tr = new tblRegistration()
            {
                Id = TcId,
                FirstName = first_name,
                MiddleName = middle_name,
                LastName = last_name,
                Husband_father_name = husband_father_name,
                Gender = gender,
                Dob = this.Dob.Value,
                JoiningState = state_id,
                SpNid = SpNid,
                SpId = SpTcId,
                Isblock = false,
                IsTerminate = false,
                IsKycUpdated=enmIsKycUpdated.No,
                JoiningDt = CurrentDatetime,
                SpLegNumber = SpNid.HasValue?(consolidatorProfile.GetNidLegCount(SpNid.Value) + 1):1,
                TCRanks = enmTCRanks.Level1
            };
            context.tblRegistration.Add(tr);
            context.SaveChanges();
            TcNId = tr.Nid;
            context.tblTree.AddRange(AllSpNid.Select(p=>new tblTree { TcNid= TcNId, TcSpNid=p }).ToList());
            tblTcAddressDetail tblTcAddressDetail = new tblTcAddressDetail()
            {
                AddressType = enmAddressType.Permanent,
                address_line1 = this.address_line1,
                address_line2 = this.address_line2,
                StateId = this.state_id,
                CountryId = this.country_id,
                TcNid = TcNId,
                IsDeleted = false,
                CreatedBy = 0,
                CreatedDt = CurrentDatetime,
                ModifiedBy = 0,
                ModifiedDt = CurrentDatetime
            };
            context.tblTcAddressDetail.Add(tblTcAddressDetail);
            tblTcRanksDetails tblTcRanksDetails = new tblTcRanksDetails()
            {
                TCRanks = enmTCRanks.Level1,
                TcNid = TcNId,
                QualifyDate = CurrentDatetime,
                PPRequired = 0,
                PPDone = 0,
                Isdeleted = false
            };
            context.tblTcRanksDetails.Add(tblTcRanksDetails);
            context.SaveChanges();
            return true;


        }
    }
}
