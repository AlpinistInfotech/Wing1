﻿using Database;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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

    public class ConsProfile : Database.Classes.ConsolidatorProfile
    {

        private readonly IConfiguration _config;

        public ConsProfile(DBContext context, IConfiguration config) :base(context)
        {
            _config = config;
        }

        public List<mdlUserInfo> GetUserName(List<int> UserId, bool IsEmployee=true)
        {
            List<mdlUserInfo> mdlUserInfos = new List<mdlUserInfo>();
            if (IsEmployee)
            {
                mdlUserInfos = _context.tblEmpMaster.Where(p => UserId.Contains(p.EmpId)).Select(p => new mdlUserInfo { UserId = p.EmpId, UserName = p.EmpCode }).ToList();
            }
            

            return mdlUserInfos;
        }


        public List<mdlTcBankWraper> GetBankDetails(enmLoadData loadType, mdlFilterModel mdl,int Nid , bool LoadImage)
        {
            List<mdlTcBankWraper> returnData = new List<mdlTcBankWraper>();
            if (loadType == enmLoadData.ByID)
            {
                Nid = GetNId(mdl.idFilter.TcId, false);
                if (Nid == 0)
                {
                    throw new Exception("Invalid Nid");
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
                    TcId=p.tblRegistration.Id,
                    TcName =
                string.Concat(p.tblRegistration.FirstName ?? "", " ", p.tblRegistration.MiddleName ?? "", " ", p.tblRegistration.MiddleName ?? ""),
                    AccountNo = p.AccountNo,
                    IFSC = p.IFSC,
                    IsApproved = p.IsApproved,
                    RequestDate = p.CreatedDt,
                    ApprovedDt = p.ApprovedDt,
                    Remarks = p.Remarks,
                    ApprovalRemarks = p.ApprovalRemarks,
                    BranchAddress=p.BranchAddress,
                    UploadImageName=p.UploadImages,
                    ApprovedBy = p.ApprovedBy ?? 0
                }).ToList();
            }
            else
            {
                returnData = _context.tblTcBankDetails.Where(p => p.TcNid==Nid )
                .Select(p => new mdlTcBankWraper
                {
                    DetailId=p.DetailId,
                    BankId = p.BankId ?? 0,
                    TcBankName = p.tblBankMaster.BankName,
                    TcId = p.tblRegistration.Id,
                    TcName =
                string.Concat(p.tblRegistration.FirstName ?? "", " ", p.tblRegistration.MiddleName ?? "", " ", p.tblRegistration.MiddleName ?? ""),
                    AccountNo = p.AccountNo,
                    IFSC = p.IFSC,
                    IsApproved = p.IsApproved,
                    RequestDate = p.CreatedDt,
                    ApprovedDt = p.ApprovedDt,
                    Remarks = p.Remarks,
                    ApprovalRemarks = p.ApprovalRemarks,
                    BranchAddress = p.BranchAddress,
                    UploadImageName = p.UploadImages,ApprovedBy=p.ApprovedBy??0
                }).ToList();
            }


            var UserIds = returnData.Where(p => p.ApprovedBy > 0).Select(p => p.ApprovedBy).Distinct().ToList();
            var UserNames = GetUserName(UserIds, true);
            returnData.ForEach(
                p => {
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
                p => {
                    var files = p.UploadImageName.Split(",");
                    foreach (var file in files)
                    {
                        p.fileData.Add(System.IO.File.ReadAllBytes(string.Concat(path, file)));
                    }
                }
                );
            }

            return returnData;

        }
    }
}
