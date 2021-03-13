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


        [StringLength(20, ErrorMessage = "The {0} must be at most {1} characters long.")]
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

}
