using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WingApi.Models
{
    public class mdlLogin
    {
        [Required]
        [Display(Name = "Code")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string CustomerCode { get; set; }

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
