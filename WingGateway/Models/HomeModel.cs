using Database;
using Database.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WingGateway.Models
{
    public class mdlTreeWraper: mdlTree
    {
        public int id { get; set; }
        public string text { get; set; }
        public List<mdlTreeWraper> children { get; set; }
        public string icon { get; set; }

    }

    public class mdlKyc
    {
        [Required]
        [Display(Name = "Id Proof Type")]        
        public enmIdentityProof IdProofType { get; set; } = enmIdentityProof.Aadhar;

        [Required]
        [Display(Name = "Document No.")]
        [StringLength(50, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string DocumentNo { get; set; }

        [Required]
        [Display(Name = "Upload(ID)")]
        public List<IFormFile> IDDocumentUpload { set; get; }

        [StringLength(200, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Remarks")]
        public string Remarks{ set; get; }

        [Display(Name = "Is Approved")]
        public enmApprovalType? IsApproved { get; set; } = null;

        [Display(Name = "Approved Dt")]
        public DateTime? ApprovedDt { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Approval Remarks")]
        public string ApprovalRemarks { set; get; }
        public List<byte[]> fileData { set; get; }       
    }


    public class mdlBank
    {   

        [Required]
        [Display(Name = "Select Bank Name")]
        public int BankId { get; set; }
        [StringLength(11, ErrorMessage = "The {0} must be at most {1} characters long.",MinimumLength =11)]
        [Required]
        [Display(Name = "IFSC Code")]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string IFSC { get; set; }

        [Required]
        [Display(Name = "Account No.")]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [StringLength(20, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Remote(action: "IsAccountNoInUse", controller: "Home",AdditionalFields = "BankId")]
        public string AccountNo { get; set; }
        
        [Required]
        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Name (as on Bank)")]
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string NameasonBank { set; get; }


        [Required]
        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Branch Address")]
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string BranchAddress { set; get; }


        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Remarks")]
        [StringLength(200, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string Remarks { set; get; }

        [Required]
        [Display(Name = "Upload Passbook/Cancelled Cheque")]
        public List<IFormFile> UploadImages { set; get; }


        [Display(Name = "Is Approved")]
        public enmApprovalType? IsApproved { get; set; } = null;

        [Display(Name = "Approved Date")]
        public DateTime? ApprovedDt { get; set; }


        [StringLength(200, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Approval Remarks")]
        public string ApprovalRemarks { set; get; }

        public List<byte[]> fileData { set; get; }
            
        public List<tblBankMaster> GetBanks(DBContext context, bool OnlyActive)
        {
            if (OnlyActive)
            {
                return context.tblBankMaster.Where(p => p.IsActive).ToList();
            }
            else
            {
                return context.tblBankMaster.ToList();
            }
            
        }
    }


    public class mdlPAN
    {
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} characters long.", MinimumLength = 3)]
        [Required]
        [Display(Name = "Name (as on PAN Card)")]
        public string PanName { get; set; }

        [Required]
        [Display(Name = "PAN No.")]
        [StringLength(10, ErrorMessage = "The {0} must be at most {1} characters long.",MinimumLength =10)]
        [Remote(action: "IsPANNoInUse", controller: "Home", AdditionalFields = "PANId")]
        public string PANNo { get; set; }

        [Required]
        [Display(Name = "Upload PAN Card")]
        public List<IFormFile> UploadImages { set; get; }

        

        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Remarks")]
        [StringLength(200, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string Remarks { set; get; }

        [Display(Name = "Is Approved")]
        public enmApprovalType? IsApproved { get; set; } = null;

        [Display(Name = "Approved Date")]
        public DateTime? ApprovedDt { get; set; }


        [StringLength(20, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Approval Remarks")]
        public string ApprovalRemarks { set; get; }

        public List<byte[]> fileData { set; get; }

        
    }

    public class mdlNominee
    {


        [Required]
        [Display(Name = "Nominee Name")]
        public string NomineeName { get; set; }


        [Required]
        [Display(Name = "Nominee Relation")]
        public enmNomineeRelation NomineeRelation { get; set; } 

        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Remarks")]
        [StringLength(200, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string Remarks { set; get; }

        
        
    }

    public class mdlContact
    {


        [Required]
        [MaxLength(10)]
        [Display(Name = "Permanent Mobile No.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid {0}")]
        [Remote(action: "ValidateContactNo", controller: "Home")]
        public string MobileNo { get; set; }

        [Required]
        [MaxLength(6)]
        [Display(Name = "OTP")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid {0}")]
        public string OTP { get; set; }


        [Display(Name = "Alternate Mobile No.")]
        [MaxLength(10)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid {0}")]
        public string AlternateMobileNo { get; set; }

        
    }


    public class mdlEmail
    {


        [Required]
        [MaxLength(80)]
        [Display(Name = "Permanent Email ID")]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmailID", controller: "Home")]
        public string EmailID { get; set; }

        

        [Display(Name = "Alternate Email ID")]
        [MaxLength(80)]
        [DataType(DataType.EmailAddress)]
        public string AlternateEmailID { get; set; }


    }

    public class mdlMarkUp
    {


        [Display(Name = "Booking Type")]
        public enmBookingType BookingType { get; set; }


        [Required]
        [Range(0,10000)]
        [Display(Name = "MarkUp Value")]
        public decimal markupValue { get; set; }

        public List<mdlTcMarkUpWraper> TcMarkUpWrapers { get; set; }

    }

    public class mdlTcMarkUpWraper  
    {
        public int DetailId { get; set; }
        public string TcId { get; set; }
        public enmBookingType BookingType { get; set; }
        public decimal MarkUpValue { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }



    public class mdlAddress
    {


        [Required]
        [MaxLength(100)]
        [Display(Name = "Address Line1")]
        public string Address1 { get; set; }

        [MaxLength(100)]
        [Display(Name = "Address Line2")]
        public string Address2 { get; set; }

        [MaxLength(100)]
        [Display(Name = "LandMark")]
        public string Landmark { get; set; }


        [Required]
        [Display(Name = "Country")]
        public int country_id { get; set; }


        [Required]
        [Display(Name = "State")]
        public int state_id { get; set; }

        [Required]
        [Display(Name = "City")]
        public int city_id { get; set; }


        [Required]
        [Display(Name = "Pincode")]
        [MaxLength(20)]
        [StringLength(20, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [DataType(DataType.PostalCode)]
        public string Pincode { get; set; }

    }

}
