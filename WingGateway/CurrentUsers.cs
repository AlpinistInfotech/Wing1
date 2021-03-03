using Database;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WingGateway
{
    //public class CurrentUsers : ICurrentUsers
    //{

        //private HttpContext _httpContext;
        //private string _userId, _TcId, _TcName, _TcLevelName;
        //private int _TcNid,_TcLevel;
        //public CurrentUsers(IHttpContextAccessor httpContext)
        //{
        //    _httpContext = httpContext.HttpContext;
        //    _userId = httpContext.HttpContext.User.Claims.Where(p => p.Type == "_userId_").FirstOrDefault()?.Value;            
        //    string temp_TcNid = httpContext.HttpContext.User.Claims.Where(p => p.Type == "_tcNid_").FirstOrDefault()?.Value;
        //    int.TryParse(temp_TcNid, out _TcNid);
        //    string temp_TcLevel = httpContext.HttpContext.User.Claims.Where(p => p.Type == "_tcLevel").FirstOrDefault()?.Value;
        //    int.TryParse(temp_TcLevel, out _TcLevel);

        //}

        //public string userId { get { return _userId; } }
        //public int TcNid { get { return _TcNid; } }
        //public int TcLevel { get { return _TcLevel; } }
        //public string TcId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public string TcName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public string TcLevelName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public List<string> Roles { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Document currentDocument { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public enmDocumentType? currentPermission { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
 //   }
}