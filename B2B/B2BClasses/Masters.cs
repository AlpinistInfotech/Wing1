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
        string FetchCountryCode(int CountryId);
        string FetchCountryName(int CountryId);
        string FetchStateName(int StateId);
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


    }
}
