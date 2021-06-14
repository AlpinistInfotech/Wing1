using B2BClasses.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using B2BClasses.Services.Enums;
namespace B2bApplication
{
    public interface ICurrentUsers
    {
        public string Name { get; }        
        public int CustomerId { get; }
        public int UserId { get; }
        public  List<int>RoleId { get; }
        public enmCustomerType CustomerType { get; }
        public bool HaveClaim(enmDocumentMaster claimId);
        public double WalletBalence { get; set; }
        public double CreditBalence { get; set; }
    }
    public class CurrentUsers : ICurrentUsers
    {

        IHttpContextAccessor _httpContext;
        private string _Name ;
        private int _UserId, _CustomerId;
        private  List<int> _RoleId;        
        private readonly DBContext _context;
        private  enmCustomerType _CustomerType;

        private double? _WalletBalence;
        private double? _CreditBalence;

        public CurrentUsers(IHttpContextAccessor httpContext, DBContext context)
        {
            _httpContext = httpContext;
            _context = context;
            string tempUserId = httpContext.HttpContext.User.FindFirst("_UserId")?.Value;
            int.TryParse(tempUserId, out _UserId);
            
            _Name= httpContext.HttpContext.User.FindFirst("_Name")?.Value;            
            string tempCustomerId = httpContext.HttpContext.User.FindFirst("_CustomerId")?.Value;
            int.TryParse(tempCustomerId, out _CustomerId);
            string tempCustomerType = httpContext.HttpContext.User.FindFirst("_CustomerType")?.Value;
            _CustomerType = enmCustomerType.B2C;
            if (!string.IsNullOrEmpty(tempCustomerType))
            {
                Enum.TryParse<enmCustomerType>(tempCustomerType,  out _CustomerType);
            }
            
        }


        private void SetWalletBalence()
        {
            var defaultBalance=_context.tblCustomerBalence.Where(p => p.CustomerId == _CustomerId).FirstOrDefault();
            if (defaultBalance == null)
            {
                _context.tblCustomerBalence.Add(new tblCustomerBalence() { CustomerId = _CustomerId, CreditBalence = 0, ModifiedDt = DateTime.Now, MPin = "0000", WalletBalence = 0 });
                _context.SaveChanges();
                this._WalletBalence = 0;
                this._CreditBalence = 0;
            }
            else
            {
                this._WalletBalence = defaultBalance.WalletBalence;
                this._CreditBalence = defaultBalance.CreditBalence;
            }

        }

        public string Name { get { return _Name; } }
        public int CustomerId { get { return _CustomerId; } }
        public int UserId { get { return _UserId; } }
        public double WalletBalence { get { if (_WalletBalence == null) { SetWalletBalence(); } return _WalletBalence.Value; } set { _WalletBalence = value; } }
        public double CreditBalence { get { if (_CreditBalence == null) { SetWalletBalence(); } return _CreditBalence.Value; } set { _CreditBalence = value; } }
        public enmCustomerType CustomerType { get { return _CustomerType; } }

        public List<int> RoleId
        {
            get
            {
                if (_RoleId == null)
                {
                    _RoleId = _context.tblUserRole.Where(p => p.UserId == _UserId ).Select(p => p.Role.Value).ToList();
                }
                return _RoleId;
            }
        }

        public bool HaveClaim(enmDocumentMaster claimId)
        {
            return _context.tblRoleClaim.Where(p => RoleId.Contains(p.Role.Value) && !p.IsDeleted && p.ClaimId == claimId).Count() > 0 ? true : false;
        }
    }
}