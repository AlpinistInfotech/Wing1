using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Database
{
    public static class ClaimStore
    {
        public static List<Claim> GetClaims()
        {
            List<Claim> AllClaim = new List<Claim>();
            foreach (var value in Enum.GetNames(typeof( enmDocumentMaster)))
            {
                AllClaim.Add(new Claim(value,value));
            }
            return AllClaim;
        }
        
    }

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
