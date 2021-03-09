using Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WingGateway.Models
{
    public class mdlTcApprovalModel
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

    public class mdlTcBankWraper : mdlBank
    {
        public string TcName { get; set; }
        public string TcBankName { get; set; }
        public string ApproverName { get; set; }
    }

    public class mdl
}
