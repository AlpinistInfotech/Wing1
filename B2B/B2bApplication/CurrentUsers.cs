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

    }
     public class CurrentUsers : ICurrentUsers
    {

        IHttpContextAccessor _httpContext;
        private string _Name ;
        private int _UserId, _CustomerId;
        private  List<int> _RoleId;        
        private readonly DBContext _context;
        private  enmCustomerType _CustomerType;

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

        public string Name { get { return _Name; } }
        public int CustomerId { get { return _CustomerId; } }
        public int UserId { get { return _UserId; } }

        
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