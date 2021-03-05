﻿using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WingGateway
{
    public class CurrentUsers : ICurrentUsers
    {

        
        private enmUserType _UserType;
        private enmTCRanks _TcRanks;
        private string _userId, _TcId, _Name, _TcRankName;
        private int _TcNid, _EmpId;
        private List<string> _Roles;
        public CurrentUsers(IHttpContextAccessor httpContext, DBContext _context)
        {   
            string tempUserId = httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (tempUserId != null)
            {
                var currentUser=_context.ApplicationUser.Where(p => p.Id == tempUserId).Include(p=>p.tblRegistration).FirstOrDefault();
                if (currentUser != null)
                {
                    _UserType = currentUser.UserType;
                    if (_UserType == enmUserType.Consolidator)
                    {
                        _userId = currentUser.Id;
                        _TcId = currentUser.tblRegistration.Id;
                        _TcNid = currentUser.tblRegistration.Nid;
                        _Name = Convert.ToString(currentUser.tblRegistration.FirstName) + " " + Convert.ToString(currentUser.tblRegistration.MiddleName) + " " + Convert.ToString(currentUser.tblRegistration.LastName);
                        _TcRanks = currentUser.tblRegistration.TCRanks;
                        _TcRankName = Enum.GetName(_TcRanks);
                    }
                    else
                    {
                        _Name = currentUser.UserName;
                        _EmpId = currentUser.EmpId??0;
                    }
                }
            }
        }

        public string userId { get { return _userId; } }
        public enmUserType UserType { get { return _UserType; } }
        public int TcNid { get { return _TcNid; } }
        public int EmpId { get { return _EmpId; } }
        public enmTCRanks TcRanks { get { return _TcRanks; } }
        public string TcId { get { return _TcId; } }
        public string Name { get { return _Name; } }
        public string TcRankName { get { return _TcRankName; } }
        public List<string> Roles { get { return _Roles; } }
        public Document currentDocument { get; set; }
        public enmDocumentType? currentPermission { get ; set ; }
    }
}