using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.DB
{
    public class d_CreatedBy
    {
        public ulong CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; } = DateTime.Now;
    }
    public class d_ModifiedBy : d_CreatedBy
    {
        [MaxLength(128)]
        public string ModifyRemarks { get; set; }
        public ulong? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public class d_RequestedBy
    {
        [MaxLength(128)]
        public string RequestedRemarks { get; set; }
        public ulong RequestedBy { get; set; }
        public DateTime? RequestedDt { get; set; } = DateTime.Now;
    }


    public class d_ApprovedBy : d_RequestedBy
    {
        public enmApprovalType ApprovalStatus { get; set; }
        [MaxLength(128)]
        public string ApprovalRemarks { get; set; }
        public ulong? ApprovedBy { get; set; }
        public DateTime? ApprovedDt { get; set; }
    }
}
