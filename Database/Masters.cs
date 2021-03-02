using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database
{

    public class ApplicationUser : IdentityUser
    {   
        public enmUserType UserType { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        [ForeignKey("tblEmpMaster")]
        public int? EmpId{ get; set; }
        public tblEmpMaster tblEmpMaster { get; set; }

    }

    public class tblCaptcha: ICreated
    {
        [Key]
        public string SaltId { get; set; }
        public string CaptchaCode { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }        
    }


     public class tblCountryMaster : IModified, ICreated
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CountryId { get; set; }
        [MaxLength(2)]
        public string CountryCode { get; set; }
        [MaxLength(100)]
        public string CountryName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public string Remarks { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        [NotMapped]
        public string ModifiedByName { get; set; }
    }
    public class tblStateMaster : IModified, ICreated
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StateId { get; set; }
        [MaxLength(2)]
        public string StateCode { get; set; }
        [MaxLength(100)]
        public string StateName { get; set; }
        [ForeignKey("tblCountryMaster")]
        public int? CountryId { get; set; }
        public tblCountryMaster tblCountryMaster { get; set; }
        public bool IsUT { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public string Remarks { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        [NotMapped]
        public string ModifiedByName { get; set; }
        [NotMapped]
        public virtual string CountryName { get; set; }

    }
    
    
    public class tblTcSequcence
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TcSequcence { get; set; }
        public int Monthyear { get; set; }
        public int StateId { get; set; }
        public int CurrentSeq { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

}
