using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WingGateway.Models
{
  

    public class mdlKyc
    {
        [Required]
        [Display(Name = "Id Type")]        
        public enmIdentityProof IdProofType { get; set; } = enmIdentityProof.Adhar;

        [Required]
        [Display(Name = "Document No")]
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
        [Display(Name = "Bank")]
        public int BankId { get; set; }

        [StringLength(11, ErrorMessage = "The {0} must be at most {1} characters long.",MinimumLength =11)]
        [Required]
        [Display(Name = "IFSC Code")]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string IFSC { get; set; }

        [Required]
        [Display(Name = "Account No")]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [StringLength(20, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Remote(action: "IsAccountNoInUse", controller: "Home",AdditionalFields = "BankId")]
        public string AccountNo { get; set; }

        [Required]
        [Display(Name = "Upload Passbook/Cancelled Cheque")]
        public List<IFormFile> UploadImages { set; get; }

        [RegularExpression("[a-zA-Z0-9,/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Branch Address")]
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string BranchAddress { set; get; }


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
        [Required]
        [Display(Name = "PAN")]
        public int PANId { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at most {1} characters long.", MinimumLength = 3)]
        [Required]
        [Display(Name = "Name (as on PAN Card)")]
        public string PANName { get; set; }

        [Required]
        [Display(Name = "PAN No")]
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

        public List<tblPANMaster> GetPAN(DBContext context, bool OnlyActive)
        {
            if (OnlyActive)
            {
                return context.tblPANMaster.Where(p => p.IsActive).ToList();
            }
            else
            {
                return context.tblPANMaster.ToList();
            }

        }
    }

}
