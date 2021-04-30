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

    public class mdlApprovalForm
    {


        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
    }


    public class mdlDateFilter
    {
        [Required]
        [Display(Name = "FromDate")]
        [DataType(DataType.Date)]
        public DateTime FromDt { get; set; } = DateTime.Now.AddMonths(-1);

        [Required]
        [Display(Name = "ToDate")]
        [DataType(DataType.Date)]
        public DateTime ToDt { get; set; } = DateTime.Now.AddDays(1);

        [Display(Name = "Approval Type")]
        public enmApprovalType approvalType { get; set; } = enmApprovalType.Pending;

        public enmStatus StatusType { get; set; } = enmStatus.Active;
    }

    public class mdlIdFilter
    {
        [Display(Name = "TcId")]
        [StringLength(11, ErrorMessage = "The {0} must be at {2} characters long.", MinimumLength = 11)]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        
        [Remote(action: "IsSponsorValid", controller: "Account", ErrorMessage = "Invalid Id")]
        public string TcId { get; set; }
    }

    public class mdlFilterModel
    {   
        public mdlDateFilter dateFilter { get; set; }   
        public mdlIdFilter idFilter { get; set; }
        public bool IsReport { get; set; } = false;
    }

 
    public class mdlTcBankWraper : mdlBank
    {
        public int DetailId { get; set; }
        public string TcId { get; set; }
        public string TcName { get; set; }
        public string TcNameasonBank { get; set; }
        public string TcBankName { get; set; }
        public int ApprovedBy { get; set; }
        public string ApproverName { get; set; }
        public DateTime RequestDate { get; set; }
        public string UploadImageName { get; set; }
        
    }
    public class mdlTcBankReportWraper 
    {
        public mdlFilterModel FilterModel { get; set; }
        public List<mdlTcBankWraper> TcBankWrapers { get; set; }
    }
    
    
    
    public class mdlHolidayPackageReportWraper
    {
        public mdlFilterModel FilterModel { get; set; }
        public List<ProcHolidayPackageSearch> HolidayPackageWrapers { get; set; }
    }


    public class mdlHolidayPackageWraper : mdlHolidayPackage
    {
        public int DetailId { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }

        public bool Isdeleted { get; set; }

        public DateTime LastModifieddate { get; set; }
        public int lastModifiedBy { get; set; }

    }

    public class mdlTcReportWraper
    {
        public mdlFilterModel FilterModel { get; set; }
        public List<ProcRegistrationSearch> TcWrapers { get; set; }
    }


    public class mdlTcPANReportWraper
    {
        public mdlFilterModel FilterModel { get; set; }
        public List<mdlTcPANWraper> TcPANWrapers { get; set; }
    }

    public class mdlTcPANWraper : mdlPAN
    {
        public int DetailId { get; set; }
        public string TcId { get; set; }
        public string TcName { get; set; }
        public string TcPANNo { get; set; }

        public string TcNameasonPAN { get; set; }
        public int ApprovedBy { get; set; }
        public string ApproverName { get; set; }
        public DateTime RequestDate { get; set; }
        public string UploadImageName { get; set; }

    }


    public class mdlHolidayPackage
    {
        
        [Required]
        [Display(Name = "Package Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} characters long.", MinimumLength = 10)]
        public string PackageName { get; set; }

        [Required]
        [Display(Name = "Package Type")]
        public enmPackageCustomerType PackageType { get; set; }

        [Required]
        [Display(Name = "Package From")]
        public DateTime  PackageFromDate { get; set; }

        [Required]
        [Display(Name = "To")]
        public DateTime PackageToDate { get; set; }

        [Required]
        [Display(Name = "Price Range")]
        public int PriceFrom { get; set; }

        [Required]
        [Display(Name = "To")]
        public int PriceTo { get; set; }

        
        [Required]
        [Display(Name = "Member's Count")]
        public int MemberCount { get; set; }

        [Required]
        [Display(Name = "Day's Count")]
        public int DaysCount{ get; set; }


        [Required]
        [Display(Name = "Country")]
        public int country_id { get; set; }

        [Required]
        [Display(Name = "State")]
        public int state_id { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string PackageDescription { get; set; }

        [Display(Name = "Note (if any")]
        public string SpecialNote{ get; set; }


        [Required]
        [Display(Name = "Upload Package Image")]
        public List<IFormFile>  UploadPackageImage { get; set; }

        [Display(Name = "Upload Other's Image")]
        public List<IFormFile>  UploadOtherImage { get; set; }


        public List<byte[]> fileDataPackageImage { set; get; } = null;
        public List<byte[]> fileDataOtherImage { set; get; } = null;
        public int is_active { get; set; }

        public int created_by { get; set; }
        public DateTime created_datetime { get; set; }
    }


    public class mdlTcWalletReportWraper
    {
        [Required]
        [Display(Name = "FromDate")]
        [DataType(DataType.Date)]
        public DateTime FromDt { get; set; } = DateTime.Now.AddMonths(-1);

        [Required]
        [Display(Name = "ToDate")]
        [DataType(DataType.Date)]
        public DateTime ToDt { get; set; } = DateTime.Now.AddDays(1);
        
        [StringLength(11, ErrorMessage = "The {0} must be at {2} characters long.", MinimumLength = 11)]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Please enter TC ID")]
        [Remote(action: "IsSponsorValid", controller: "Account", ErrorMessage = "Invalid TC ID")]
        public string SpTcId { get; set; }
        public List<ProcWalletSearch> mdlTcWalletWraper  { get; set; }

        
    }
  
    public class mdlWallet 
    {

        [StringLength(11, ErrorMessage = "The {0} must be at {2} characters long.", MinimumLength = 11)]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Please enter TC ID")]
        [Remote(action: "IsSponsorValid", controller: "Account", ErrorMessage = "Invalid TC ID")]
        public string SpTcId { get; set; }

        [Required]
        [Display(Name = "Transaction Type")]
        public enmWalletTransactiontype TransactionType { get; set; }

        [Required]
        [Display(Name = "Wallet Amt")]
        public decimal WalletAmt { get; set; }

        
        [Display(Name = "Remarks")]
        [StringLength(200, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string Remarks { set; get; }

        
    }

}

