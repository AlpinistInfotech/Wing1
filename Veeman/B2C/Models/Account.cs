using B2C.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace B2C.Models
{
    public class mdlLogin
    {
        public enmCustomerType CustomerType { get; set; } = enmCustomerType.MLM;
        [Display(Name = "Code")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string Code { get; set; }
        [Required]
        [Display(Name = "UserName")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Password { get; set; }

    }
}
