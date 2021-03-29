using Database;
using Database.Classes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WingGateway.Models;

namespace WingGateway.Classes
{
    public class mdlUserInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    
    public interface IConsProfile
    {
        mdlTreeWraper GetAllDownline(int NID);
        List<mdlTcBankWraper> GetBankDetails(enmLoadData loadType, mdlFilterModel mdl, int Nid, bool LoadImage);
        Task<List<mdlTcMarkUpWraper>> GetMarkUpDetails(int tcnid);
        List<mdlTcPANWraper> GetPANDetails(enmLoadData loadType, mdlFilterModel mdl, int Nid, bool LoadImage);
        List<ProcRegistrationSearch> GetTCDetails(enmLoadData loadType, mdlFilterModel mdl, int Nid, int spmode, bool LoadImage);
        List<mdlUserInfo> GetUserName(List<int> UserId, bool IsEmployee = true);
        int GetNId(string Id, bool OnlyUnterminated = false);
        
    }

    public class ConsProfile : Database.Classes.ConsolidatorProfile, IConsProfile
    {

        private readonly IConfiguration _config;

        public ConsProfile(DBContext context, IConfiguration config) : base(context)
        {
            _config = config;
        }

        public List<mdlUserInfo> GetUserName(List<int> UserId, bool IsEmployee = true)
        {
            List<mdlUserInfo> mdlUserInfos = new List<mdlUserInfo>();
            if (IsEmployee)
            {
                mdlUserInfos = _context.tblEmpMaster.Where(p => UserId.Contains(p.EmpId)).Select(p => new mdlUserInfo { UserId = p.EmpId, UserName = p.EmpCode }).ToList();
            }
            return mdlUserInfos;
        }

        public List<mdlTcBankWraper> GetBankDetails(enmLoadData loadType, mdlFilterModel mdl, int Nid, bool LoadImage)
        {
            List<mdlTcBankWraper> returnData = new List<mdlTcBankWraper>();
            if (loadType == enmLoadData.ByID)
            {
                Nid = GetNId(mdl.idFilter.TcId, false);
                if (Nid == 0)
                {
                    throw new Exception("Invalid TC ID");
                }
            }
            if (loadType == enmLoadData.ByDateFilter)
            {
                returnData = _context.tblTcBankDetails.Where(p => p.CreatedDt >= mdl.dateFilter.FromDt && p.CreatedDt < mdl.dateFilter.ToDt && mdl.dateFilter.approvalType.HasFlag(p.IsApproved))
                .Select(p => new mdlTcBankWraper
                {
                    DetailId = p.DetailId,
                    BankId = p.BankId ?? 0,
                    TcBankName = p.tblBankMaster.BankName,
                    TcId = p.tblRegistration.Id,
                    TcName =
                string.Concat(p.tblRegistration.FirstName ?? "", " ", p.tblRegistration.MiddleName ?? "", " ", p.tblRegistration.LastName ?? ""),
                    AccountNo = p.AccountNo,
                    IFSC = p.IFSC,
                    IsApproved = p.IsApproved,
                    RequestDate = p.CreatedDt,
                    ApprovedDt = p.ApprovedDt,
                    Remarks = p.Remarks,
                    NameasonBank = p.NameasonBank,
                    ApprovalRemarks = p.ApprovalRemarks,
                    BranchAddress = p.BranchAddress,
                    UploadImageName = p.UploadImages,
                    ApprovedBy = p.ApprovedBy ?? 0
                }).ToList();
            }
            else
            {
                returnData = _context.tblTcBankDetails.Where(p => p.TcNid == Nid)
                .Select(p => new mdlTcBankWraper
                {
                    DetailId = p.DetailId,
                    BankId = p.BankId ?? 0,
                    TcBankName = p.tblBankMaster.BankName,
                    TcId = p.tblRegistration.Id,
                    TcName =
                string.Concat(p.tblRegistration.FirstName ?? "", " ", p.tblRegistration.MiddleName ?? "", " ", p.tblRegistration.LastName ?? ""),
                    AccountNo = p.AccountNo,
                    IFSC = p.IFSC,
                    IsApproved = p.IsApproved,
                    RequestDate = p.CreatedDt,
                    ApprovedDt = p.ApprovedDt,
                    Remarks = p.Remarks,
                    NameasonBank = p.NameasonBank,
                    ApprovalRemarks = p.ApprovalRemarks,
                    BranchAddress = p.BranchAddress,
                    UploadImageName = p.UploadImages,
                    ApprovedBy = p.ApprovedBy ?? 0
                }).ToList();
            }


            var UserIds = returnData.Where(p => p.ApprovedBy > 0).Select(p => p.ApprovedBy).Distinct().ToList();
            var UserNames = GetUserName(UserIds, true);
            returnData.ForEach(
                p =>
                {
                    p.ApproverName = UserNames.FirstOrDefault(q => q.UserId == p.ApprovedBy)?.UserName;
                }
                );

            if (LoadImage)
            {
                string filePath = _config["FileUpload:Bank"];
                var path = Path.Combine(
                         Directory.GetCurrentDirectory(),
                         "wwwroot/" + filePath);
                returnData.ForEach(
                p =>
                {
                    var files = p.UploadImageName.Split(",");
                    foreach (var file in files)
                    {
                        try
                        {
                            p.fileData.Add(System.IO.File.ReadAllBytes(string.Concat(path, file)));
                        }
                        catch { }
                        
                    }
                }
                );
            }

            return returnData;

        }


        public List<mdlTcPANWraper> GetPANDetails(enmLoadData loadType, mdlFilterModel mdl, int Nid, bool LoadImage)
        {
            List<mdlTcPANWraper> returnData = new List<mdlTcPANWraper>();
            if (loadType == enmLoadData.ByID)
            {
                Nid = GetNId(mdl.idFilter.TcId, false);
                if (Nid == 0)
                {
                    throw new Exception("Invalid TC ID");
                }
            }
            if (loadType == enmLoadData.ByDateFilter)
            {
                returnData = _context.TblTcPanDetails.Where(p => p.CreatedDt >= mdl.dateFilter.FromDt && p.CreatedDt < mdl.dateFilter.ToDt && mdl.dateFilter.approvalType.HasFlag(p.IsApproved))
                .Select(p => new mdlTcPANWraper
                {
                    DetailId = p.DetailId,
                    TcId = p.tblRegistration.Id,
                    TcName =
                string.Concat(p.tblRegistration.FirstName ?? "", " ", p.tblRegistration.MiddleName ?? "", " ", p.tblRegistration.LastName ?? ""),
                    TcPANNo = p.PANNo,
                    TcNameasonPAN = p.PANName,
                    IsApproved = p.IsApproved,
                    RequestDate = p.CreatedDt,
                    ApprovedDt = p.ApprovedDt,
                    Remarks = p.Remarks,
                    ApprovalRemarks = p.ApprovalRemarks,
                    UploadImageName = p.UploadImages,
                    ApprovedBy = p.ApprovedBy ?? 0
                }).ToList();
            }
            else
            {
                returnData = _context.TblTcPanDetails.Where(p => p.TcNid == Nid)
                .Select(p => new mdlTcPANWraper
                {
                    DetailId = p.DetailId,
                    TcId = p.tblRegistration.Id,
                    TcName =
                string.Concat(p.tblRegistration.FirstName ?? "", " ", p.tblRegistration.MiddleName ?? "", " ", p.tblRegistration.LastName ?? ""),
                    TcPANNo = p.PANNo,
                    TcNameasonPAN = p.PANName,
                    IsApproved = p.IsApproved,
                    RequestDate = p.CreatedDt,
                    ApprovedDt = p.ApprovedDt,
                    Remarks = p.Remarks,
                    ApprovalRemarks = p.ApprovalRemarks,
                    UploadImageName = p.UploadImages,
                    ApprovedBy = p.ApprovedBy ?? 0
                }).ToList();
            }


            var UserIds = returnData.Where(p => p.ApprovedBy > 0).Select(p => p.ApprovedBy).Distinct().ToList();
            var UserNames = GetUserName(UserIds, true);
            returnData.ForEach(
                p =>
                {
                    p.ApproverName = UserNames.FirstOrDefault(q => q.UserId == p.ApprovedBy)?.UserName;
                }
                );

            if (LoadImage)
            {
                string filePath = _config["FileUpload:PAN"];
                var path = Path.Combine(
                         Directory.GetCurrentDirectory(),
                         "wwwroot/" + filePath);
                returnData.ForEach(
                p =>
                {
                    var files = p.UploadImageName.Split(",");
                    foreach (var file in files)
                    {
                        try
                        {
                            p.fileData.Add(System.IO.File.ReadAllBytes(string.Concat(path, file)));
                        }
                        catch { }
                        }
                }
                );
            }

            return returnData;

        }

        public List<ProcRegistrationSearch> GetTCDetails(enmLoadData loadType, mdlFilterModel mdl, int Nid, int spmode, bool LoadImage)
        {
            List<ProcRegistrationSearch> returnData = new List<ProcRegistrationSearch>();
            if (loadType == enmLoadData.ByID)
            {
                Nid = GetNId(mdl.idFilter.TcId, false);
                if (Nid == 0)
                {
                    throw new Exception("Invalid TC ID");
                }
            }

            DateTime datefrom = DateTime.Now;
            DateTime datetto = DateTime.Now;
            string tcid = "";
            int status = 0;

            if (mdl.dateFilter != null)
            {
                datefrom = mdl.dateFilter.FromDt;
                datetto = mdl.dateFilter.ToDt;
                status = Convert.ToInt32(mdl.dateFilter.approvalType);
                spmode = 1;
            }

            if (mdl.idFilter != null)
            {
                tcid = mdl.idFilter.TcId;
                spmode = 2;
            }

            using (SqlConnection sqlconnection = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand("proc_registration_search", sqlconnection))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Add(new SqlParameter("datefrom", datefrom));
                    sqlcmd.Parameters.Add(new SqlParameter("dateto", datetto));
                    sqlcmd.Parameters.Add(new SqlParameter("is_active", status));
                    sqlcmd.Parameters.Add(new SqlParameter("tcid", tcid));
                    sqlcmd.Parameters.Add(new SqlParameter("session_nid", Nid));
                    sqlcmd.Parameters.Add(new SqlParameter("spmode", spmode));


                    sqlconnection.Open();
                    SqlDataReader rd = sqlcmd.ExecuteReader();
                    while (rd.Read())
                    {
                        returnData.Add(new ProcRegistrationSearch()
                        {
                            TCID = Convert.ToString(rd["tcid"]),
                            tcnid = Convert.ToInt32(rd["tcnid"]),
                            FirstName = Convert.ToString(rd["firstname"]),
                            MiddleName = Convert.ToString(rd["middlename"]),
                            LastName = Convert.ToString(rd["lastname"]),
                            husband_wifename = Convert.ToString(rd["Husband_father_name"]),
                            DOB = Convert.ToString(rd["DOB"]),
                            JoiningDate = Convert.ToString(rd["joining_date"]),
                            Address1 = Convert.ToString(rd["address_line1"]),
                            Address2 = Convert.ToString(rd["address_line2"]),
                            pincode = Convert.ToString(rd["pincode"]),
                            landmark = Convert.ToString(rd["landmark"]),
                            mobileno = Convert.ToString(rd["mobileno"]),
                            emailid = Convert.ToString(rd["emailid"]),
                            statename = Convert.ToString(rd["statename"]),
                            cityname = Convert.ToString(rd["cityname"]),
                            countryname = Convert.ToString(rd["countryname"]),
                            tcspid = Convert.ToString(rd["spid"]),
                            tcspname = Convert.ToString(rd["spname"]),
                            tcspnid = Convert.ToInt32(rd["tcspnid"]),
                            block = (enmIsKycUpdated)Convert.ToInt32(rd["isblock"]),
                            terminate = (enmIsKycUpdated)Convert.ToInt32(rd["isterminate"]),
                            stateid = Convert.ToInt32(rd["stateid"]),
                            isactive = (enmApprovalType)Convert.ToInt32(rd["IsActive"]),
                            cityid = Convert.ToInt32(rd["cityid"]),
                            countryid = Convert.ToInt32(rd["countryid"]),
                            gender_id = (enmGender)Convert.ToInt32(rd["gender"]),


                        });
                    }
                }

            }

            return returnData;

        }


        public List<ProcHolidayPackageSearch> GetHolidayPackageDetails(enmLoadData loadType, mdlFilterModel mdl, int empid, int spmode, bool LoadImage)
        {
            List<ProcHolidayPackageSearch> returnData = new List<ProcHolidayPackageSearch>();
            

            DateTime datefrom = DateTime.Now;
            DateTime datetto = DateTime.Now;
            string tcid = "";
            int status = 0;

            if (mdl.dateFilter != null)
            {
                datefrom = mdl.dateFilter.FromDt;
                datetto = mdl.dateFilter.ToDt;
                status = Convert.ToInt32(mdl.dateFilter.approvalType);
                spmode = 1;
            }

            else if (mdl.idFilter != null)
            {
                tcid = mdl.idFilter.TcId;
                spmode = 2;
            }
            else
                spmode = 3;

            string filePath_holiday_package_image = _config["FileUpload:HolidayPackageImage"];
            string filePath_other_package_image = _config["FileUpload:HolidayOtherImage"];

            var path_holiday_package = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + filePath_holiday_package_image);

            var path_other_package = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + filePath_other_package_image);

            using (SqlConnection sqlconnection = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand("proc_holiday_package_search", sqlconnection))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Add(new SqlParameter("datefrom", datefrom));
                    sqlcmd.Parameters.Add(new SqlParameter("dateto", datetto));
                    sqlcmd.Parameters.Add(new SqlParameter("is_active", status));
                    sqlcmd.Parameters.Add(new SqlParameter("session_nid", empid));
                    sqlcmd.Parameters.Add(new SqlParameter("spmode", spmode));


                    sqlconnection.Open();
                    SqlDataReader rd = sqlcmd.ExecuteReader();
                    while (rd.Read())
                    {
                        returnData.Add(new ProcHolidayPackageSearch()
                        {
                            PackageName = Convert.ToString(rd["packagename"]),
                            PackageType = (enmPackageCustomerType) Convert.ToInt32(rd["PackageType"]),
                            PackageFromDate = Convert.ToString(rd["packagefromdate"]),
                            PackageToDate = Convert.ToString(rd["packagetodate"]),
                            PriceFrom = Convert.ToInt32(rd["pricefrom"]),
                            PriceTo= Convert.ToInt32(rd["priceto"]),
                            MemberCount = Convert.ToInt32(rd["MemberCount"]),
                            DaysCount = Convert.ToInt32(rd["DaysCount"]),
                            state_name = Convert.ToString(rd["statename"]),
                            country_name = Convert.ToString(rd["countryname"]),
                            is_active = (enmStatus)Convert.ToInt32(rd["IsActive"]),
                            country_id = Convert.ToInt32(rd["countryid"]),
                            state_id = Convert.ToInt32(rd["countryid"]),
                            PackageDescription=Convert.ToString(rd["PackageDescription"]),
                            SpecialNote= Convert.ToString(rd["SpecialNote"]),
                            UploadPackageImage= filePath_holiday_package_image + ""+Convert.ToString(rd["UploadPackageImage"]),
                            UploadOtherImage= filePath_other_package_image+ ""+Convert.ToString(rd["UploadOtherImage"]),
                            CreatedByid = Convert.ToInt32(rd["createdbyid"]),
                            CreatedByName = Convert.ToString(rd["CreatedByName"]),
                            CreatedDt=Convert.ToString(rd["createddt"]),
                            lastModifiedByid = Convert.ToInt32(rd["lastModifiedByid"]),
                            lastModifiedByName= Convert.ToString(rd["lastModifiedByName"]),
                            LastModifieddate= Convert.ToString(rd["LastModifieddate"]),
                            
                        });
                    }
                }

            }

            return returnData;

        }


        new public mdlTreeWraper GetAllDownline(int NID)
        {
            mdlTreeWraper mdltreeWraper = new mdlTreeWraper();
            var reg = _context.tblRegistration.Where(p => p.Nid == NID).FirstOrDefault();
            if (reg == null)
            {
                return mdltreeWraper;
            }
            mdltreeWraper.id = reg.Nid;
            mdltreeWraper.TcId = reg.Id;
            mdltreeWraper.Nid = reg.Nid;
            mdltreeWraper.Rank = reg.TCRanks;
            mdltreeWraper.Isterminate = reg.IsTerminate;
            mdltreeWraper.LegId = reg.SpLegNumber;
            mdltreeWraper.Name = string.Concat(reg.FirstName, " ", reg.MiddleName, " ", reg.LastName);
            mdltreeWraper.text = string.Format($"{reg.Id} - {mdltreeWraper.Name},<span class='badge badge-success'> Rank : {mdltreeWraper.Rank}</span>");
            mdltreeWraper.icon = "fas fa-user";
            List<mdlTree> mdlTrees = base.GetAllDownline(NID);
            BindWithTree(mdlTrees, mdltreeWraper);
            return mdltreeWraper;
        }

        private void BindWithTree(IEnumerable<mdlTree> mdlTrees, mdlTreeWraper mdltreeWraper)
        {
            var tempdatas = mdlTrees.Where(p => p.SpNid == mdltreeWraper.Nid).OrderBy(p => p.LegId);
            foreach (var tempdata in tempdatas)
            {
                mdlTreeWraper mdl = new mdlTreeWraper()
                {
                    id = tempdata.Nid,
                    TcId = tempdata.TcId,
                    Nid = tempdata.Nid,
                    Rank = tempdata.Rank,
                    Isterminate = tempdata.Isterminate,
                    LegId = tempdata.LegId,
                    Name = tempdata.Name

                };
                if (mdl.Isterminate)
                {
                    mdl.icon = "fas fa-ban";
                }
                else
                {
                    mdl.icon = "fas fa-user";
                }

                mdl.text = string.Format($"{mdl.LegId}) {mdl.TcId} - {mdl.Name},<span class='badge badge-success'> Rank : {mdl.Rank}</span>");
                BindWithTree(mdlTrees, mdl);
                if (mdltreeWraper.children == null)
                {
                    mdltreeWraper.children = new List<mdlTreeWraper>();
                }
                mdltreeWraper.children.Add(mdl);
            }
        }

        public async Task <List<mdlTcMarkUpWraper>> GetMarkUpDetails(int tcnid)
        {
            List<mdlTcMarkUpWraper> returnData = new List<mdlTcMarkUpWraper>();
            
                returnData.AddRange( await _context.TblTcMarkUp.Where(p => p.TcNid == tcnid)
                .Select(p => new mdlTcMarkUpWraper
                {
                    DetailId = p.DetailId,
                    BookingType = p.BookingType,
                    MarkUpValue = p.MarkupValue,
                    TcId = p.tblRegistration.Id,
                    RequestDate = p.CreatedDt,
                    ModifiedDate = p.LastModifieddate
                }).ToListAsync());
            return returnData;
        }

    }

}
