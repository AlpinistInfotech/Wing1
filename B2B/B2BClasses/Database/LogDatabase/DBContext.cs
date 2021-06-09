using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;

namespace B2BClasses.Database.LogDatabase
{
   public class LogDBContext : DbContext
    {
        public LogDBContext(DbContextOptions<DBContext> options) : base(options)
        {
            
        }
        public DbSet<tblCustomerMasterLog> tblCustomerMasterLog { get; set; }

    }

    public class tblCustomerMasterLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        [MaxLength(10)]
        public string Code { get; set; }        
        [MaxLength(500)]
        public string CustomerName { get; set; }
        public string Logo { get; set; }
        [MaxLength(500)]
        public string Email { get; set; }
        public bool HaveGST { get; set; }
        [MaxLength(500)]
        public string Address { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        [MaxLength(20)]
        public string PinCode { get; set; }
        [MaxLength(20)]
        public string ContactNo { get; set; }
        [MaxLength(20)]
        public string AlternateNo { get; set; }
        public enmCustomerType CustomerType { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; } = DateTime.Now;
        public int ModifyBy { get; set; } = 1;
        public DateTime ModifyDt { get; set; } = DateTime.Now;
    }
}
