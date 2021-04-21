using B2BClasses.Database;
using B2BClasses.Services.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BClasses
{

    public interface ISettings
    {
        string GetErrorMessage(enmMessage message);
    }

    public class Settings : ISettings
    {
        private readonly DBContext _context;
        private IConfiguration _config;
        public Settings(DBContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }

        public string GetErrorMessage(enmMessage message)
        {
            return _config[string.Concat("ErrorMessages:", message.ToString())] ?? message.ToString();
        }

    }
}
