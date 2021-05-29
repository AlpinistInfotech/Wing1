using B2BClasses.Services.Air;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace B2BClasses.Database
{
    public class tblBankMaster
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BankId { get; set; }
        [Required]
        [MaxLength(100)]
        public string BankName { get; set; }
        public bool IsActive { get; set; }
    }

    [Index(nameof(CountryCode))]
    public class tblCountryMaster
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CountryId { get; set; }
        [MaxLength(3)]
        public string CountryCode { get; set; }
        [MaxLength(2)]
        public string CountryCodeIso2 { get; set; }
        [MaxLength(200)]
        public string CountryName { get; set; }
        [MaxLength(30)]
        public string PhoneCode { get; set; }
        [MaxLength(200)]
        public string Capital { get; set; }
        [MaxLength(100)]
        public string currency { get; set; }
        [MaxLength(100)]
        public string CurrencySymbol { get; set; }
        [MaxLength(100)]
        public string Domain { get; set; }
        [MaxLength(100)]
        public string Native { get; set; }
        [MaxLength(100)]
        public string Region { get; set; }
        [MaxLength(100)]
        public string SubRegion { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        [MaxLength(200)]
        public string Remarks { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        [NotMapped]
        public string ModifiedByName { get; set; }
    }

    public class tblStateMaster
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StateId { get; set; }
        [MaxLength(10)]
        public string StateCode { get; set; }
        [MaxLength(200)]
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
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        [NotMapped]
        public string ModifiedByName { get; set; }
        [NotMapped]
        public virtual string CountryName { get; set; }

    }

    public class tblAirline
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Code { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public bool isLcc { get; set; }
        [MaxLength(500)]
        public string ImagePath { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }

    }
    public class tblAirport : mdlAirport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsDomestic { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public class tblWingBankAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("tblBankMaster")]
        public int? BankId { get; set; }
        public tblBankMaster tblBankMaster { get; set; }
        [MaxLength(20)]
        public string IFSC { get; set; }
        [MaxLength(20)]
        public string AccountNo { get; set; }
        [MaxLength(200)]
        public string BranchAddress { get; set; }
        [MaxLength(200)]
        public string NameasonBank { get; set; }
        public string UpiId { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public bool Isdeleted { get; set; }
    }

    

}
