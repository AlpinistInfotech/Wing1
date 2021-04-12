using B2BClasses.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BClasses
{
    public class CustomerWallet
    {
        private readonly DBContext _context;
        public CustomerWallet(DBContext context)
        {
            _context = context;
        }
        
    }
}
