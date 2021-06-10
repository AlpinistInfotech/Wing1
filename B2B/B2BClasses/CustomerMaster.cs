using B2BClasses.Database;
using B2BClasses.Models;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using B2BClasses.Database.LogDatabase;
using System.Threading.Tasks;
namespace B2BClasses
{
    public class CustomerMaster
    {
        private readonly DBContext _context;
        private readonly LogDBContext _logDbContext;
        public int CustomerId { get {  return _CustomerId; } set { FetchCustomerPermission(); _CustomerId = value; } }
        private int _CustomerId { get; set; }
        
        private int _UserId { get; set; }
        List<enmDocumentMaster> _DocumentPermission { get; set; }
        public List<ValidationResult> _validationResultList = new List<ValidationResult>();
        private bool _IsCurrentCustomerPermission;
        

        public CustomerMaster(DBContext context, LogDBContext logDbContext,int UserId)
        {
            _context = context;
            _UserId = UserId;
            _logDbContext = logDbContext;
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
                    ModifyDt=p.ModifyDt,
                    Logo=p.Logo

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

        public async Task<bool> SaveBasicDetailsAsync(mdlCustomerMaster mdl)
        {
            bool IsUpdate = false,IsFoundUpdate=false;
            if (!_IsCurrentCustomerPermission)
            {
                _validationResultList.Add(new ValidationResult("Access Denied"));
                return false;                
            }
            
            
            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Write))
            {
                tblCustomerMaster CustomerMaster = null;
                if (_CustomerId > 0)
                {
                    CustomerMaster = _context.tblCustomerMaster.Where(p => p.Id == _CustomerId).FirstOrDefault();
                    if (CustomerMaster == null)
                    {
                        _validationResultList.Add(new ValidationResult("Invalid Customer Id"));
                        return false;
                    }
                    IsUpdate = true;
                        
                }                
                else
                {
                    CustomerMaster = new tblCustomerMaster();
                }

                if ( IsUpdate && ((!CustomerMaster.CustomerName.Trim().Equals(mdl.CustomerName?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (!CustomerMaster.Email.Trim().Equals(mdl.Email?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    CustomerMaster.HaveGST != mdl.HaveGST || CustomerMaster.CountryId != mdl.CountryId || CustomerMaster.StateId != mdl.StateId ||
                    (!CustomerMaster.Address.Trim().Equals(mdl.Address?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (!CustomerMaster.PinCode.Trim().Equals(mdl.PinCode?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (!CustomerMaster.ContactNo.Trim().Equals(mdl.ContactNo?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (!CustomerMaster.AlternateNo.Trim().Equals(mdl.AlternateNo?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    CustomerMaster.CustomerType != mdl.CustomerType || CustomerMaster.IsActive != mdl.IsActive)
                    )
                {
                    IsFoundUpdate = true;
                    _logDbContext.tblCustomerMasterLog.Add(new tblCustomerMasterLog() {
                        CustomerId =CustomerMaster.Id,
                        Code= CustomerMaster.Code,
                        CustomerName= CustomerMaster.CustomerName,
                        Logo= CustomerMaster.Logo,
                        Email= CustomerMaster.Email,
                        HaveGST= CustomerMaster.HaveGST,
                        Address= CustomerMaster.Address,
                        CountryId= CustomerMaster.CountryId,
                        StateId = CustomerMaster.StateId,
                        PinCode= CustomerMaster.PinCode,
                        ContactNo= CustomerMaster.ContactNo,
                        AlternateNo= CustomerMaster.AlternateNo,
                        CustomerType= CustomerMaster.CustomerType,
                        IsActive= CustomerMaster.IsActive,
                        CreatedBy= CustomerMaster.ModifyBy,
                        CreatedDt = CustomerMaster.ModifyDt,
                        ModifyBy=_UserId,
                        ModifyDt=DateTime.Now});
                   await _logDbContext.SaveChangesAsync();

                }
                if ((!IsUpdate) || (IsUpdate && IsFoundUpdate))
                {
                    CustomerMaster.Code = mdl.Code;
                    CustomerMaster.CustomerName = mdl.CustomerName;
                    CustomerMaster.Logo = mdl.Logo;
                    CustomerMaster.Email = mdl.Email;
                    CustomerMaster.HaveGST = mdl.HaveGST;
                    CustomerMaster.Address = mdl.Address;
                    CustomerMaster.CountryId = mdl.CountryId;
                    CustomerMaster.StateId = mdl.StateId;
                    CustomerMaster.PinCode  = mdl.PinCode;
                    CustomerMaster.ContactNo = mdl.ContactNo;
                    CustomerMaster.AlternateNo = mdl.AlternateNo;
                    CustomerMaster.CustomerType = mdl.CustomerType;
                    CustomerMaster.IsActive = mdl.IsActive;
                    CustomerMaster.ModifyBy = _UserId;
                    CustomerMaster.ModifyDt = DateTime.Now;

                }

                if (!IsUpdate)
                {
                    CustomerMaster.CreatedBy = _UserId;
                    CustomerMaster.CreatedDt = DateTime.Now;
                    _context.tblCustomerMaster.Add(CustomerMaster);
                    
                }
                else
                {
                    _context.tblCustomerMaster.UpdateRange(CustomerMaster);
                }
               await _context.SaveChangesAsync();

            }
            return false;

        }


    }
}
