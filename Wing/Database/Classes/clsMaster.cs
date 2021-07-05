using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Classes
{
    public interface ITcMaster
    {
        List<tblBankMaster> GetBanks(DBContext context, bool OnlyActive);
    }

     public class TcMaster : ITcMaster
    {
        private readonly DBContext _context;
        public TcMaster(DBContext context)
        {
            _context = context;
        }

        public List<tblBankMaster> GetBanks(DBContext context, bool OnlyActive)
        {
            if (OnlyActive)
            {
                return context.tblBankMaster.Where(p => p.IsActive).ToList();
            }
            else
            {
                return context.tblBankMaster.ToList();
            }
        }

        


    }
}
