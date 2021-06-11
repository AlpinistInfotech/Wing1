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

namespace B2bApplication.Models
{

    public class mdlCustomerMasterWraper: mdlCustomer
    {
        public int CustomerId { get; set; }
        [Required]
        [Display(Name = "Logo")]
        public IFormFile Logo { set; get; }
        public byte[] LogoData { set; get; }
        

        public Dictionary<int, string> CustomerMasterList { get; set; }

        public void LoadCustomer(ICustomerMaster cm)
        {
            CustomerMasterList = cm.FetchAllCustomer();
        }


        public void LoadData(int CustomerID, ICustomerMaster cm)
        {
            cm.CustomerId = CustomerID;
            if (CustomerID > 0)
            {
                this.customerMaster = cm.FetchBasicDetail();
                this.GSTDetails = cm.FetchGSTDetail();
                this.banks = cm.FetchBanks();
                this.pan = cm.FetchPan();
                this.AllUserList = cm.FetchUserMasters();
                //this.userMaster = this.AllUserList.Where(p => p.IsPrimary).FirstOrDefault();
                this.customerSetting = cm.FetchSetting();
            }
            else
            {
                this.customerMaster = cm.DocumentPermission.Any(p=>p== enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Read)? new mdlCustomerMaster():null;
                this.GSTDetails = cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_GSTDetail_Read) ? new mdlCustomerGSTDetails() : null;
                this.banks =cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Bank_Read) ? new mdlBanks() : null;
                this.pan =cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Pan_Read) ? new  mdlPan() : null;
                this.AllUserList = cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_UserDetail_Read) ? new List<mdlUserMaster>() : null;
                //this.userMaster = cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_UserDetail_Read) ? new mdlUserMaster() : null;
                this.customerSetting = cm.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Setting_Read) ? new mdlCustomerSetting() : null;

            }
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
        [MaxLength(150)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [MaxLength(10)]
        [Display(Name = "Mobile No.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid {0}")]
        public string MobileNo { get; set; }

        [MaxLength(10)]
        [Display(Name = "Alternate Mobile No.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid {0}")]
        public string AlternateMobileNo { get; set; }
        public bool Status { get; set; } = true;

        public int customerid { get; set; }
        public List<tblCustomerMaster> CustomerMasters { get; set; }

        
    }

    public class mdlAddCustomerUser
    {

        [Required]
        [MaxLength(10)]
        //[Remote(action: "CustomerCodeValidate", controller: "Customer", ErrorMessage = "Invalid Company Code")]
        [Display(Name = "Customer Code")]
        public string CustomerID { set; get; }

        //[Required]
        [MaxLength(50)]
        [Display(Name = "User Name")]
        public string UserName { set; get; }

       
       // [Required]
        [MaxLength(50)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool Status { get; set; } = true;

        public int userid { get; set; }
        public List<tblUserMaster> UserMasters { get; set; }
      
    }

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
        public List<ProcWalletSearch> mdlTcWalletWraper { get; set; }
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
        public string Passowrd { get; set; }

        [Required(ErrorMessage ="Enter New Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Re-Enter New Password")]
        public string ConfirmNewPassword { get; set; }
    }

    public class mdlPaymentRequest
    {

        [Required]
        [Display(Name = "Customer Code")]
        public int CustomerID { set; get; }

        [Required]
        [Range(0,1000000) ]
        [Display(Name = "Requested Amount")]
        public double CreditAmt { set; get; }

        [MaxLength(250)]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Status")]
        public enmApprovalStatus Status { set; get; }

        [Display(Name = "Request Type")]
        public enmRequestType RequestType { set; get; }

        [Display(Name = "Upload Document")]
        public List<IFormFile> UploadImages { set; get; }

        public List<byte[]> fileData { set; get; }

        public List<mdlPaymentRequestWraper> PaymentRequestList { get; set; }


    }

    public class mdlPaymentRequestWraper : tblPaymentRequest
    {
        public bool paymentrequestid { get; set; }
        public string CustomerName { get; set; }
        public string Code { get; set; }

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
        public void loadBookingData(IBooking _booking, int CustomerId)
        {
            _booking.CustomerId = CustomerId;
            FBMs = _booking.FlighBookReport(FromDt, ToDt, (DateFliterType == 2 ? false : true), false,false ,bookingStatus);
        }
        public string GetLabelClass(enmBookingStatus bookingStatus)
        {
            switch (bookingStatus)
            {
                case enmBookingStatus.Pending:return "label-info";
                case enmBookingStatus.Booked: return "label-success arrowed-in";
                case enmBookingStatus.Refund: return "label-danger";
                case enmBookingStatus.PartialBooked: return "label-warning arrowed-in";
                
            }
            return "label-inverse";
        }

    }

}


