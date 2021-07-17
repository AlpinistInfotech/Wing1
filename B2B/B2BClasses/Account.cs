using B2BClasses.Database;
using B2BClasses.Models;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2BClasses
{
    public interface IAccount
    {
        public enmCustomerType customerType { get; set; }
        Task<bool> IsValidIPAsync(int CustomerId, string UserIp);
        Task<tblUserMaster> LoginAsync(mdlLogin mdl, string UserIp);
        Task<IEnumerable<enmDocumentMaster>> GetEnmDocumentsAsync(int UserId);
        Task<string> GetTokken(ISettings settings, int CustomerId_, int userId_, enmCustomerType customerType_, string Name_);
    }

    public class Account : IAccount
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        private enmCustomerType? _customerType;


        public Account(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public enmCustomerType customerType { get { return _customerType.Value; } set { _customerType = value; } }

        public async Task<bool> IsValidIPAsync(int CustomerId, string UserIp)
        {
            var IPFilterMaster = await _context.tblCustomerIPFilter.Where(p => p.CustomerId == CustomerId ).FirstOrDefaultAsync();
            if (IPFilterMaster == null)
            {
                return false;
            }
            if (!IPFilterMaster.AllowedAllIp)
            {
                if ((await _context.tblCustomerIPFilterDetails.Where(p => p.CustomerId == CustomerId && p.IPAddress == UserIp).CountAsync()) == 0)
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
            _customerType = customerMaster.CustomerType;
            if (!(await IsValidIPAsync(customerMaster.Id, UserIp)))
            {
                throw new Exception("Invalid User IP");
            }
            var userMaster = _context.tblUserMaster.Where(p => p.UserName == mdl.Username && p.CustomerId== customerMaster.Id && p.IsActive).Include(p=>p.tblCustomerMaster).FirstOrDefault();
            if (userMaster == null || userMaster.Password != mdl.Password)
            {
                throw new Exception("Invalid Username/Password");
            }
            return userMaster;
        }

        public async Task<IEnumerable<enmDocumentMaster>> GetEnmDocumentsAsync(int UserId)
        {
            var AllRoles = await _context.tblUserRole.Where(p => p.UserId == UserId).Select(p => p.Role).ToListAsync();
            return _context.tblRoleClaim.Where(p => AllRoles.Contains(p.Role)).Select(p => p.ClaimId).Distinct();           
        }


        public async Task<string> GetTokken(ISettings settings,int CustomerId_, int userId_, enmCustomerType customerType_, string Name_)
        {
            string tboUrl = _config["API:GenrateToken"];
            string jsonString = JsonConvert.SerializeObject(new{ CustomerId= CustomerId_, userId = userId_ , customerType = customerType_ , Name = Name_ });
            var HaveResponse = settings.GetResponse(jsonString, tboUrl, "POST");
            if (HaveResponse.Code == 0)
            {
                return HaveResponse.Message;
            }
            else
            {
                throw new Exception(HaveResponse.Message);
            }
        }

    }
}
