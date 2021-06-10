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
        public DbSet<tblCustomerGSTDetailsLog> tblCustomerGSTDetailsLog { get; set; }
        public DbSet<tblCustomerBankDetailsLog> tblCustomerBankDetailsLog { get; set; }
        public DbSet<tblWalletBalanceAlertLog> tblWalletBalanceAlertLog { get; set; }
        public DbSet<tblCustomerMarkupLog> tblCustomerMarkupLog { get; set; }
        public DbSet<tblCustomerPanDetailsLog> tblCustomerPanDetailsLog { get; set; }
        public DbSet<tblCustomerIPFilterLog> tblCustomerIPFilterLog { get; set; }
        public DbSet<tblUserMasterLog> tblUserMasterLog { get; set; }   
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

    public class tblCustomerGSTDetailsLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string GstNumber { get; set; }
        [Required]
        [MaxLength(200)]
        public string Email { get; set; }
        [Required]
        [MaxLength(200)]
        public string RegisteredName { get; set; }
        [MaxLength(20)]
        public string Mobile { get; set; }
        [Required]
        [MaxLength(200)]
        public string Address { get; set; }
        
        public int? CountryId { get; set; }
        
        public int? StateId { get; set; }
        [MaxLength(20)]
        public string PinCode { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        public int? CustomerId { get; set; }        
    }

    public class tblCustomerBankDetailsLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public int? CustomerId { get; set; }
        public int? BankId { get; set; }        
        [MaxLength(20)]
        public string IFSC { get; set; }
        [MaxLength(20)]
        public string AccountNo { get; set; }
        [MaxLength(200)]
        public string BranchAddress { get; set; }

        [MaxLength(200)]
        public string NameasonBank { get; set; }
        [MaxLength(200)]
        public string UpiId { get; set; }
        [MaxLength(200)]
        public string UploadImages { get; set; }
        public enmApprovalStatus IsApproved { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public string Remarks { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDt { get; set; }
        public string ApprovalRemarks { get; set; }
    }


    public class tblCustomerPanDetailsLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        [MaxLength(100)]
        public string PANName { get; set; }
        [MaxLength(10)]
        public string PANNo { get; set; }
        [MaxLength(200)]
        public string UploadImages { get; set; }
        public enmApprovalStatus IsApproved { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public string Remarks { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDt { get; set; }
        public string ApprovalRemarks { get; set; }
    }


    public class tblWalletBalanceAlertLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public double MinBalance { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public class tblCustomerMarkupLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public int? CustomerId { get; set; }        
        public double MarkupAmt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }

    }

    public class tblCustomerIPFilterLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public bool AllowedAllIp { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        public string IPDetails { get; set; }

    }



    public class tblUserMasterLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool ForcePasswordChange { get; set; }
        [Required]
        [MaxLength(10)]
        public string Email { get; set; } = string.Empty;
        public bool IsMailVarified { get; set; }
        [MaxLength(1000)]
        public string MailVerficationTokken { get; set; }
        public DateTime TokkenExpiryTime { get; set; }
        public string Phone { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime BlockStartTime { get; set; }
        public DateTime BlockEndTime { get; set; }
        public int LoginFailCount { get; set; }
        public DateTime LastLogin { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }        
        public int? CustomerId { get; set; }
        public string Roles { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    
}
