using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace B2BClasses.Database
{

   

    public class tblCustomerMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]        
        public string Code { get; set; }
        [Required]
        [MaxLength(500)]
        public string CustomerName{ get; set; }
        public string Logo { get; set; }

        [Required]
        [MaxLength(500)]
        public string Email { get; set; }        
        public bool HaveGST { get; set; }
        [MaxLength(500)]
        public string Address { get; set; }
        [ForeignKey("tblCountryMaster")]
        public int? CountryId { get; set; }
        public tblCountryMaster tblCountryMaster { get; set; }
        [ForeignKey("tblStateMaster")]
        public int? StateId { get; set; }
        public tblStateMaster tblStateMaster { get; set; }
        [MaxLength(20)]
        public string PinCode { get; set; }
        [MaxLength(20)]
        public string ContactNo{ get; set; }
        [MaxLength(20)]
        public string AlternateNo { get; set; }        
        public enmCustomerType CustomerType { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
    }

    public class tblCustomerBalence
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string  MPin { get; set; }
        public double WalletBalence { get; set; }
        public double CreditBalence { get; set; }
        public DateTime? ModifiedDt { get; set; }
        [ForeignKey("tblCustomerMaster")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }
    }

    public class tblCustomerGSTDetails
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
        [ForeignKey("tblCountryMaster")]
        public int? CountryId { get; set; }
        public tblCountryMaster tblCountryMaster { get; set; }
        [ForeignKey("tblStateMaster")]
        public int? StateId { get; set; }
        public tblStateMaster tblStateMaster { get; set; }
        [MaxLength(20)]
        public string PinCode { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        [ForeignKey("tblCustomerMaster")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }
    }

    public class tblUserMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
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
        [ForeignKey("tblCustomerMaster")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }

    }

    public class tblUserLoginLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblUserMaster")] // Foreign Key here
        public int? UserId { get; set; }
        public tblUserMaster tblUserMaster { get; set; }
        public DateTime LoginDt { get; set; }
        [MaxLength(100)]
        public string FromIP { get; set; }
        [MaxLength(100)]
        public string FromMachine { get; set; }
        [MaxLength(1000)]
        public string FromBrowser{ get; set; }
    }

    public class tblRoleMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; }
        public bool IsAdminRole { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        [InverseProperty("tblRoleMaster")]
        public ICollection<tblRoleClaim> tblRoleClaim { get; set; }
    }

    public class tblRoleClaim
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblRoleMaster")] // Foreign Key here
        public int? Role { get; set; }
        public tblRoleMaster tblRoleMaster { get; set; }
        public enmDocumentMaster ClaimId { get; set; }        
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public class tblUserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblUserMaster")] // Foreign Key here
        public int? UserId { get; set; }
        public tblUserMaster tblUserMaster { get; set; }
        [ForeignKey("tblRoleMaster")] // Foreign Key here
        public int? Role { get; set; }
        public tblRoleMaster tblRoleMaster { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public class tblCustomerIPFilter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblCustomerMaster")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }
        public bool AllowedAllIp { get; set; }
        //public bool AllowedAPI { get; set; }
        //public bool AllowedGUI { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }        
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        [InverseProperty("tblCustomerIPFilter")]
        public ICollection<tblCustomerIPFilterDetails> tblCustomerIPFilterDetails { get; set; }
    }

    public class tblCustomerIPFilterDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        [ForeignKey("tblCustomerIPFilter")] // Foreign Key here
        public int? FilterId { get; set; }
        public tblCustomerIPFilter tblCustomerIPFilter { get; set; }
        [Required]
        [MaxLength(100)]
        public string IPAddress { get; set; }
    }

    public class tblCustomerBankDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblCustomerMaster")]
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }
        [ForeignKey("tblBankMaster")]
        public int? BankId { get; set; }
        public tblBankMaster tblBankMaster { get; set; }
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
        public bool Isdeleted { get; set; }
    }

    public class tblCustomerPanDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblCustomerMaster")]
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }

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
        public bool Isdeleted { get; set; }
    }


    public class tblWalletBalanceAlert
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblCustomerMaster")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }
        public double MinBalance { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }



    public class tblWalletDetailLedger
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public DateTime TransactionDt { get; set; }
        [ForeignKey("tblCustomerMaster")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public enmTransactionType TransactionType { get; set; }
        [Required]
        [MaxLength(100)]
        public string TransactionDetails { get; set; }
        [Required]
        [MaxLength(200)]
        public string Remarks { get; set; }
        [ForeignKey("tblPaymentRequest")] // Foreign Key here
        public int? PaymentRequestId { get; set; }
        public tblPaymentRequest tblPaymentRequest { get; set; }
    }

    

    public class tblCustomerMarkup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblCustomerMaster")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }
        public double MarkupAmt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public class tblCustomerMarkupLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblCustomerMaster")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }
        public double MarkupAmt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? EffectiveFromDt { get; set; }

    }


    public class tblPaymentRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }   
        [ForeignKey("tblCustomerMaster")] // Foreign Key here
        public int? CustomerId { get; set; }
        public tblCustomerMaster tblCustomerMaster { get; set; }
        public double RequestedAmt { get; set; }
        public enmApprovalStatus Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        [MaxLength(200)]
        public string CreatedRemarks { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        [MaxLength(200)]
        public string ModifiedRemarks { get; set; }
        public enmRequestType RequestType { get; set; }
        public string UploadImages { get; set; }
    }
    
}
