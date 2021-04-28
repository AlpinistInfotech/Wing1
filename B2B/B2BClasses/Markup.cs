using B2BClasses.Database;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BClasses
{
    public class Markup
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public int CustomerId { get { return _CustomerId; } set { _CustomerId = value; } }
        private int _CustomerId;
        public Markup(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        
        

        

    }
}
