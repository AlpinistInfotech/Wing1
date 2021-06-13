using B2BClasses.Database;
using B2BClasses.Services.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace B2BClasses
{
   

    public interface IMasters
    {
        string FetchBankName(int BankId);
        string FetchCountryCode(int CountryId);
        string FetchCountryName(int CountryId);
        string FetchStateName(int StateId);
        Dictionary<int, string> FetchCountryNames();
        Dictionary<int, string> FetchStateNames(int CountryId);
    }

    public class Masters : IMasters
    {

        private readonly DBContext _context;
        private IConfiguration _config;
        public Masters(DBContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }

        public String FetchCountryName(int CountryId)
        {
            return _context.tblCountryMaster.FirstOrDefault(p => p.CountryId == CountryId)?.CountryName ?? "";
        }
        public String FetchCountryCode(int CountryId)
        {
            return _context.tblCountryMaster.FirstOrDefault(p => p.CountryId == CountryId)?.CountryCode ?? "";
        }
        public String FetchStateName(int StateId)
        {
            return _context.tblStateMaster.FirstOrDefault(p => p.StateId == StateId)?.StateName ?? "";
        }
        public String FetchBankName(int BankId)
        {
            return _context.tblBankMaster.FirstOrDefault(p => p.BankId == BankId)?.BankName ?? "";
        }

        public Dictionary<int, string> FetchCountryNames()
        {
            return _context.tblCountryMaster.Where(p => p.IsActive).OrderBy(p=>p.CountryName).ThenBy(p=>p.CountryCode).ToDictionary(p => p.CountryId, p => string.Concat(p.CountryName, " - ", p.CountryCode));
        }
        public Dictionary<int, string> FetchStateNames(int CountryId)
        {
            if (CountryId > 0)
            {
                return _context.tblStateMaster.Where(p => p.IsActive && p.CountryId == CountryId).OrderBy(p => p.StateName).ToDictionary(p => p.StateId, p => p.StateName);
            }
            else
            {
                return new Dictionary<int, string>();
            }
            
        }

    }
}
