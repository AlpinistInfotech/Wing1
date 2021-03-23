using Database;
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
    
    
    public class mdlTcBankApprovalWraper : mdlBank
    {
        public mdlApprovalForm approval { get; set; }
        public bool IsReport { get; set; } = true;

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


}
