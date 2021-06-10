using B2BClasses.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace B2BClasses.Models
{

    public interface ImdlCustomer : ImdlCustomerMaster, ImdlCustomerGSTDetails, ImdlBanks, ImdlPan, ImdlCustomerSetting
    {
        public List<ImdlUserMaster> UserMaster { get; set; }
    }

    public class mdlCustomer : ImdlCustomer
    {
        public List<ImdlUserMaster> UserMaster {get; set;}
        public string Address {get; set;}
        public string AlternateNo {get; set;}
        public string Code {get; set;}
        public string ContactNo {get; set;}
        public int? CountryId {get; set;}
        public int CreatedBy {get; set;}
        public string CreatedByName {get; set;}
        public DateTime CreatedDt {get; set;}
        public int ModifyBy { get; set; }
        public string ModifyByName { get; set; }
        public DateTime ModifyDt { get; set; }
        public int CustomerId {get; set;}
        public string CustomerName {get; set;}
        public enmCustomerType CustomerType {get; set;}
        public string Email {get; set;}
        public bool HaveGST {get; set;}
        public bool IsActive {get; set;}
        public string PinCode {get; set;}
        public int? StateId {get; set;}        
        public string GstNumber {get; set;}
        public string Mobile {get; set;}
        public string RegisteredName {get; set;}
        public string AccountNo {get; set;}
        public string ApprovalRemarks {get; set;}
        public int BankId {get; set;}
        public string BranchAddress {get; set;}
        public string IFSC {get; set;}
        public enmApprovalStatus IsApproved {get; set;}
        public string NameasonBank {get; set;}
        public string Remarks {get; set;}
        public string UpiId {get; set;}
        public string PANName {get; set;}
        public string PANNo {get; set;}
        public bool AllowedAllIp {get; set;}
        public string IPAddess {get; set;}
        public double MarkupAmount {get; set;}
        public double MinBalance {get; set;}
        
       
    }

    
    public interface ImdlCustomerMaster
    {
        string Address { get; set; }
        string AlternateNo { get; set; }
        string Code { get; set; }
        string ContactNo { get; set; }
        int? CountryId { get; set; }
        int CustomerId { get; set; }
        string CustomerName { get; set; }
        enmCustomerType CustomerType { get; set; }
        string Email { get; set; }
        bool HaveGST { get; set; }
        bool IsActive { get; set; }
        int ModifyBy { get; set; }
        string ModifyByName { get; set; }
        DateTime ModifyDt { get; set; }
        string PinCode { get; set; }
        int? StateId { get; set; }
    }

    public class mdlCustomerMaster : ImdlCustomerMaster
    {
        public int CustomerId { get; set; }
        [Required]
        [Display(Name = "Code")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Customer Name")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        [RegularExpression("[a-zA-Z0-9/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string CustomerName { get; set; }
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Have GST")]
        public bool HaveGST { get; set; }
        [Required]
        [Display(Name = "Address")]
        [RegularExpression("[a-zA-Z0-9/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string Address { get; set; }
        [Display(Name = "Country")]
        public int? CountryId { get; set; }
        [Display(Name = "State")]
        public int? StateId { get; set; }
        [DataType(DataType.PostalCode)]
        [Display(Name = "Pincode")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string PinCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Contact No")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 10)]
        [Required]
        public string ContactNo { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Alternate No")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 10)]
        public string AlternateNo { get; set; }

        [Display(Name = "Customer Type")]
        public enmCustomerType CustomerType { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Modified By")]
        public int ModifyBy { get; set; }
        public string ModifyByName { get; set; }
        [Display(Name = "Modified Date")]
        public DateTime ModifyDt { get; set; }

        public string Logo { get; set; }


    }

    public interface ImdlCustomerGSTDetails
    {
        string Address { get; set; }
        int? CountryId { get; set; }
        
        string CustomerName { get; set; }
        string Email { get; set; }
        
        string GstNumber { get; set; }
        string Mobile { get; set; }
        string PinCode { get; set; }
        string RegisteredName { get; set; }
        int? StateId { get; set; }
    }

    public class mdlCustomerGSTDetails : ImdlCustomerGSTDetails
    {
        
        [Required]
        [Display(Name = "GST")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 10)]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string GstNumber { get; set; }
        [Required]
        [Display(Name = "Email")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Registered Name")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        [RegularExpression("[a-zA-Z0-9/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string RegisteredName { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Contact No")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 10)]
        [Required]
        public string Mobile { get; set; }
        [Required]
        [Display(Name = "Address")]
        [RegularExpression("[a-zA-Z0-9/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string Address { get; set; }
        [Display(Name = "Country")]
        public int? CountryId { get; set; }
        [Display(Name = "State")]
        public int? StateId { get; set; }
        [Display(Name = "Pincode")]
        public string PinCode { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
    }

    public interface ImdlUserMaster
    {
        DateTime BlockEndTime { get; set; }
        DateTime BlockStartTime { get; set; }
        string ConfirmPassword { get; set; }
        int? CustomerId { get; set; }
        string Email { get; set; }
        bool ForcePasswordChange { get; set; }
        bool IsActive { get; set; }
        bool IsBlocked { get; set; }
        bool IsPrimary { get; set; }
        DateTime lastLogin { get; set; }
        string Password { get; set; }
        string Phone { get; set; }
        List<int> Roles { get; set; }
        int UserId { get; set; }
        string UserName { get; set; }
    }

    public class mdlUserMaster : ImdlUserMaster
    {
        public int UserId { get; set; }
        [Required]
        [Display(Name = "User Name")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Password")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Force Password Change")]
        public bool ForcePasswordChange { get; set; }
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; }
        [Display(Name = "Is Blocked")]
        public bool IsBlocked { get; set; }
        public bool IsPrimary { get; set; }
        [Display(Name = "Blocked Date")]
        public DateTime BlockStartTime { get; set; }
        [Display(Name = "Blocked EndDate")]
        public DateTime BlockEndTime { get; set; }
        public int? CustomerId { get; set; }
        [Display(Name = "Last Login")]
        public DateTime lastLogin { get; set; }
        public List<int> Roles { get; set; }
        

    }

    public interface ImdlBanks
    {
        string AccountNo { get; set; }
        string ApprovalRemarks { get; set; }
        int BankId { get; set; }
        string BranchAddress { get; set; }        
        string IFSC { get; set; }
        enmApprovalStatus IsApproved { get; set; }
        string NameasonBank { get; set; }
        string Remarks { get; set; }
        string UpiId { get; set; }
    }

    public class mdlBanks : ImdlBanks
    {
        public int? CustomerId { get; set; }
        [Required]
        [Display(Name = "Bank")]
        public int BankId { get; set; }
        [Required]
        [Display(Name = "IFSC")]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 11)]
        [RegularExpression("[A-Za-z]{4}[a-zA-Z0-9]{7}$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string IFSC { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 10)]
        [RegularExpression("[a-zA-Z0-9]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string AccountNo { get; set; }
        [Display(Name = "Branch Address")]
        [RegularExpression("[a-zA-Z0-9/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string BranchAddress { get; set; }
        [Required]
        [Display(Name = "Beneficiary")]
        [RegularExpression("[a-zA-Z0-9/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string NameasonBank { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.")]
        [RegularExpression("[a-zA-Z0-9\\@]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string UpiId { get; set; }
        [Display(Name = "Is Approved")]
        public enmApprovalStatus IsApproved { get; set; }

        [Display(Name = "Remarks")]
        [RegularExpression("[a-zA-Z0-9/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.")]
        public string Remarks { get; set; }

        [Display(Name = "Approval Remarks")]
        [RegularExpression("[a-zA-Z0-9/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.")]
        public string ApprovalRemarks { get; set; }

    }

    public interface ImdlPan
    {
        string ApprovalRemarks { get; set; }        
        enmApprovalStatus IsApproved { get; set; }
        string PANName { get; set; }
        string PANNo { get; set; }
        string Remarks { get; set; }
    }

    public class mdlPan : ImdlPan
    {
        public int? CustomerId { get; set; }
        [Required]
        [Display(Name = "Name  on Pan")]
        [RegularExpression("[a-zA-Z/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string PANName { get; set; }
        [Required]
        [Display(Name = "Pan No")]
        [RegularExpression("[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Invalid {0}, no special charcter")]

        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 10)]
        public string PANNo { get; set; }
        [Display(Name = "Is Approved")]
        public enmApprovalStatus IsApproved { get; set; }

        [Display(Name = "Remarks")]
        [RegularExpression("[a-zA-Z/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 0)]
        public string Remarks { get; set; }

        [Display(Name = "Approval Remarks")]
        [RegularExpression("[a-zA-Z0-9/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.")]
        public string ApprovalRemarks { get; set; }
    }

    public interface ImdlCustomerSetting
    {
        bool AllowedAllIp { get; set; }        
        string IPAddess { get; set; }
        double MarkupAmount { get; set; }
        double MinBalance { get; set; }
    }

    public class mdlCustomerSetting : ImdlCustomerSetting
    {
        public int? CustomerId { get; set; }
        [Display(Name = "Min Balance Alert")]
        public double MinBalance { get; set; }
        [Display(Name = "Markup Amount")]
        public double MarkupAmount { get; set; }
        [Display(Name = "Allowed All IP")]
        public bool AllowedAllIp { get; set; }
        [RegularExpression("[a-zA-Z0-9/.,\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        public string IPAddess { get; set; }
    }

}
