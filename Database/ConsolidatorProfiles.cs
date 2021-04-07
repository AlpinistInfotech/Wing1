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
        public enmApprovalType is_active { get; set; }

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

    public class tblTCStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        
        public enmTCStatus action_type { get; set; }
        
        public enmApprovalType action { get; set; }

        public string action_remarks { get; set; }
        public int action_by { get; set; }
        public DateTime action_datetime { get; set; }
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

        [ForeignKey("tblCountryMaster")]
        public int? CountryId { get; set; }
        public tblCountryMaster tblCountryMaster { get; set; }

        [ForeignKey("tblStateMaster")]
        public int? StateId { get; set; }
        public tblStateMaster tblStateMaster { get; set; }

        [ForeignKey("tblCityMaster")]
        public int? CityId { get; set; }
        public tblCityMaster tblCityMaster { get; set; }

        public string remarks { get; set; }

        public string Pincode { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public string landMark { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        
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
        public string NameasonBank { get; set; }


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

    public class tblTcPanDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        
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


    public class tblTcNominee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }

        [MaxLength(100)]
        public string NomineeName { get; set; }

        public enmNomineeRelation NomineeRelation { get; set; }

        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }

        public string Remarks { get; set; }
        public bool Isdeleted { get; set; }

        public DateTime LastModifieddate { get; set; }
        public int lastModifiedBy { get; set; }

    }

    public class tblTcContact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }

        [MaxLength(10)]
        public string MobileNo { get; set; }

        [MaxLength(10)]
        public string AlternateMobileNo { get; set; }

        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }

        public bool Isdeleted { get; set; }

        public DateTime LastModifieddate { get; set; }
        public int lastModifiedBy { get; set; }

    }

    public class tblTcEmail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }

        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }

        [MaxLength(80)]
        public string EmailID { get; set; }

        [MaxLength(80)]
        public string AlternateEmailID { get; set; }

        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }

        public bool Isdeleted { get; set; }

        public DateTime LastModifieddate { get; set; }
        public int lastModifiedBy { get; set; }

    }

    public class tblTcMarkUp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }

        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }

        public enmBookingType BookingType { get; set; }

        [MaxLength(4)]
        public decimal MarkupValue { get; set; }

        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }

        public bool Isdeleted { get; set; }

        public DateTime LastModifieddate { get; set; }
        public int lastModifiedBy { get; set; }

    }


    public class tblHolidayPackageMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        public string PackageName { get; set; }

        public enmPackageCustomerType PackageType { get; set; }

        public DateTime PackageFromDate { get; set; }

        public DateTime PackageToDate { get; set; }

        public int PriceFrom { get; set; }

        public int PriceTo { get; set; }
        public int MemberCount { get; set; }

        public int DaysCount { get; set; }


        public int country_id { get; set; }
        public int state_id { get; set; }

        public string PackageDescription { get; set; }
        public string SpecialNote { get; set; }
        public string UploadPackageImage { get; set; }
        public string UploadOtherImage { get; set; }

        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public int is_active { get; set; }

        public bool Isdeleted { get; set; }

        public DateTime LastModifieddate { get; set; }
        public int lastModifiedBy { get; set; }

    }

    public class tblTCWallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }

        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }

        public decimal walletamt { get; set; }
    }

    public class tblTCWalletLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }

        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }

        public decimal credit { get; set; }
        public decimal debit { get; set; }
        public int groupid { get; set; }
        public string remarks { get; set; }
        public int reqno { get; set; }
        public int createdby { get; set; }
        public DateTime createddatetime { get; set; }

    }

    public class tblTCMemberRank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        public enmMemberRank levelid { get; set; }
        public enmMemberRank displayid { get; set; }
    }

    public class tblTCMemberRankLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        public enmMemberRank levelid { get; set; }
        public DateTime qualifieddate { get; set; }
    }


    public class tblTCInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        public DateTime inv_date { get; set; }
        public decimal inv_amt { get; set; }
        public decimal pv { get; set; }
        public decimal bv { get; set; }
        public DateTime createddatetime { get; set; }
    }


    public class tblTCStatement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int inentiveId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        public int monthyear { get; set; }
        public decimal incentive { get; set; }
        public decimal deduction { get; set; }
        public decimal tds { get; set; }
        public decimal netincentive { get; set; }
        public enmIncentiveStatus incentivestatus { get; set; }
        public DateTime createddatetime { get; set; }
    }

    public class tblTCStatementLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailedId { get; set; }
        
        [ForeignKey("tblTCStatement")]
        public int? inentiveId { get; set; }
        public enmIncentiveStatus incentive_status { get; set; }
        public string remarks { get; set; }
        public int createdby { get; set; }
        public DateTime createddatetime { get; set; }
    }




    public class tblTCSaleSummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int salesummaryId { get; set; }
        [ForeignKey("tblRegistration")]
        public int? TcNid { get; set; }
        public tblRegistration tblRegistration { get; set; }
        public int monthyear { get; set; }

        [ForeignKey("tblRegistration")]
        public int? TcSpNid { get; set; }
        public tblRegistration tblRegistrationSp { get; set; }
        public enmMemberRank TcNidlevelid { get; set; }
        public enmMemberRank TcSpNidlevelid { get; set; }
        public decimal incentive { get; set; }
    }




}
