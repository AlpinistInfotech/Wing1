using B2BClasses.Database;
using B2BClasses.Models;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B2BClasses
{
    public class CustomerMaster
    {
        private readonly DBContext _context;
        public int CustomerId { get {  return _CustomerId; } set { FetchCustomerPermission(); _CustomerId = value; } }
        private int _CustomerId { get; set; }
        
        private int _UserId { get; set; }
        List<enmDocumentMaster> _DocumentPermission { get; set; }


        private bool _IsCurrentCustomerPermission;


        public CustomerMaster(DBContext context,int UserId)
        {
            _context = context;
            _UserId = UserId;
            _DocumentPermission = new List<enmDocumentMaster>();
        }

        private void FetchCustomerPermission()
        {
            var Customers=_context.tblUserMaster.Where(p => p.Id == _UserId )
                .Select(p =>new { p.CustomerId,p.tblCustomerMaster.CustomerType}).FirstOrDefault();
            if (Customers == null)
            {
                _IsCurrentCustomerPermission = false;
            }
            if (Customers.CustomerId == _CustomerId || Customers.CustomerType == enmCustomerType.Admin)
            {
                _IsCurrentCustomerPermission = true;
            }
            _DocumentPermission.Clear();
            if (_IsCurrentCustomerPermission)
            {
                List<int?> Role=_context.tblUserRole.Where(p => p.UserId == _UserId && !p.IsDeleted).Select(p => p.Role).ToList();
                _DocumentPermission.AddRange( _context.tblRoleClaim.Where(p => (int)p.ClaimId >= 10001 && (int)p.ClaimId < 10100 && Role.Contains(p.Role) && !p.IsDeleted).Select(p => p.ClaimId).Distinct());

            }
        }

        public mdlCustomerMaster FetchBasicDetail()
        {
            mdlCustomerMaster mdl=null;
            if (!_IsCurrentCustomerPermission)
            {
                return mdl;
            }
            //Check Read Permission
            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Read))
            {
                mdl = _context.tblCustomerMaster.Where(P => P.Id == _CustomerId).Select(p => new
                mdlCustomerMaster
                {
                    CustomerId= p.Id,
                    Code=p.Code,
                    CustomerName=p.CustomerName,
                    
                    Email=p.Email,
                    HaveGST=p.HaveGST,
                    Address = p.Address,
                    CountryId=p.CountryId,
                    StateId=p.StateId,
                    PinCode=p.PinCode,
                    ContactNo=p.ContactNo,
                    AlternateNo=p.AlternateNo,
                    CustomerType=p.CustomerType,
                    IsActive=p.IsActive,
                    ModifyBy=p.ModifyBy,
                    ModifyDt=p.ModifyDt
                }
                ).FirstOrDefault();
            }
            return mdl;
        }

        public mdlCustomerGSTDetails FetchGSTDetail()
        {
            mdlCustomerGSTDetails mdl = null;
            if (!_IsCurrentCustomerPermission)
            {
                return mdl;
            }
            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_GSTDetail_Read))
            {
                mdl=_context.tblCustomerGSTDetails.Where(p => p.CustomerId == _CustomerId && !p.IsDeleted)
                    .Select(p=> new mdlCustomerGSTDetails {
                        GSTDetailId=p.Id,
                        GstNumber=p.GstNumber,
                        Email=p.Email,
                        Address=p.Address,
                        CountryId=p.CountryId,
                        StateId=p.StateId,
                        CustomerId=p.CustomerId,
                        Mobile=p.Mobile,
                        PinCode=p.PinCode,
                        RegisteredName=p.RegisteredName
                    }).FirstOrDefault();
            }
            return mdl;
        }

        public List<mdlUserMaster> FetchUserMasters()
        {
            List<mdlUserMaster> mdl = new List<mdlUserMaster>();
            if (!_IsCurrentCustomerPermission)
            {
                return mdl;
            }
            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_UserDetail_Read))
            {
                mdl.AddRange(_context.tblUserMaster.Where(p => p.CustomerId == _CustomerId)
                    .Select(p=>new mdlUserMaster {
                        UserId=p.Id,
                        UserName=p.UserName,
                        Password=p.Password,
                        ConfirmPassword=p.Password,
                        IsActive = p.IsActive,
                        ForcePasswordChange=p.ForcePasswordChange,
                        Email=p.Email,
                        Phone=p.Phone,
                        IsBlocked=p.IsBlocked,
                        IsPrimary=p.IsPrimary,
                        BlockStartTime=p.BlockStartTime,
                        BlockEndTime=p.BlockEndTime,
                        CustomerId=p.CustomerId,
                        lastLogin=p.LastLogin,
                        Roles=p.tblUserRole.Select(p=>p.Role.Value).ToList()
                    }));
            }
            return mdl;
        }

        public mdlBanks FetchBanks()
        {
            mdlBanks mdl = null;
            if (!_IsCurrentCustomerPermission)
            {
                return mdl;
            }
            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Bank_Read))
            {
                mdl=_context.tblCustomerBankDetails.Where(p => p.CustomerId == _CustomerId && !p.IsDeleted)
                    .Select(p => new mdlBanks
                    {
                        CustomerId=p.CustomerId,
                        BankId=p.BankId??0,
                        IFSC=p.IFSC,
                        AccountNo =p.AccountNo,
                        BranchAddress=p.BranchAddress,
                        ApprovalRemarks=p.ApprovalRemarks,
                        IsApproved=p.IsApproved,
                        NameasonBank=p.NameasonBank,
                        Remarks=p.Remarks,
                        UpiId=p.UpiId
                    }).FirstOrDefault();
            }
            return mdl;
        }

        public mdlPan FetchPan()
        {
            mdlPan mdl = null;
            if (!_IsCurrentCustomerPermission)
            {
                return mdl;
            }
            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Pan_Read))
            {
                mdl=_context.tblCustomerPanDetails.Where(p => p.CustomerId == _CustomerId && !p.IsDeleted)
                    .Select(p => new mdlPan
                    {
                        CustomerId = p.CustomerId,
                        PANNo=p.PANNo,
                        PANName = p.PANName,                        
                        ApprovalRemarks = p.ApprovalRemarks,
                        IsApproved = p.IsApproved,                        
                        Remarks = p.Remarks,                        
                    }).FirstOrDefault();
            }
            return mdl;
        }

        public mdlCustomerSetting FetchSetting()
        {
            mdlCustomerSetting mdl = new mdlCustomerSetting();
            if (!_IsCurrentCustomerPermission)
            {
                return mdl;
            }
            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Setting_Read))
            {

                mdl.MinBalance = _context.tblWalletBalanceAlert.Where(p => p.CustomerId == _CustomerId && !p.IsDeleted ).FirstOrDefault()?.MinBalance??0;
                mdl.MarkupAmount = _context.tblCustomerMarkup.Where(p => p.CustomerId == _CustomerId && !p.IsDeleted).FirstOrDefault()?.MarkupAmt ?? 0;
                var IPFilter=_context.tblCustomerIPFilter.Where(p => p.CustomerId == _CustomerId && !p.IsDeleted).Include(p=>p.tblCustomerIPFilterDetails).FirstOrDefault();
                if (IPFilter != null)
                {
                    mdl.AllowedAllIp = IPFilter.AllowedAllIp;
                    mdl.IPAddess = string.Join(", ", IPFilter.tblCustomerIPFilterDetails.Select(p => p.IPAddress).ToList());

                }

            }
            return mdl;
        }

        public bool SaveCustomerMaster(mdlCustomerMaster mdl)
        {
            if (!_IsCurrentCustomerPermission)
            {
                return mdl;
            }
            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Write))
            {
                if (!TryValidateModel(Movie, nameof(Movie)))
                {
                    return Page();
                }
                //_context.tbl

            }   

        }



    }
}
