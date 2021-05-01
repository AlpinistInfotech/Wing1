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

        public tblCustomerMaster GetCustomerData(DBContext context, int customerid)
        {
            return context.tblCustomerMaster.Where(p => p.Id == customerid).FirstOrDefault();
        }
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

        public List<tblCustomerMaster> GetCustomerMaster(DBContext context, bool OnlyActive, int customerid)
        {
            if (customerid > 0)
            {
                return context.tblCustomerMaster.Where(p => p.Id==customerid).ToList();
            }
            else
            {
                if (OnlyActive)
                {
                    return context.tblCustomerMaster.Where(p => p.IsActive).ToList();
                }
                else
                {
                    return context.tblCustomerMaster.ToList();
                }
            }

        }

        public List<tblUserMaster> UserMasters { get; set; }
        public List<tblUserMaster> GetCustomerUserList(DBContext context, bool OnlyActive,int customerid)
        {
            if (OnlyActive)
            {
                return context.tblUserMaster.Where(p => p.IsActive && p.CustomerId == customerid).ToList();
            }
            else
            {
                return context.tblUserMaster.Where( p => p.CustomerId == customerid).ToList();
            }

        }

        public tblUserMaster GetCustomerUserData(DBContext context, int userid)
        {
            return context.tblUserMaster.Where(p => p.Id == userid).FirstOrDefault();   
        }
    }
   

}


