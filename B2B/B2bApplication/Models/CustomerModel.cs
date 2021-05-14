﻿using B2BClasses;
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

namespace B2bApplication.Models
{

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

}


