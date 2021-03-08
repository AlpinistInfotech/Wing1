using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database
{
    [Index(nameof(Id))]
    [Index(nameof(SpNid))]
    public class tblRegistration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Nid { get; set; }        
        public string Id { get; set; }
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string MiddleName { get; set; }
        [MaxLength(30)]
        public string LastName { get; set; }
        [MaxLength(100)]
        public string Husband_father_name { get; set; }
        public enmGender Gender { get; set; }
        public DateTime Dob { get; set; }
        [ForeignKey("tblStateMaster")]
        public int? JoiningState { get; set; }
        public tblStateMaster tblStateMaster { get; set; }
        [ForeignKey("tblRegistrationSponsor")]
        public int? SpNid { get; set; }
        public tblRegistration tblRegistrationSponsor { get; set; }
        public string SpId { get; set; }
        public bool Isblock { get; set; }
        public enmIsKycUpdated IsKycUpdated { get; set; }
        public bool IsTerminate { get; set; }
        public DateTime JoiningDt { get; set; }
        public int SpLegNumber { get; set; }        
        public enmTCRanks TCRanks { get; set; }
    }

    [Index(nameof(TcNid))]
    [Index(nameof(TcSpNid))]
    public class tblTree
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TreeId { get; set; }
        public int TcNid { get; set; }
        public int TcSpNid { get; set; }
    }


    public class tblTcRanksDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        public enmTCRanks TCRanks { get; set; }
        public DateTime QualifyDate { get; set; }
        public double PPRequired { get; set; }
        public double PPDone { get; set; }
        public bool Isdeleted { get; set; }
    }

    public class tblTcAddressDetail : IModified, ICreated
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        public enmAddressType AddressType { get; set; }
        [MaxLength(200)]
        public string address_line1 { get; set; }
        [MaxLength(200)]
        public string address_line2 { get; set; }
        [ForeignKey("tblStateMaster")]
        public int? StateId { get; set; }
        public tblStateMaster tblStateMaster { get; set; }
        [ForeignKey("tblCountryMaster")]
        public int? CountryId { get; set; }
        public tblCountryMaster tblCountryMaster { get; set; }
        public string Pincode { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public string Remarks { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        [NotMapped]
        public string ModifiedByName { get; set; }
    }


    public class tblKycMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        public enmIdentityProof IdProofType { get; set; }
        public string IdDocumentNo { get; set; }
        public string IdDocumentName { get; set; }
        public enmApprovalType IsApproved { get; set; }        
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public string Remarks { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDt { get; set; }
        public string ApprovalRemarks { get; set; }
        public bool Isdeleted { get; set; }

    }

    public class tblTcBankDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        [ForeignKey("tblBankMaster")]
        public int? BankId { get; set; }
        public tblBankMaster tblBankMaster { get; set; }
        [MaxLength(20)]
        public string IFSC { get; set; }
        [MaxLength(20)]
        public string AccountNo { get; set; }
        [MaxLength(200)]
        public string BranchAddress{ get; set; }
        [MaxLength(200)]
        public string UploadImages { get; set; }
        public enmApprovalType IsApproved { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public string Remarks { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDt { get; set; }
        public string ApprovalRemarks { get; set; }
        public bool Isdeleted { get; set; }
    }

    public class tblTcPANDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        [ForeignKey("tblPANMaster")]
        public int? PANId { get; set; }
        public tblPANMaster tblPANMaster { get; set; }
        [MaxLength(100)]
        public string PANName { get; set; }
        [MaxLength(10)]
        public string PANNo { get; set; }
       
        [MaxLength(200)]
        public string UploadImages { get; set; }
        public enmApprovalType IsApproved { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public string Remarks { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDt { get; set; }
        public string ApprovalRemarks { get; set; }
        public bool Isdeleted { get; set; }
    }

}
