using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Database.Classes
{
    

    public interface IConsolidatorProfile
    {
        List<int> GetAllSpNid(int Nid);
        int GetNId(string Id,bool OnlyUnterminated=false);
        int GetNidLegCount(int Nid);
        bool ValidateSponsorId(string Id, bool OnlyUnterminated = false);
    }
    public class mdlTree
    {
        public int Nid { get; set; }
        public string TcId { get; set; }
        public string Name { get; set; }
        public enmTCRanks Rank { get; set; }
        public bool Isterminate { get; set; }
        public int SpNid { get; set; }        
        public int LegId { get; set; }
    }


    public class ConsolidatorProfile : IConsolidatorProfile
    {
        protected readonly DBContext _context;
        public ConsolidatorProfile(DBContext context)
        {
            _context = context;
        }

        public List<int> GetAllSpNid(int Nid)
        {
            return _context.tblTree.Where(p => p.TcNid == Nid).Select(p => p.TcSpNid).ToList();
        }

        public bool ValidateSponsorId(string Id, bool OnlyUnterminated = false)
        {
            if (OnlyUnterminated)
            {
                return _context.tblRegistration.Where(p => p.Id == Id && !p.IsTerminate).Any();
            }            
            return _context.tblRegistration.Where(p => p.Id == Id).Any();
        }
        public int GetNId(string Id, bool OnlyUnterminated=false)
        {

            int? Nid = OnlyUnterminated? _context.tblRegistration.Where(p => p.Id == Id && !p.IsTerminate).FirstOrDefault()?.Nid:_context.tblRegistration.Where(p => p.Id == Id ).FirstOrDefault()?.Nid;
            return Nid.HasValue ? Nid.Value : 0;
        }

        public int GetNidLegCount(int Nid)
        {
            return _context.tblRegistration.Where(p => p.SpNid == Nid).Count();
        }

        public  List<mdlTree> GetAllDownline(int spNid)
        {
            return (from t1 in _context.tblTree
                    join t2 in _context.tblRegistration on t1.TcNid equals t2.Nid
                    where t1.TcSpNid == spNid
                    select new mdlTree { TcId = t2.Id, Nid = t2.Nid, SpNid = t2.SpNid ?? 0, Name = string.Concat(t2.FirstName, " ", t2.MiddleName, " ", t2.LastName), Isterminate = t2.IsTerminate, LegId = t2.SpLegNumber, Rank = t2.TCRanks }).ToList();

        }
    }
}
