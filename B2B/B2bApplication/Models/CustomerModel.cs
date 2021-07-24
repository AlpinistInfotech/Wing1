using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Services.Air;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2BClasses.Services.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Database;
using Microsoft.AspNetCore.Http;
using B2BClasses.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace B2bApplication.Models
{

    public class mdlCustomerMasterWraper: mdlCustomer
    {

        [Display(Name = "Customer Name")]

        public int CustomerId { get; set; }        
        
        [Display(Name = "Logo")]
        public IFormFile Logo { set; get; }
        public byte[] LogoData { set; get; }
        
        public Dictionary<int, string> CustomerMasterList { get; set; }
        public List<enmDocumentMaster> DocumentPermission { get; set; }
        public double WalletBalance { get; set; }
        public double CreditBalace { get; set; }
        [StringLength(4, ErrorMessage = "The {0} must be {1} characters long.", MinimumLength = 4)]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "MPin")]
        [DataType(DataType.Password)]
        public string NewMpin { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm MPin")]
        [Compare(nameof(NewMpin))]
        public string ConfirmNewMpin { get; set; }




        public void LoadCustomer(ICustomerMaster cm)
        {
            CustomerMasterList = cm.FetchAllCustomer(IncludeAdmin:true,OnlyActive:false);
        }

        public void SetWalletBalance(DBContext _context) {
            var defaultBalance = _context.tblCustomerBalance.Where(p => p.CustomerId == CustomerId).FirstOrDefault();
            if (defaultBalance == null)
            {
                _context.tblCustomerBalance.Add(new tblCustomerBalance() { CustomerId = CustomerId, CreditBalance = 0, ModifiedDt = DateTime.Now, MPin = Settings.Encrypt( "0000"), WalletBalance = 0 });
                _context.SaveChanges();
                this.WalletBalance = 0;
                this.CreditBalace = 0;
            }
            else
            {
                this.WalletBalance = defaultBalance.WalletBalance;
                this.CreditBalace = defaultBalance.CreditBalance;
            }
        }

        

        public void LoadData(int CustomerID, ICustomerMaster cm, IConfiguration config)
        {
            this.CustomerId= CustomerID;
            cm.CustomerId = CustomerID;

            this.DocumentPermission = cm.DocumentPermission;
            if (CustomerID > 0)
            {
                this.customerMaster = cm.FetchBasicDetail();
                this.GSTDetails = cm.FetchGSTDetail();
                this.banks = cm.FetchBanks();
                this.pan = cm.FetchPan();
                this.AllUserList = cm.FetchUserMasters();
                this.userMaster = this.AllUserList.Where(p => p.IsPrimary).FirstOrDefault();
                this.customerSetting = cm.FetchSetting();
                
            }

            if (cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Read) && this.customerMaster == null)
            {
                this.customerMaster = new mdlCustomerMaster();
            }
            if (cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Read) && this.GSTDetails == null)
            {
                this.GSTDetails = new mdlCustomerGSTDetails();
            }
            if (cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Read) && this.banks == null)
            {
                this.banks = new mdlBanks();
            }
            if (cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Read) && this.pan == null)
            {
                this.pan = new mdlPan();
            }
            if (cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Read) && this.AllUserList == null)
            {
                this.AllUserList = new List<mdlUserMaster>();
            }
            if (cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Read) && this.userMaster == null)
            {
                this.userMaster = new mdlUserMaster();
            }
            if (cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Read) && this.customerSetting == null)
            {
                this.customerSetting = new mdlCustomerSetting();
            }
            this.ConfirmNewMpin=this.NewMpin = this.customerSetting.MPin;

            if (string.IsNullOrWhiteSpace(this.customerMaster.Logo))
            {
                string DefaultImage = config["Organisation:DefaultIcon"];
                var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + DefaultImage);
                LogoData = System.IO.File.ReadAllBytes(path);
            }
            else
            {
                string DefaultPath = config["Organisation:IconPath"];
                var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + DefaultPath);
                LogoData = System.IO.File.ReadAllBytes(string.Concat(path, customerMaster.Logo) );
            }

        }


        public void SetCountryState(dynamic ViewBag, DBContext context)
        {   
            int CountryId = customerMaster?.CountryId ?? 0;
            var AllStateLIst = context.tblStateMaster.Where(p => p.CountryId == CountryId && p.IsActive).Select(p => new { p.StateId, p.StateName }).OrderBy(p => p.StateName);
            SelectList CountryList = new SelectList(context.tblCountryMaster.Where(p=>p.IsActive).Select(p=>new {p.CountryId,p.CountryName}).OrderBy(p=>p.CountryName) , "CountryId", "CountryName", CountryId);            
            if (CountryId > 0)
            {  
                SelectList StateList = new SelectList(AllStateLIst, "StateId", "StateName", customerMaster?.StateId ?? 0);
                SelectList GStStateList = new SelectList(AllStateLIst, "StateId", "StateName", GSTDetails?.StateId ?? 0);                
                ViewBag.StateList = StateList;
                ViewBag.GSTStateList = GStStateList;
            }
            ViewBag.CountryList = CountryList;
            var BankData = context.tblBankMaster.Where(p => p.IsActive).Select(p => new { p.BankId, p.BankName });
            SelectList BankList = new SelectList(BankData, "BankId", "BankName", banks?.BankId ?? 0);
            ViewBag.BankList = BankList;
        }

    }


    public class mdlAddCustomer
    {
        [Display(Name = "Customer Type")]
        public enmCustomerType customerType { get; set; }


        [Required]
        [MaxLength(5)]
        [Display(Name = "Customer Code")]
        public string CustomerCode { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Customer Name")]
        public string CustomerName { set; get; }

        [MaxLength(80)]
        [Display(Name = "Email ID")]
        [DataType(DataType.EmailAddress)]
        public string Email{ set; get; }

        [Required]
        [MaxLength(10)]
        [Display(Name = "Mobile No.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid {0}")]
        public string MobileNo { get; set; }


        [Required]
        [MaxLength(150)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        
        [MaxLength(10)]
        [Display(Name = "Alternate Mobile No.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid {0}")]
        public string AlternateMobileNo { get; set; }
        public bool Status { get; set; } = true;

        public int customerid { get; set; }
        public List<tblCustomerMaster> CustomerMasters { get; set; }

        
    }

    //public class mdlAddCustomerUser : mdlUserMaster
    //{

    //    [MaxLength(80)]
    //    [DefaultValue("")]
    //    [Display(Name = "Email ID")]
    //    [DataType(DataType.EmailAddress)]
    //    public string Email { set; get; }


    //    [MaxLength(10)]
    //    [DefaultValue("")]
    //    [Display(Name = "Mobile No.")]
    //    [DataType(DataType.PhoneNumber)]
    //    [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid {0}")]
    //    public string MobileNo { get; set; }

    //    public bool Status { get; set; } = true;

    //    public bool ForcePasswordChange { get; set; } = true;

    //    public int userid { get; set; }

    //    [Display(Name = "Role")]
    //    public bool IsAllRole { get; set; } = true;

    //    public List<int> RoleId { get; set; } 

    //    public MultiSelectList _RoleMaster { get; set; }


    //    public List<tblUserMaster> UserMasters { get; set; }
      
    //}

    public class mdlCustomerMarkup
    {
        [Required]
        [Display(Name = "Customer Type")]
        public string CustomerID { set; get; }
        
        [Required]
        [Range(0, 10000)]
        [Display(Name = "MarkUp Value")]
        public double MarkupValue { get; set; }

        public int markupid { get; set; }

        public List<tblCustomerMaster> CustomerMasters { get; set; }

        public List<tblCustomerMarkup> MarkupData{ get; set; }
    }

    public class mdlCustomerWallet
    {
        [Required]
        [Display(Name = "Customer Type")]
        public string CustomerID { set; get; }

        [Required]
        [Display(Name = "Transaction Type")]
        public enmCreditDebit creditDebit { set; get; }


        [Display(Name = "Transaction Details")]
        public string TransactionDetails { set; get; }

        [Display(Name = "Remarks")]
        public string Remarks { set; get; }

        [Required]
        [Range(0, 100000)]
        [Display(Name = "Wallet Amt")]
        public double WalletAmt { get; set; }

        [Required]
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

    }

    public class mdlCustomerWalletReport
    {
        [Required]
        [Display(Name = "FromDate")]
        [DataType(DataType.Date)]
        public DateTime FromDt { get; set; } = DateTime.Now.AddMonths(-1);

        [Required]
        [Display(Name = "ToDate")]
        [DataType(DataType.Date)]
        public DateTime ToDt { get; set; } = DateTime.Now.AddDays(1);

        [Required]
        [Display(Name = "Customer Type")]
        public string CustomerID { set; get; }
        public List<tblWalletDetailLedger> mdlTcWalletWraper { get; set; }
    }

    public class mdlCustomerIPFilter
    {
        [Required]
        [Display(Name = "Customer Type")]
        public int CustomerID { set; get; }

        [Display(Name = "All IP Applicable")]
        public bool allipapplicable { set; get; }

        [Display(Name = "IP Address")]
        public string IPAddess { set; get; } = "";

        public int IPFilterId { set; get; }

        public List<tblCustomerIPFilter> IPFilterReport { get; set; }


    }

    public class mdlCustomerChangePassword
    {

        [Required(ErrorMessage ="Enter Old Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Enter New Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Confirm New Password")]
        [Compare(nameof(NewPassword), ErrorMessage = "Password & confirm Password do not match")]
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }

    }

    public class mdlPaymentRequest
    {

        [Required]
        [Display(Name = "Customer Code")]
        public int CustomerID { set; get; }
        
        [Required]
        [Range(1,1000000)]
        [Display(Name = "Requested Amount")]
        
        public double CreditAmt { set; get; }

        [System.ComponentModel.DefaultValue("")]
        [StringLength(250, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; } = "";
        
        [Display(Name = "Status")]
        public enmApprovalStatus Status { set; get; }
        
        [Display(Name = "Request Type")]
        public enmRequestType RequestType { set; get; }
        
        [Display(Name = "Transaction Type")]
        public enmBankTransactionType BankTransactionType { set; get; } = enmBankTransactionType.None;
        
        //[Required(ErrorMessage = "Please Select Document")]
        [Display(Name = "Upload Document")]
        public List<IFormFile> UploadImages { set; get; }
        public List<byte[]> fileData { set; get; }
        public List<mdlPaymentRequestWraper> PaymentRequestList { get; set; }

        [Display(Name = "Transaction No")]
        public string TransactionNumber { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
        public List<mdlPaymentWraper> PaymentWraper { get; set; }

    }

    public class mdlPaymentRequestWraper : tblPaymentRequest
    {
        public bool paymentrequestid { get; set; }
        public string CustomerName { get; set; }
        public string Code { get; set; }

    }

    public class mdlPaymentWraper : tblPaymentRequest
    {
    }

    public class mdlFlightBookingReport
    {
        [Display(Name = "From Dt")]
        public DateTime FromDt { get; set; } = DateTime.Now.AddDays(-2);
        [Display(Name = "To Dt")]
        public DateTime ToDt { get; set; } = DateTime.Now;
        [Display(Name = "Status")]
        public enmBookingStatus bookingStatus { get; set; } = enmBookingStatus.All;
        [Display(Name = "Filter By")]
        public int DateFliterType { get; set; } = 1; //1 = on Booking Date, 2 on Travel Date
        public List<tblFlightBookingMaster> FBMs { get; set; } = new List<tblFlightBookingMaster>();
        public void loadBookingData(IBooking _booking, int CustomerId,ICurrentUsers currentUser)
        {
            _booking.CustomerId = CustomerId;
            FBMs = _booking.FlighBookReport(FromDt, ToDt, (DateFliterType == 2 ? false : true), currentUser.CustomerType== enmCustomerType.Admin?true: false,false ,bookingStatus);
        }
        public string GetLabelClass(enmBookingStatus bookingStatus)
        {
            switch (bookingStatus)
            {
                case enmBookingStatus.Pending:return "label-info";
                case enmBookingStatus.Cancel: return "label-danger arrowed-in";
                case enmBookingStatus.Booked: return "label-success arrowed-in";
                case enmBookingStatus.Refund: return "label-danger";
                case enmBookingStatus.PartialBooked: return "label-warning arrowed-in";                
            }
            return "label-inverse";
        }

    }
    public class mdlPaymentReport
    {
        [Display(Name = "From Dt")]
        public DateTime FromDt { get; set; } = DateTime.Now.AddDays(-2);
        [Display(Name = "To Dt")]
        public DateTime ToDt { get; set; } = DateTime.Now;
        [Display(Name = "Status")]
    }

}


