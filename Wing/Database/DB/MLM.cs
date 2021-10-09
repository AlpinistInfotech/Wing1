using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.DB
{

    public class tblProcessMaster
    {
        [Key]        
        public double ProcessId { get; set; }
        [MaxLength(32)]
        public string ProcessName { get; set; }
        public byte DisplayOrder { get; set; }
        public DateTime EffectiveFromDate { get; set; }
        public bool IsActive { get; set; }
    }
    public class tblProcessExecution
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public double ProcessId { get; set; }
        public int MonthYear { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now;
        public enmProcessStatus Status { get; set; }
    }
    public class tblProcessDependency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public double ProcessId { get; set; }
        public double DependentProcessId { get; set; }
    }
    public class tblSaleSummary
    { 
        public int MonthYear { get; set; }
        public ulong Nid{ get; set; }
        public double PV { get; set; }
        public double GroupPV { get; set; }
        public double PlattoonPV { get; set; }
    }
    public class tblSaleSummaryDayDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int MonthYear { get; set; }
        public ulong Nid { get; set; }
        public ulong FromNid { get; set; }
        public double PV { get; set; }
    }
    public class tblFixedRankMaster
    {
        [Key]
        public double LevelId { get; set; }
        [MaxLength(32)]
        public string ProcessName { get; set; }
        public double Percentage { get; set; }
    }
    public class tblFixedRank
    {
        [Key]
        public ulong Nid{ get; set; }
        public double PreviousLevelId { get; set; }
        public double CurrentLevelId { get; set; }
        public double DisplayLevelId { get; set; }
        public DateTime LastQualifyDt { get; set; }
    }
    public class tblFixedRankDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }        
        public ulong Nid { get; set; }
        public double LevelId { get; set; }        
        public DateTime QualifyDt { get; set; }
    }
    public class tblClubQualifer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int ClubId{ get; set; }
        public ulong Nid { get; set; }
        public int MonthYear { get; set; }
    }
   
    public class tblIncentiveMaster
    {
        [Key]        
        public byte IncentiveId { get; set; }
        [MaxLength(24)]
        public string Name { get; set; }        
        public byte DisplayOrder { get; set; }
    }
    public class tblIncentiveDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public byte IncentiveId { get; set; }
        public ulong Nid { get; set; }
        public ulong FromNid { get; set; }
        public ulong DepthLevel { get; set; }
        [MaxLength(24)]
        public string Description { get; set; }
        public double Credit { get; set; }
        public int MonthYear { get; set; }
        public double LevelId { get; set; }
    }
    public class tblRptSummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int MonthYear { get; set; }
        public byte IncentiveId { get; set; }
        public ulong Nid { get; set; }                
        public double Credit { get; set; }
        public double Debit { get; set; }
        public enmIncentiveTransactionType TransactionType { get; set; }
        public int TransactionId { get; set; }
        public short TypeId { get; set; }
        [MaxLength(256)]
        public string Remarks { get; set; }
    }

    public class tblEnableIncenitve
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int MonthYear { get; set; }
        public ulong Nid { get; set; }
        public bool IsEnable { get; set; }        
    }

    public class tblDispatchList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int MonthYear { get; set; }
        public ulong Nid { get; set; }
        public double PrevTaxable { get; set; }
        public double Incentive { get; set; }
        public double TaxableAmount { get; set; }
        public double Tds { get; set; }
        public double Net { get; set; }
        public int TransactionId { get; set; }
        public bool IsReturn { get; set; }
    }
    
    public class tblTdsDeduction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public ulong Nid { get; set; }
        public int TransactionId { get; set; }
    }

    public class tblFutureHoldIncentive :d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public ulong Nid { get; set; }
        public bool IsHold{ get; set; }
    }
    public class tblHoldIncentive : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HoldId { get; set; }
        public ulong Nid { get; set; }
        public double  HoldAmount { get; set; }
        public int MonthYear{ get; set; }        
    }
    public class tblReleaseIncentive : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HoldId { get; set; }
        public ulong Nid { get; set; }
        public double ReleaseAmount { get; set; }
        public int MonthYear { get; set; }
    }

}
