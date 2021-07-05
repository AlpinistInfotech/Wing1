using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database
{
    public class tblEmpMaster
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EmpId { get; set; }
        [MaxLength(20)]
        public string EmpCode { get; set; }
        public bool IsTerminate { get; set; }
        public DateTime JoiningDt { get; set; }
    }
}
