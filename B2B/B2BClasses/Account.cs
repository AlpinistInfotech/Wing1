using B2BClasses.Database;
using B2BClasses.Models;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2BClasses
{
    public interface IAccount
    {
        Task<bool> IsValidIPAsync(int CustomerId, string UserIp);
        Task<tblUserMaster> LoginAsync(mdlLogin mdl, string UserIp);
        Task<IEnumerable<enmDocumentMaster>> GetEnmDocumentsAsync(int UserId);
    }

    public class Account : IAccount
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public Account(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<bool> IsValidIPAsync(int CustomerId, string UserIp)
        {
            var IPFilterMaster = await _context.tblCustomerIPFilter.Where(p => p.CustomerId == CustomerId && !p.IsDeleted).FirstOrDefaultAsync();
            if (IPFilterMaster == null)
            {
                return false;
            }
            if (!IPFilterMaster.AllowedAllIp)
            {
                if ((await _context.tblCustomerIPFilterDetails.Where(p => p.FilterId == IPFilterMaster.Id && p.IPAddress == UserIp).CountAsync()) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<tblUserMaster> LoginAsync(mdlLogin mdl, string UserIp)
        {

            var customerMaster = await _context.tblCustomerMaster.Where(p => p.Code == mdl.Code && p.IsActive).FirstOrDefaultAsync();
            if (customerMaster == null)
            {
                throw new Exception("Invalid Customer ID");
            }

            if (await IsValidIPAsync(customerMaster.Id, UserIp))
            {
                throw new Exception("Invalid User IP");
            }
            var userMaster = _context.tblUserMaster.Where(p => p.UserName == mdl.Username && p.IsActive).FirstOrDefault();
            if (userMaster == null || userMaster.Password != mdl.Password)
            {
                throw new Exception("Invalid Username/Password");
            }
            return userMaster;
        }

        public async Task<IEnumerable<enmDocumentMaster>> GetEnmDocumentsAsync(int UserId)
        {
            return await _context.tblUserRole.Where(p => p.UserId == UserId).Select(p=>p.Role).ToListAsync();
        }
    }
}
