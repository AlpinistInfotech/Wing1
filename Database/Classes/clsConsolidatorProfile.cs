using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Classes
{
    

    public interface IConsolidatorProfile
    {
        List<int> GetAllSpNid(int Nid);
        int GetNId(string Id);
        int GetNidLegCount(int Nid);
        bool ValidateSponsorId(string Id);
    }

    public class ConsolidatorProfile : IConsolidatorProfile
    {
        private readonly DBContext _context;
        public ConsolidatorProfile(DBContext context)
        {
            _context = context;
        }

        public List<int> GetAllSpNid(int Nid)
        {
            return _context.tblTree.Where(p => p.TcNid == Nid).Select(p => p.TcSpNid).ToList();
        }

        public bool ValidateSponsorId(string Id)
        {
            return _context.tblRegistration.Where(p => p.Id == Id).Any();
        }
        public int GetNId(string Id)
        {

            int? Nid = _context.tblRegistration.Where(p => p.Id == Id).FirstOrDefault()?.Nid;
            return Nid.HasValue ? Nid.Value : 0;
        }

        public int GetNidLegCount(int Nid)
        {
            return _context.tblRegistration.Where(p => p.SpNid == Nid).Count();
        }

    }
}
