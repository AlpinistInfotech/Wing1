using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public interface IModified
    {
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
    }

    public interface ICreated
    {
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
    }

    

}
