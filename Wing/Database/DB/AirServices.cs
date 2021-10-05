using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.DB
{
    public class tblAirline :d_ModifiedBy
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
        public bool IsDeleted { get; set; }
    }

    public class tblAirport :d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(200)]
        public string AirportCode { get; set; }
        [MaxLength(200)]
        public string AirportName { get; set; }
        [MaxLength(200)]
        public string Terminal { get; set; }
        [MaxLength(200)]
        public string CityCode { get; set; }
        [MaxLength(200)]
        public string CityName { get; set; }
        [MaxLength(200)]
        public string CountryCode { get; set; }
        [MaxLength(200)]
        public string CountryName { get; set; }
        public bool IsDomestic { get; set; }        
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

    }
    public class DbWingMarkupServiceProvider
    {
        [Key]   
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual int? MarkupId { get; set; }
        public enmServiceProvider ServiceProvider { get; set; }
    }


}
