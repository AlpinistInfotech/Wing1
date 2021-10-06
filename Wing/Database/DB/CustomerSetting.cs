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

    public class tblWalletBalanceAlert : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public double MinBalance { get; set; }        
    }


    public class tblCustomerWalletAmount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Sno { get; set; }
        public ulong UserId { get; set; }
        public int CustomerId { get; set; }
        public double WalletAmount { get; set; }
        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

    public class tblWalletDetailLedger
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public DateTime TransactionDt { get; set; }
        public int CustomerId { get; set; }
        public ulong UserId { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public enmTransactionType TransactionType { get; set; }
        [Required]
        [MaxLength(100)]
        public string TransactionDetails { get; set; }
        [Required]
        [MaxLength(200)]
        public string Remarks { get; set; }
        public ulong PaymentRequestId { get; set; }
        [NotMapped]
        public double Balance { get; set; }
    }


    public class tblPaymentRequest :d_ApprovedBy
    {
        [Key]        
        public ulong PaymentRequestId { get; set; }        
        public int? CustomerId { get; set; }
        public ulong UserId { get; set; }
        public double RequestedAmt { get; set; }
        public enmApprovalType Status { get; set; }
        [Required]
        [MaxLength(256)]
        public string TransactionNumber { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        public enmBankTransactionType TransactionType { get; set; }
        public bool IsDeleted { get; set; }
        public enmPaymentRequestType RequestType { get; set; }
        public string UploadImages { get; set; }
        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }


    public class tblCustomerNotification:d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int CustomerId { get; set; }
        public ulong UserId { get; set; }
        public enmNotificationType NotificationType {get; set;}
        public bool SendSms { get; set; }
        public bool SendEmail { get; set; }
        public bool SendDeviceNotification { get; set; }
    }

    public class tblCustomerFlightClassOfBooking:d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int CustomerId { get; set; }
        public bool EnableAllClasses{ get; set; }    
    }

    public class tblCustomerFlightClassOfBookingDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int CustomerId { get; set; }
        public int  BookingClassId{ get; set; }
    }

}
