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
        public string TempUserId { get; set; } = Guid.NewGuid().ToString().Replace("-","");
        public enmCustomerType CustomerType { get; set; } = enmCustomerType.MLM;
        [Display(Name = "Code")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string Code { get; set; }
        [Required]
        [Display(Name = "UserName")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Password { get; set; }
        public string CaptchaId { get; set; }
        public string CaptchaValue { get; set; }
        public string OrgCode { get; set; }
        public string Longitute { get; set; }
        public string Latitude { get; set; }
        public string FromLocation{ get; set; }
        public int UserType { get; set; } = 4;
        public string CaptchaImage { get; set; }
    }

    public class mdlLoginResponse
    {
        public ulong userId { get; set; }
        public string normalizedName { get; set; } = string.Empty;
        public int empId { get; set; }
        public int customerId { get; set; }
        public ulong distributorId { get; set; }
        public enmCustomerType customerType { get; set; }
        public int orgId { get; set; }
        public string email { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;
        public string token { get; set; }

    }

    public class mdlLoginResponseWraper
    {
        public string message { get; set; }
        public enmMessageType messageType { get; set; }
        public mdlLoginResponse returnId { get; set; }
    }

    public class mdlCaptchaWraper
    {
        public string message { get; set; }
        public enmMessageType messageType { get; set; }
        public mdlCaptcha returnId { get; set; }
    }

    public class mdlCaptcha
    {
        public string captchaImage { get; set; }
        public string tempUserId { get; set; }
        public string captchaId { get; set; }
    }
}
