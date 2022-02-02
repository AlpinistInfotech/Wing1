using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace B2C.Classes
{

    public interface ICurrentUsers
    {
        string Name { get; }
        int CustomerId { get; }
        ulong UserId { get; }
        string Email{ get; }
        string Contact { get; }
        string Token { get; }
        enmCustomerType CustomerType { get; }
        CultureInfo cultureInfo { get; }
        

    }

    public class CurrentUser : ICurrentUsers
    {
        private readonly IHttpContextAccessor _httpContext;
        private string _Name, _Email, _Contact, _MPin,_Token;
        private int _CustomerId;
        private ulong _UserId, _DistributorId;        
        private enmCustomerType _CustomerType;
        private CultureInfo _cultureInfo;
        public CurrentUser(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            _Name = _httpContext.HttpContext.User.FindFirst("__Name")?.Value;
            _Email = _httpContext.HttpContext.User.FindFirst("__Email")?.Value;
            _Contact = _httpContext.HttpContext.User.FindFirst("__Contact")?.Value;
            _MPin = _httpContext.HttpContext.User.FindFirst("__MPin")?.Value;
            int.TryParse(_httpContext.HttpContext.User.FindFirst("__CustomerId")?.Value,out _CustomerId);
            ulong.TryParse(_httpContext.HttpContext.User.FindFirst("__UserId")?.Value, out _UserId);
            ulong.TryParse(_httpContext.HttpContext.User.FindFirst("__DistributorId")?.Value, out _DistributorId);            
            string tempCustomerType = httpContext.HttpContext.User.FindFirst("__CustomerType")?.Value;
            string _Token = httpContext.HttpContext.User.FindFirst("__Token")?.Value;
            _CustomerType = enmCustomerType.B2C;
            if (!string.IsNullOrEmpty(tempCustomerType))
            {
                Enum.TryParse<enmCustomerType>(tempCustomerType, out _CustomerType);
            }
            _cultureInfo = new CultureInfo("en-IN", false); 
        }
        public string Name { get { return _Name; } }
        public string Email { get { return _Email; } }
        public string Contact { get { return _Contact; } }
        

        public int CustomerId { get { return _CustomerId; } }
        public ulong UserId { get { return _UserId; } }
        public ulong DistributorId { get { return _DistributorId; } }
        public enmCustomerType CustomerType { get { return _CustomerType; } }
        public CultureInfo cultureInfo { get { return _cultureInfo; } }
        public string Token{ get { return _Token; } }


    }

}
