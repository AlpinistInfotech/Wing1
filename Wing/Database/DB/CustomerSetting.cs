using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.DB
{
    public class tblCustomerIPFilter : d_ModifiedBy
    {
        [Key]
        public int CustomerId { get; set; }
        public bool AllowedAllIp { get; set; }
        [InverseProperty("tblCustomerIPFilter")]
        public ICollection<tblCustomerIPFilterDetails> tblCustomerIPFilterDetails { get; set; }
    }

    public class tblCustomerIPFilterDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblCustomerIPFilter")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerIPFilter tblCustomerIPFilter { get; set; }
        [Required]
        [MaxLength(100)]
        public string IPAddress { get; set; }
    }

    public class tblCustomerMarkup : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public enmBookingType BookingType { get; set; }
        public double MarkupAmount { get; set; }
        public DateTime EffectiveFromDt { get; set; } = DateTime.Now;
        public DateTime EffectiveToDt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }


}
