using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace B2bApplication.Models
{
    public class mdlAccountChangePassword
    {
        [Required(ErrorMessage = "Enter Old Password")]
        public string Passowrd { get; set; }

        [Required(ErrorMessage = "Enter New Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Re-Enter New Password")]
        public string ConfirmNewPassword { get; set; }
    }
}
