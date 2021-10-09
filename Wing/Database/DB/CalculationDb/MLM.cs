using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.DB.CalculationDb
{
    public class tblSaleSummaryTemp
    {
        public int MonthYear { get; set; }
        public ulong Nid { get; set; }
        public double PV { get; set; }
        public double GroupPV { get; set; }
        public double PlattoonPV { get; set; }
    }
    public class tblSaleSummaryDayDetailsTemp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int MonthYear { get; set; }
        public ulong Nid { get; set; }
        public ulong FromNid { get; set; }
        public double PV { get; set; }
    }

    public class tblFixedRankTemp
    {
        [Key]
        public ulong Nid { get; set; }
        public double PreviousLevelId { get; set; }
        public double CurrentLevelId { get; set; }
        public double DisplayLevelId { get; set; }
        public DateTime LastQualifyDt { get; set; }
    }
    public class tblFixedRankDetailTemp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public ulong Nid { get; set; }
        public double LevelId { get; set; }
        public DateTime QualifyDt { get; set; }
    }

    public class tblClubQualiferTemp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int ClubId { get; set; }
        public ulong Nid { get; set; }
        public int MonthYear { get; set; }
    }


    public class tblIncentiveDetailTemp
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



}
