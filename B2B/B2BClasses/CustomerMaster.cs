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
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Storage;

namespace B2BClasses
{
    
    
    public interface ICustomerMaster
    {
        int CustomerId { get; set; }
        List<enmDocumentMaster> DocumentPermission { get; }
        bool IsCurrentCustomerPermission { get; }
        List<ValidationResult> validationResultList { get; }

        void BeginTransaction();
        void ComitTransaction();
        Dictionary<int, string> FetchAllCustomer(bool IncludeAdmin = false, bool OnlyActive = false);
        mdlBanks FetchBanks();
        mdlCustomerMaster FetchBasicDetail();
        mdlCustomerGSTDetails FetchGSTDetail();
        mdlPan FetchPan();
        mdlCustomerSetting FetchSetting();
        List<mdlUserMaster> FetchUserMasters();
        mdlUserMaster FetchUserMasters(int userId);
        void RollbackTransaction();
        Task<bool> SaveBankDetailsAsync(mdlBanks mdl);
        Task<bool> SaveBasicDetailsAsync(mdlCustomerMaster mdl);
        Task<bool> SaveGSTDetailsAsync(mdlCustomerGSTDetails mdl);
        Task<bool> SavePanDetailsAsync(mdlPan mdl);
        Task<bool> SaveSettingDetailsAsync(mdlCustomerSetting mdl);
        Task<bool> SaveUserDetailsAsync(mdlUserMaster mdl);
    }

    public class CustomerMaster : ICustomerMaster
    {

        private int _CustomerId, _UserId;
        private bool _IsCurrentCustomerPermission;
        private readonly DBContext _context;
        private readonly LogDBContext _logDbContext;
        private readonly IConfiguration _config;
        private List<ValidationResult> _validationResultList = new List<ValidationResult>();
        private List<enmDocumentMaster> _DocumentPermission { get; set; }
        private IDbContextTransaction _transaction;

        public int CustomerId { get { return _CustomerId; } set { FetchCustomerPermission(); _CustomerId = value; } }
        public List<ValidationResult> validationResultList { get { return _validationResultList; } }
        public List<enmDocumentMaster> DocumentPermission { get { return _DocumentPermission; } }
        public bool IsCurrentCustomerPermission { get { return _IsCurrentCustomerPermission; } }

        public CustomerMaster(DBContext context, LogDBContext logDbContext, ISettings settings, IConfiguration config, int UserId)
        {
            _config = config;
            _context = context;
            _UserId = UserId;
            _logDbContext = logDbContext;
            _DocumentPermission = new List<enmDocumentMaster>();
        }

        private void FetchCustomerPermission()
        {
            var Customers = _context.tblUserMaster.Where(p => p.Id == _UserId)
                .Select(p => new { p.CustomerId, p.tblCustomerMaster.CustomerType }).FirstOrDefault();
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
                List<int?> Role = _context.tblUserRole.Where(p => p.UserId == _UserId).Select(p => p.Role).ToList();
                _DocumentPermission.AddRange(_context.tblRoleClaim.Where(p => (int)p.ClaimId >= 10001 && (int)p.ClaimId < 10100 && Role.Contains(p.Role) && !p.IsDeleted).Select(p => p.ClaimId).Distinct());

            }
        }

        public Dictionary<int, string> FetchAllCustomer(bool IncludeAdmin = false, bool OnlyActive = false)
        {
            var CustomerMaster = _context.tblCustomerMaster.AsQueryable();
            if (!IncludeAdmin)
            {
                CustomerMaster = CustomerMaster.Where(p => p.CustomerType != enmCustomerType.Admin);
            }
            if (OnlyActive)
            {
                CustomerMaster = CustomerMaster.Where(p => p.IsActive);
            }

            return CustomerMaster.OrderBy(P => P.Code).ThenBy(P => P.CustomerName).ToDictionary(x => x.Id, x => string.Concat(x.Code, " - ", x.CustomerName));

        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void ComitTransaction()
        {
            _transaction.Commit();
        }
        public void RollbackTransaction()
        {
            _transaction.Rollback();
        }


        public mdlCustomerMaster FetchBasicDetail()
        {
            mdlCustomerMaster mdl = null;
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
                    CustomerId = p.Id,
                    Code = p.Code,
                    CustomerName = p.CustomerName,
                    Email = p.Email,
                    HaveGST = p.HaveGST,
                    Address = p.Address,
                    CountryId = p.CountryId,
                    StateId = p.StateId,
                    PinCode = p.PinCode,
                    ContactNo = p.ContactNo,
                    AlternateNo = p.AlternateNo,
                    CustomerType = p.CustomerType,
                    IsActive = p.IsActive,
                    ModifyBy = p.ModifyBy,
                    ModifyDt = p.ModifyDt,
                    Logo = p.Logo

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
                mdl = _context.tblCustomerGSTDetails.Where(p => p.CustomerId == _CustomerId)
                    .Select(p => new mdlCustomerGSTDetails
                    {
                        GstNumber = p.GstNumber,
                        Email = p.Email,
                        Address = p.Address,
                        CountryId = p.CountryId,
                        StateId = p.StateId,
                        CustomerId = p.CustomerId,
                        Mobile = p.Mobile,
                        PinCode = p.PinCode,
                        RegisteredName = p.RegisteredName
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
                    .Select(p => new mdlUserMaster
                    {
                        UserId = p.Id,
                        UserName = p.UserName,
                        Password = p.Password,
                        ConfirmPassword = p.Password,
                        IsActive = p.IsActive,
                        ForcePasswordChange = p.ForcePasswordChange,
                        Email = p.Email,
                        Phone = p.Phone,
                        IsBlocked = p.IsBlocked,
                        IsPrimary = p.IsPrimary,
                        BlockStartTime = p.BlockStartTime,
                        BlockEndTime = p.BlockEndTime,
                        CustomerId = p.CustomerId,
                        lastLogin = p.LastLogin,
                        Roles = p.tblUserRole.Select(p => p.Role.Value).ToList()
                    }));
            }
            return mdl;
        }

        public mdlUserMaster FetchUserMasters(int userId)
        {

            mdlUserMaster mdl = new mdlUserMaster();
            if (userId == 0) return mdl;

            if (!_IsCurrentCustomerPermission)
            {
                return mdl;
            }
            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_UserDetail_Read))
            {
                mdl = (_context.tblUserMaster.Where(p => p.CustomerId == _CustomerId && p.Id == userId)
                    .Select(p => new mdlUserMaster
                    {
                        UserId = p.Id,
                        UserName = p.UserName,
                        Password = p.Password,
                        ConfirmPassword = p.Password,
                        Oldpassword=p.Password,
                        IsActive = p.IsActive,
                        ForcePasswordChange = p.ForcePasswordChange,
                        Email = p.Email,
                        Phone = p.Phone,
                        IsBlocked = p.IsBlocked,
                        IsPrimary = p.IsPrimary,
                        BlockStartTime = p.BlockStartTime,
                        BlockEndTime = p.BlockEndTime,
                        CustomerId = p.CustomerId,
                        lastLogin = p.LastLogin,
                        Roles = p.tblUserRole.Select(p => p.Role.Value).ToList()
                    })).FirstOrDefault();
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
                mdl = _context.tblCustomerBankDetails.Where(p => p.CustomerId == _CustomerId)
                    .Select(p => new mdlBanks
                    {
                        CustomerId = p.CustomerId,
                        BankId = p.BankId ?? 0,
                        IFSC = p.IFSC,
                        AccountNo = p.AccountNo,
                        BranchAddress = p.BranchAddress,
                        ApprovalRemarks = p.ApprovalRemarks,
                        IsApproved = p.IsApproved,
                        NameasonBank = p.NameasonBank,
                        Remarks = p.Remarks,
                        UpiId = p.UpiId
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
                mdl = _context.tblCustomerPanDetails.Where(p => p.CustomerId == _CustomerId)
                    .Select(p => new mdlPan
                    {
                        CustomerId = p.CustomerId,
                        PANNo = p.PANNo,
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

                mdl.MinBalance = _context.tblWalletBalanceAlert.Where(p => p.CustomerId == _CustomerId).FirstOrDefault()?.MinBalance ?? 0;
                mdl.MarkupAmount = _context.tblCustomerMarkup.Where(p => p.CustomerId == _CustomerId).FirstOrDefault()?.MarkupAmt ?? 0;
                var IPFilter = _context.tblCustomerIPFilter.Where(p => p.CustomerId == _CustomerId).Include(p => p.tblCustomerIPFilterDetails).FirstOrDefault();
                if (IPFilter != null)
                {
                    mdl.AllowedAllIp = IPFilter.AllowedAllIp;
                    mdl.IPAddess = string.Join(", ", IPFilter.tblCustomerIPFilterDetails.Select(p => p.IPAddress).ToList());

                }
                mdl.MPin = _context.tblCustomerBalance.Where(p => p.CustomerId == _CustomerId).FirstOrDefault()?.MPin ?? Settings.Decrypt("0000");

            }
            return mdl;
        }

        public async Task<bool> SaveBasicDetailsAsync(mdlCustomerMaster mdl)
        {
            bool IsUpdate = false, IsFoundUpdate = false;
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

                if (IsUpdate && ((!CustomerMaster.CustomerName.Trim().Equals(mdl.CustomerName?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
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
                    _logDbContext.tblCustomerMasterLog.Add(new tblCustomerMasterLog()
                    {
                        CustomerId = CustomerMaster.Id,
                        Code = CustomerMaster.Code,
                        CustomerName = CustomerMaster.CustomerName,
                        Logo = CustomerMaster.Logo,
                        Email = CustomerMaster.Email,
                        HaveGST = CustomerMaster.HaveGST,
                        Address = CustomerMaster.Address,
                        CountryId = CustomerMaster.CountryId,
                        StateId = CustomerMaster.StateId,
                        PinCode = CustomerMaster.PinCode,
                        ContactNo = CustomerMaster.ContactNo,
                        AlternateNo = CustomerMaster.AlternateNo,
                        CustomerType = CustomerMaster.CustomerType,
                        IsActive = CustomerMaster.IsActive,
                        CreatedBy = CustomerMaster.ModifyBy,
                        CreatedDt = CustomerMaster.ModifyDt,
                        ModifyBy = _UserId,
                        ModifyDt = DateTime.Now
                    });
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
                    CustomerMaster.PinCode = mdl.PinCode;
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
                    await _context.SaveChangesAsync();
                    _CustomerId = CustomerMaster.Id;
                }
                else
                {
                    _context.tblCustomerMaster.UpdateRange(CustomerMaster);
                    await _context.SaveChangesAsync();
                }


                return true;

            }
            return false;

        }

        public async Task<bool> SaveGSTDetailsAsync(mdlCustomerGSTDetails mdl)
        {
            bool IsUpdate = false, IsFoundUpdate = false;
            if (!_IsCurrentCustomerPermission)
            {
                _validationResultList.Add(new ValidationResult("Access Denied"));
                return false;
            }
            if (mdl == null)
            {
                return false;
            }


            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_GSTDetail_Write))
            {
                tblCustomerGSTDetails saveData = null;
                if (_CustomerId > 0)
                {
                    saveData = _context.tblCustomerGSTDetails.Where(p => p.CustomerId == _CustomerId).FirstOrDefault();
                    if (saveData == null)
                    {
                        saveData = new tblCustomerGSTDetails();
                    }
                    else
                    {
                        IsUpdate = true;
                    }


                    if (IsUpdate && ((!saveData.RegisteredName.Trim().Equals(mdl.CustomerName?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (!saveData.Email.Trim().Equals(mdl.Email?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    saveData.CountryId != mdl.CountryId || saveData.StateId != mdl.StateId ||
                    (!saveData.Address.Trim().Equals(mdl.Address?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (!saveData.PinCode.Trim().Equals(mdl.PinCode?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (!saveData.Mobile.Trim().Equals(mdl.Mobile?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (!saveData.GstNumber.Trim().Equals(mdl.GstNumber?.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                    )
                    {
                        IsFoundUpdate = true;
                        _logDbContext.tblCustomerGSTDetailsLog.Add(new tblCustomerGSTDetailsLog()
                        {

                            CustomerId = saveData.CustomerId,
                            RegisteredName = saveData.RegisteredName,
                            GstNumber = saveData.GstNumber,
                            Email = saveData.Email,
                            Address = saveData.Address,
                            CountryId = saveData.CountryId,
                            StateId = saveData.StateId,
                            PinCode = saveData.PinCode,
                            Mobile = saveData.Mobile,
                            CreatedBy = saveData.ModifiedBy ?? 0,
                            CreatedDt = saveData.ModifiedDt ?? DateTime.Now,
                            ModifiedBy = _UserId,
                            ModifiedDt = DateTime.Now
                        });
                        await _logDbContext.SaveChangesAsync();

                    }

                    if ((!IsUpdate) || (IsUpdate && IsFoundUpdate))
                    {
                        saveData.GstNumber = mdl.GstNumber;
                        saveData.RegisteredName = mdl.RegisteredName;
                        saveData.Email = mdl.Email;
                        saveData.Address = mdl.Address;
                        saveData.CountryId = mdl.CountryId;
                        saveData.StateId = mdl.StateId;
                        saveData.PinCode = mdl.PinCode;
                        saveData.Mobile = mdl.Mobile;
                        saveData.ModifiedBy = _UserId;
                        saveData.ModifiedDt = DateTime.Now;
                        saveData.CustomerId = _CustomerId;

                    }
                    if (!IsUpdate)
                    {
                        saveData.CreatedBy = _UserId;
                        saveData.CreatedDt = DateTime.Now;
                        _context.tblCustomerGSTDetails.Add(saveData);
                    }
                    else
                    {
                        _context.tblCustomerGSTDetails.UpdateRange(saveData);
                    }
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;

        }

        public async Task<bool> SaveBankDetailsAsync(mdlBanks mdl)
        {
            bool IsUpdate = false, IsFoundUpdate = false;
            if (!_IsCurrentCustomerPermission)
            {
                _validationResultList.Add(new ValidationResult("Access Denied"));
                return false;
            }
            if (mdl == null)
            {
                return false;
            }


            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Bank_Write))
            {
                tblCustomerBankDetails saveData = null;
                if (_CustomerId > 0)
                {
                    saveData = _context.tblCustomerBankDetails.Where(p => p.CustomerId == _CustomerId).FirstOrDefault();
                    if (saveData == null)
                    {
                        saveData = new tblCustomerBankDetails();
                    }
                    else
                    {
                        IsUpdate = true;
                    }


                    if (IsUpdate && ((!saveData.NameasonBank.Trim().Equals(mdl.NameasonBank?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (!saveData.AccountNo.Trim().Equals(mdl.AccountNo?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (!saveData.IFSC.Trim().Equals(mdl.IFSC?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    saveData.BankId != mdl.BankId ||
                    (!saveData.BranchAddress.Trim().Equals(mdl.BranchAddress?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    //(!saveData.Remarks.Trim().Equals(mdl.Remarks?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                   // (!saveData.ApprovalRemarks.Trim().Equals(mdl.ApprovalRemarks?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    saveData.IsApproved != mdl.IsApproved)
                    )
                    {
                        IsFoundUpdate = true;
                        _logDbContext.tblCustomerBankDetailsLog.Add(new tblCustomerBankDetailsLog()
                        {

                            NameasonBank = saveData.NameasonBank,
                            BankId = saveData.BankId,
                            IFSC = saveData.IFSC,
                            AccountNo = saveData.AccountNo,
                            BranchAddress = saveData.BranchAddress,
                            ApprovalRemarks = saveData.ApprovalRemarks,
                            Remarks = saveData.Remarks,
                            ApprovedBy = saveData.ApprovedBy,
                            ApprovedDt = saveData.ApprovedDt,
                            CustomerId = saveData.CustomerId,
                            IsApproved = saveData.IsApproved,
                            UpiId = saveData.UpiId,
                            CreatedBy = _UserId,
                            CreatedDt = DateTime.Now,
                            UploadImages = saveData.UploadImages

                        });
                        await _logDbContext.SaveChangesAsync();

                    }

                    if ((!IsUpdate) || (IsUpdate && IsFoundUpdate))
                    {
                        saveData.NameasonBank = mdl.NameasonBank;
                        saveData.BankId = mdl.BankId;
                        saveData.IFSC = mdl.IFSC;
                        saveData.AccountNo = mdl.AccountNo;
                        saveData.BranchAddress = mdl.BranchAddress;
                        saveData.ApprovalRemarks = mdl.ApprovalRemarks;
                        saveData.Remarks = mdl.Remarks;
                        saveData.ApprovedBy = _UserId;
                        saveData.ApprovedDt = DateTime.Now;
                        saveData.CustomerId = _CustomerId;
                        saveData.IsApproved = mdl.IsApproved;
                        saveData.UpiId = mdl.UpiId;
                        saveData.CreatedBy = _UserId;
                        saveData.CreatedDt = DateTime.Now;
                        saveData.UploadImages = string.Empty;

                    }
                    if (!IsUpdate)
                    {
                        saveData.CreatedBy = _UserId;
                        saveData.CreatedDt = DateTime.Now;
                        _context.tblCustomerBankDetails.Add(saveData);
                    }
                    else
                    {
                        _context.tblCustomerBankDetails.UpdateRange(saveData);
                    }
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;

        }

        public async Task<bool> SavePanDetailsAsync(mdlPan mdl)
        {
            bool IsUpdate = false, IsFoundUpdate = false;
            if (!_IsCurrentCustomerPermission)
            {
                _validationResultList.Add(new ValidationResult("Access Denied"));
                return false;
            }
            if (mdl == null)
            {
                return false;
            }


            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Pan_Write))
            {
                tblCustomerPanDetails saveData = null;
                if (_CustomerId > 0)
                {
                    saveData = _context.tblCustomerPanDetails.Where(p => p.CustomerId == _CustomerId).FirstOrDefault();
                    if (saveData == null)
                    {
                        saveData = new tblCustomerPanDetails();
                    }
                    else
                    {
                        IsUpdate = true;
                    }


                    if (IsUpdate && ((!saveData.PANName.Trim().Equals(mdl.PANName?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    (!saveData.PANNo.Trim().Equals(mdl.PANNo?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                 //   (!saveData.Remarks.Trim().Equals(mdl.Remarks?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                 //   (!saveData.ApprovalRemarks.Trim().Equals(mdl.ApprovalRemarks?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                    saveData.IsApproved != mdl.IsApproved)
                    )
                    {
                        IsFoundUpdate = true;
                        _logDbContext.tblCustomerPanDetailsLog.Add(new tblCustomerPanDetailsLog()
                        {

                            PANName = saveData.PANName,
                            PANNo = saveData.PANNo,
                            ApprovalRemarks = saveData.ApprovalRemarks,
                            Remarks = saveData.Remarks,
                            ApprovedBy = saveData.ApprovedBy,
                            ApprovedDt = saveData.ApprovedDt,
                            CustomerId = saveData.CustomerId,
                            IsApproved = saveData.IsApproved,
                            CreatedBy = _UserId,
                            CreatedDt = DateTime.Now,
                            UploadImages = saveData.UploadImages

                        });
                        await _logDbContext.SaveChangesAsync();

                    }

                    if ((!IsUpdate) || (IsUpdate && IsFoundUpdate))
                    {
                        saveData.PANName = mdl.PANName;
                        saveData.PANNo = mdl.PANNo;
                        saveData.ApprovalRemarks = mdl.ApprovalRemarks;
                        saveData.Remarks = mdl.Remarks;
                        saveData.ApprovedBy = _UserId;
                        saveData.ApprovedDt = DateTime.Now;
                        saveData.CustomerId = _CustomerId;
                        saveData.IsApproved = mdl.IsApproved;
                        saveData.CreatedBy = _UserId;
                        saveData.CreatedDt = DateTime.Now;
                        saveData.UploadImages = string.Empty;

                    }
                    if (!IsUpdate)
                    {
                        saveData.CreatedBy = _UserId;
                        saveData.CreatedDt = DateTime.Now;
                        _context.tblCustomerPanDetails.Add(saveData);
                    }
                    else
                    {
                        _context.tblCustomerPanDetails.UpdateRange(saveData);
                    }
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;

        }

        private bool IsAllIPEqual(List<string> IPLIst, string IPS)
        {
            bool AllIPEqual = true;
            var NewIPList = IPS?.Split(",").ToList();
            if (NewIPList == null)
            {
                NewIPList = new List<string>();
            }
            if (IPLIst == null)
            {
                IPLIst = new List<string>();
            }

            if (IPLIst.Count != NewIPList.Count)
            {
                return false;
            }

            foreach (var ips in IPLIst)
            {
                AllIPEqual = NewIPList.Any(p => p.Trim().ToLower() == ips.Trim().ToLower());
                if (!AllIPEqual)
                {
                    return false;
                }
            }
            return AllIPEqual;
        }

        public async Task<bool> SaveSettingDetailsAsync(mdlCustomerSetting mdl)
        {
            bool IsUpdate = false, IsFoundUpdate = false;
            if (!_IsCurrentCustomerPermission)
            {
                _validationResultList.Add(new ValidationResult("Access Denied"));
                return false;
            }
            if (mdl == null)
            {
                return false;
            }


            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Setting_Write))
            {
                tblCustomerMarkup saveDataMarkup = null;
                tblWalletBalanceAlert saveDataWalletAlert = null;
                tblCustomerIPFilter saveDataIPFilter = null;
                if (_CustomerId > 0)
                {
                    //Save Markup
                    {
                        saveDataMarkup = _context.tblCustomerMarkup.Where(p => p.CustomerId == _CustomerId).FirstOrDefault();
                        if (saveDataMarkup == null)
                        {
                            saveDataMarkup = new tblCustomerMarkup();
                        }
                        else
                        {
                            IsUpdate = true;
                        }

                        if (IsUpdate && (saveDataMarkup.MarkupAmt != mdl.MarkupAmount))
                        {
                            IsFoundUpdate = true;
                            _logDbContext.tblCustomerMarkupLog.Add(new tblCustomerMarkupLog()
                            {
                                CreatedBy = saveDataMarkup.ModifiedBy ?? _UserId,
                                CreatedDt = saveDataMarkup.ModifiedDt ?? DateTime.Now,
                                CustomerId = _CustomerId,
                                MarkupAmt = mdl.MarkupAmount,
                                //MinBalance=mdl.MinBalance,
                                ModifiedBy = _UserId,
                                ModifiedDt = DateTime.Now

                            });
                            await _logDbContext.SaveChangesAsync();

                        }

                        if ((!IsUpdate) || (IsUpdate && IsFoundUpdate))
                        {
                            saveDataMarkup.MarkupAmt = mdl.MarkupAmount;
                            saveDataMarkup.CustomerId = _CustomerId;
                            saveDataMarkup.ModifiedBy = _UserId;
                            saveDataMarkup.ModifiedDt = DateTime.Now;

                        }
                        if (!IsUpdate)
                        {
                            saveDataMarkup.CreatedBy = _UserId;
                            saveDataMarkup.CreatedDt = DateTime.Now;
                            _context.tblCustomerMarkup.Add(saveDataMarkup);
                        }
                        else
                        {
                            _context.tblCustomerMarkup.UpdateRange(saveDataMarkup);
                        }
                    }

                    //Min balaence Alert 
                    {
                        IsUpdate = false; IsFoundUpdate = false;
                        saveDataWalletAlert = _context.tblWalletBalanceAlert.Where(p => p.CustomerId == _CustomerId).FirstOrDefault();
                        if (saveDataWalletAlert == null)
                        {
                            saveDataWalletAlert = new tblWalletBalanceAlert();
                        }
                        else
                        {
                            IsUpdate = true;
                        }

                        if (IsUpdate && (saveDataWalletAlert.MinBalance != mdl.MinBalance))
                        {
                            IsFoundUpdate = true;
                            _logDbContext.tblWalletBalanceAlertLog.Add(new tblWalletBalanceAlertLog()
                            {
                                CreatedBy = saveDataWalletAlert.ModifiedBy ?? _UserId,
                                CreatedDt = saveDataWalletAlert.ModifiedDt ?? DateTime.Now,
                                CustomerId = _CustomerId,
                                MinBalance = mdl.MinBalance,
                                ModifiedBy = _UserId,
                                ModifiedDt = DateTime.Now

                            });
                            await _logDbContext.SaveChangesAsync();

                        }

                        if ((!IsUpdate) || (IsUpdate && IsFoundUpdate))
                        {
                            saveDataWalletAlert.MinBalance = mdl.MinBalance;
                            saveDataWalletAlert.CustomerId = _CustomerId;
                            saveDataWalletAlert.ModifiedBy = _UserId;
                            saveDataWalletAlert.ModifiedDt = DateTime.Now;

                        }
                        if (!IsUpdate)
                        {
                            saveDataWalletAlert.CreatedBy = _UserId;
                            saveDataWalletAlert.CreatedDt = DateTime.Now;
                            _context.tblWalletBalanceAlert.Add(saveDataWalletAlert);
                        }
                        else
                        {
                            _context.tblWalletBalanceAlert.UpdateRange(saveDataWalletAlert);
                        }

                        IsUpdate = false;
                        var ExistingMpin = _context.tblCustomerBalance.Where(p => p.CustomerId == _CustomerId).FirstOrDefault();
                        string encryptpin = Settings.Encrypt(mdl.MPin);
                        if (ExistingMpin == null)
                        {
                            ExistingMpin = new tblCustomerBalance() { CustomerId = _CustomerId, CreditBalance = 0, WalletBalance = 0, MPin = encryptpin, ModifiedDt = DateTime.Now };
                            _context.tblCustomerBalance.Add(ExistingMpin);
                            _context.SaveChanges();
                        }
                        else
                        {
                            if (ExistingMpin.MPin != encryptpin)
                            {
                                ExistingMpin.MPin = encryptpin;
                                ExistingMpin.ModifiedDt = DateTime.Now;
                                _context.tblCustomerBalance.Update(ExistingMpin);
                                _context.SaveChanges();
                            }

                        }




                    }

                    //IP Filter Setting
                    {
                        IsUpdate = false; IsFoundUpdate = false;
                        saveDataIPFilter = _context.tblCustomerIPFilter.Where(p => p.CustomerId == _CustomerId)
                            .Include(p => p.tblCustomerIPFilterDetails).FirstOrDefault();
                        if (saveDataIPFilter == null)
                        {
                            saveDataIPFilter = new tblCustomerIPFilter();
                            saveDataIPFilter.tblCustomerIPFilterDetails = new List<tblCustomerIPFilterDetails>();

                        }
                        else
                        {
                            IsUpdate = true;
                        }

                        if (IsUpdate && ((saveDataIPFilter.AllowedAllIp != mdl.AllowedAllIp) ||
                            !IsAllIPEqual(saveDataIPFilter.tblCustomerIPFilterDetails.Select(p => p.IPAddress).ToList(), mdl.IPAddess)
                            ))
                        {
                            IsFoundUpdate = true;
                            _logDbContext.tblCustomerIPFilterLog.Add(new tblCustomerIPFilterLog()
                            {
                                CreatedBy = saveDataWalletAlert.ModifiedBy ?? _UserId,
                                CreatedDt = saveDataWalletAlert.ModifiedDt ?? DateTime.Now,
                                CustomerId = _CustomerId,
                                AllowedAllIp = mdl.AllowedAllIp,
                                ModifiedBy = _UserId,
                                ModifiedDt = DateTime.Now,
                                IPDetails = string.Join(",", saveDataIPFilter.tblCustomerIPFilterDetails.Select(p => p.IPAddress))

                            });
                            await _logDbContext.SaveChangesAsync();

                        }

                        if ((!IsUpdate) || (IsUpdate && IsFoundUpdate))
                        {

                            saveDataIPFilter.AllowedAllIp = mdl.AllowedAllIp;
                            saveDataIPFilter.CustomerId = _CustomerId;
                            saveDataIPFilter.ModifiedBy = _UserId;
                            saveDataIPFilter.ModifiedDt = DateTime.Now;

                            if (!mdl.AllowedAllIp)
                            {
                                var IPAddressDatas = mdl.IPAddess?.Split(",").Select(p => new tblCustomerIPFilterDetails { CustomerId = _CustomerId, IPAddress = p.Trim() }).ToList();
                                foreach (var ipAdd in IPAddressDatas)
                                {
                                    saveDataIPFilter.tblCustomerIPFilterDetails.Add(ipAdd);
                                }
                            }

                        }
                        if (!IsUpdate)
                        {
                            saveDataIPFilter.CreatedBy = _UserId;
                            saveDataIPFilter.CreatedDt = DateTime.Now;
                            _context.tblCustomerIPFilter.Add(saveDataIPFilter);
                        }
                        else
                        {
                            _context.tblCustomerIPFilterDetails.RemoveRange(_context.tblCustomerIPFilterDetails.Where(p => p.CustomerId == _CustomerId));
                            _context.tblCustomerIPFilter.UpdateRange(saveDataIPFilter);
                        }
                    }



                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;

        }

        private bool IsAllRoleEqual(List<int> roleList, List<int> mdlRoleList)
        {
            bool AllIPEqual = true;

            foreach (var rl in roleList)
            {
                AllIPEqual = mdlRoleList.Any(p => p == rl);
                if (!AllIPEqual)
                {
                    return false;
                }
            }
            return AllIPEqual;
        }

        public async Task<bool> SaveUserDetailsAsync(mdlUserMaster mdl)
        {
            bool IsUpdate = false, IsFoundUpdate = false;
            if (!_IsCurrentCustomerPermission)
            {
                _validationResultList.Add(new ValidationResult("Access Denied"));
                return false;
            }
            if (mdl == null)
            {
                return false;
            }


            if (_DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_UserDetail_Write))
            {
                tblUserMaster saveData = null;
                if (_CustomerId > 0)
                {
                    if (mdl.UserId > 0)
                    {
                        saveData = _context.tblUserMaster.Where(p => p.CustomerId == _CustomerId && p.Id == mdl.UserId).Include(q => q.tblUserRole)
                            .FirstOrDefault();
                        if (saveData == null)
                        {
                            saveData = new tblUserMaster();
                        }
                        else
                        {
                            IsUpdate = true;
                        }


                        if (IsUpdate && ((!saveData.UserName.Trim().Equals(mdl.UserName?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                        (!saveData.Password.Trim().Equals(mdl.Password?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                        (!saveData.Email.Trim().Equals(mdl.Email?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                        DateTime.Compare(saveData.BlockEndTime, mdl.BlockEndTime) == 0 ||
                        (!saveData.Phone.Trim().Equals(mdl.Phone?.Trim(), StringComparison.CurrentCultureIgnoreCase)) ||
                        saveData.IsActive != mdl.IsActive || saveData.IsBlocked != mdl.IsBlocked ||
                        saveData.ForcePasswordChange != mdl.ForcePasswordChange ||
                        !IsAllRoleEqual(saveData.tblUserRole.Select(r => r.Role.Value).ToList(), mdl.Roles))
                        )
                        {
                            IsFoundUpdate = true;
                            _logDbContext.tblUserMasterLog.Add(new tblUserMasterLog()
                            {
                                UserId = saveData.Id,
                                UserName = saveData.UserName,
                                Password = saveData.Password,
                                IsActive = saveData.IsActive,
                                ForcePasswordChange = saveData.ForcePasswordChange,
                                Email = saveData.Email,
                                IsMailVarified = saveData.IsMailVarified,
                                CustomerId = saveData.CustomerId,
                                Phone = saveData.Phone,
                                IsBlocked = saveData.IsBlocked,
                                CreatedBy = saveData.ModifiedBy ?? 0,
                                CreatedDt = saveData.ModifiedDt ?? DateTime.Now,
                                IsPrimary = saveData.IsPrimary,
                                BlockStartTime = saveData.BlockStartTime,
                                BlockEndTime = saveData.BlockEndTime,
                                LoginFailCount = saveData.LoginFailCount,
                                LastLogin = saveData.LastLogin,
                                ModifiedBy = _UserId,
                                ModifiedDt = saveData.ModifiedDt,
                                Roles = string.Join(",", saveData.tblUserRole.Select(p => p.Role))

                            });
                            await _logDbContext.SaveChangesAsync();

                        }

                        if ((!IsUpdate) || (IsUpdate && IsFoundUpdate))
                        {

                            saveData.UserName = mdl.UserName;
                            saveData.Password = mdl.Password;
                            saveData.IsActive = mdl.IsActive;
                            saveData.ForcePasswordChange = mdl.ForcePasswordChange;
                            saveData.Email = mdl.Email;
                            saveData.CustomerId = mdl.CustomerId;
                            saveData.Phone = mdl.Phone;
                            saveData.IsBlocked = mdl.IsBlocked;
                            saveData.IsPrimary = mdl.IsPrimary;
                            saveData.BlockStartTime = mdl.BlockStartTime;
                            saveData.BlockEndTime = mdl.BlockEndTime;
                            saveData.ModifiedBy = _UserId;
                            saveData.ModifiedDt = DateTime.Now;
                            foreach (var rl in mdl.Roles)
                            {
                                saveData.tblUserRole.Add(new tblUserRole() { Role = rl, CreatedBy = _UserId, CreatedDt = DateTime.Now });
                            }
                        }
                        if (!IsUpdate)
                        {
                            int ExpiryTime = 0;
                            int.TryParse(_config["UserSetting:MailTokenExpiryTime"], out ExpiryTime);
                            saveData.MailVerficationTokken = Settings.Encrypt(Settings.TokenGenrate());
                            saveData.TokkenExpiryTime = DateTime.Now.AddMinutes(ExpiryTime);
                            saveData.IsMailVarified = false;
                            saveData.LoginFailCount = 0;
                            saveData.CreatedBy = _UserId;
                            saveData.CreatedDt = DateTime.Now;
                            _context.tblUserMaster.Add(saveData);
                        }
                        else
                        {
                            _context.tblUserRole.RemoveRange(_context.tblUserRole.Where(p => p.UserId == mdl.UserId));
                            _context.tblUserMaster.UpdateRange(saveData);
                        }
                    }
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;

        }

    }
}
