using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public interface ICurrentUsers
    {
        public string userId { get;  }
        public int TcNid { get; }                
        public int TcLevel { get; }
        public string TcId { get; }
        public string TcName { get; }
        public string TcLevelName { get; }        
        public List<string> Roles { get;  } 
        public Document currentDocument { get; set; }        
        public enmDocumentType? currentPermission { get; set; }

    }
}
