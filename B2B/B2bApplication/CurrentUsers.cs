using B2BClasses.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace B2bApplication
{
    public interface ICurrentUsers
    {
        public string Name { get; }        
        public int CustomerId { get; }
        public int UserId { get; }
        
    }
     public class CurrentUsers : ICurrentUsers
    {

        IHttpContextAccessor _httpContext;
        private string _Name ;
        private int _UserId, _CustomerId;
        
        public CurrentUsers(IHttpContextAccessor httpContext, DBContext _context)
        {
            _httpContext = httpContext;
            
            string tempUserId = httpContext.HttpContext.User.FindFirst("_UserId")?.Value;
            int.TryParse(tempUserId, out _UserId);
            _Name= httpContext.HttpContext.User.FindFirst("_Name")?.Value;            
            string tempCustomerId = httpContext.HttpContext.User.FindFirst("_CustomerId")?.Value;
            int.TryParse(tempCustomerId, out _CustomerId);
        }
        public string Name { get { return _Name; } }
        public int CustomerId { get { return _CustomerId; } }
        public int UserId { get { return _UserId; } }
    }
}