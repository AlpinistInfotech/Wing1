using Database;
using Microsoft.AspNetCore.Http;
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
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public enmIdentityProof IdProofType { get; set; } = enmIdentityProof.Adhar;

        [Required]
        [Display(Name = "Document No")]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string DocumentNo { get; set; }

        [Required]
        [Display(Name = "Upload(ID)")]
        public List<IFormFile> IDDocumentUpload { set; get; }


        [Required]
        [Display(Name = "Address Type")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public enmIdentityProof AddressProofType { get; set; } = enmIdentityProof.Adhar;

        [Required]
        [Display(Name = "Address DocumentNo")]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string AddressDocumentNo { get; set; }

        [Required]
        [Display(Name = "Upload(Address)")]
        public List<IFormFile> AddressDocumentUpload { set; get; }

        [Required]
        [Display(Name = "Remarks")]
        public string Remarks{ set; get; }

        [Display(Name = "Is Approved")]
        public enmApprovalType IsApproved { get; set; } = enmApprovalType.Pending;

        [Display(Name = "Approved Dt")]
        public DateTime? ApprovedDt { get; set; }

        [Required]
        [Display(Name = "Approval Remarks")]
        public string ApprovalRemarks { set; get; }
    }
}
