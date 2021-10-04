using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.DB
{
    public class tblBankMaster : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BankId { get; set; }
        [MaxLength(256)]
        public string BankName { get; set; }
        public bool IsActive { get; set; }
    }

    public class tblCurrency : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CurrencyId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(8)]
        public string Symbol { get; set; }
        public bool IsActive { get; set; }
    }

    public class tblTaxMaster : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaxId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        public double TaxPercentage { get; set; }
        public bool IsActive { get; set; }
    }

    public class tblCompanyMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyId { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(128)]
        public string AddressLine1 { get; set; }
        [MaxLength(128)]
        public string AddressLine2 { get; set; }
        public bool IsActive { get; set; }
        public string CompanyLogo { get; set; }
    }

    public class tblUserIdentity
    {
        [Key]
        public int CountryId { get; set; }
        public int UserCounter { get; set; }
        [Timestamp]
        [ConcurrencyCheck]
        public byte[] RowVersion { get; set; }
    }

    public class tblDispatchIdentity
    {
        [Key]
        public int FiscalYearID { get; set; }
        public int DispatchCounter { get; set; }
        [Timestamp]
        [ConcurrencyCheck]
        public byte[] RowVersion { get; set; }
    }

    public class tblBookingIdentity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int CountryId { get; set; }
        public int YearId { get; set; }
        public int WeekId { get; set; }
        public enmBookingType BookingType { get; set; }
        public int ProductCounter { get; set; }
        [Timestamp]
        [ConcurrencyCheck]
        public byte[] RowVersion { get; set; }
    }

    public class tblInvoiceIdentity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sno { get; set; }
        public int CountryId { get; set; }
        public int YearId { get; set; }
        public int MonthId { get; set; }
        public int InvoiceCounter { get; set; }
        [Timestamp]
        [ConcurrencyCheck]
        public byte[] RowVersion { get; set; }
    }

}
