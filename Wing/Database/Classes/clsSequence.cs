using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Classes
{
    public interface ISequenceMaster
    {
        string GenrateSequence(int StateId);
    }

    public class SequenceMaster : ISequenceMaster
    {
        private readonly DBContext _context;
        public SequenceMaster(DBContext context)
        {
            _context = context;
        }
        public string GenrateSequence(int StateId)
        {
            var stateMaster = _context.tblStateMaster.Where(p => p.StateId == StateId).FirstOrDefault();
            if (stateMaster == null)
            {
                throw new Exception("Invalid State Id");
            }
            string Id = "";
            int Monthyear = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
            int SeqNo = 0;
            var tblTcSequcence = _context.tblTcSequcence.Where(p => p.Monthyear == Monthyear && p.StateId == StateId).FirstOrDefault();
            if (tblTcSequcence != null)
            {
                SeqNo = tblTcSequcence.CurrentSeq+1;
                tblTcSequcence.CurrentSeq = tblTcSequcence.CurrentSeq + 1;
                _context.tblTcSequcence.Update(tblTcSequcence);
                _context.SaveChanges();
            }
            else
            {
                SeqNo = 1;
                tblTcSequcence = new tblTcSequcence()
                {
                    Monthyear = Monthyear,
                    CurrentSeq = SeqNo,
                    StateId = StateId
                };
                _context.tblTcSequcence.Add(tblTcSequcence);
                _context.SaveChanges();
            }

            Id = string.Concat(stateMaster.StateCode, DateTime.Now.Year % 100, Convert.ToString(DateTime.Now.Month, 16), SeqNo.ToString("D5"));
            return Id;
        }
    }
}
