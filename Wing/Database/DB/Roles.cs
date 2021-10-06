using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.DB
{
    public class tblRoles :d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        [MaxLength(32)]
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class tblRoleClaim : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sno { get; set; }
        public int RoleId { get; set; }
        public enmDocumentMaster ClaimId { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class tblUserRole : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sno { get; set; }
        public int RoleId { get; set; }
        public ulong UserId { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class tblUserClaim : d_ModifiedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sno { get; set; }        
        public ulong UserId { get; set; }
        public enmDocumentMaster ClaimId { get; set; }
        public bool IsDeleted { get; set; }
    }



}
