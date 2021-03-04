using Database;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WingGateway
{
    public class CurrentUsers : ICurrentUsers
    {

        private HttpContext _httpContext;
        private string _userId, _TcId, _TcName, _TcLevelName;
        private int _TcNid, _TcLevel;
        private List<string> _Roles;
        public CurrentUsers(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext.HttpContext;
            _userId = httpContext.HttpContext.User.Claims.Where(p => p.Type == "_userId_").FirstOrDefault()?.Value;
            string temp_TcNid = httpContext.HttpContext.User.Claims.Where(p => p.Type == "_tcNid_").FirstOrDefault()?.Value;
            int.TryParse(temp_TcNid, out _TcNid);
            string temp_TcLevel = httpContext.HttpContext.User.Claims.Where(p => p.Type == "_tcLevel").FirstOrDefault()?.Value;
            int.TryParse(temp_TcLevel, out _TcLevel);
            _TcName = httpContext.HttpContext.User.Claims.Where(p => p.Type == "_tcName").FirstOrDefault()?.Value;
            _TcId = httpContext.HttpContext.User.Claims.Where(p => p.Type == "_tcId").FirstOrDefault()?.Value;
            _TcLevelName = httpContext.HttpContext.User.Claims.Where(p => p.Type == "_tcLevelName").FirstOrDefault()?.Value;

        }

        public string userId { get { return _userId; } }
        public int TcNid { get { return _TcNid; } }
        public int TcLevel { get { return _TcLevel; } }
        public string TcId { get { return _TcId; } }
        public string TcName { get { return _TcName; } }
        public string TcLevelName { get { return _TcLevelName; } }
        public List<string> Roles { get { return _Roles; } }
        public Document currentDocument { get; set; }
        public enmDocumentType? currentPermission { get ; set ; }
    }
}