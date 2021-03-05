using Database;
using Database.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WingGateway.Models;

namespace WingGateway.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;        
        private readonly DBContext _context;        
        
        
        public HomeController(ILogger<HomeController> logger,DBContext context)
        {
            _logger = logger;            
            _context = context;
        }



        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Dashboard))]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public dynamic GetMenu([FromServices] ICurrentUsers currentUsers)
        {
            var roles = _context.UserRoles.Where(p => p.UserId == currentUsers.userId).Select(p => p.RoleId).ToList();
            var AllClaims = _context.RoleClaims.Where(p => roles.Contains(p.RoleId)).Select(p => p.ClaimType).ToList();
            List<Document> alldocument = new List<Document>();
            List<Module> allModule = new List<Module>();
            List<SubModule> allSubModule = new List<SubModule>();
            var eDms = Enum.GetValues(typeof(enmDocumentMaster)).Cast<enmDocumentMaster>().ToList();
            eDms.RemoveAll(p => !AllClaims.Contains(p.ToString()));
            foreach (var doc in eDms)
            {
                alldocument.Add(doc.GetDocumentDetails());
            }
            var eMods = Enum.GetValues(typeof(enmModule)).Cast<enmModule>().ToList();
            foreach (var doc in eMods)
            {
                allModule.Add(doc.GetModuleDetails());
            }
            var eModSubs = Enum.GetValues(typeof(enmSubModule)).Cast<enmSubModule>().ToList();
            foreach (var doc in eModSubs)
            {
                allSubModule.Add(doc.GetSubModuleDetails());
            }


            var _document = alldocument.Select(p => new { id = p.Id, name = p.Name, subModuleId = p.EnmSubModule.HasValue ? p.EnmSubModule.Value : 0,
                moduleId = p.EnmModule.HasValue ? p.EnmModule.Value : 0,
                displayOrder = p.DisplayOrder,
                actionName = p.ActionName,
                icon = p.Icon
            }).OrderBy(p => p.displayOrder);
            var _subModule =allSubModule.Select(p => new { id = p.Id, name = p.Name,
                moduleId = p.EnmModule,
                displayOrder = p.DisplayOrder,
                cntrlName = p.CntrlName,
                icon = p.Icon
            });
            var _module = allModule.Select(p => new {
                id = p.Id,
                name = p.Name,                
                displayOrder = p.DisplayOrder,
                cntrlName = p.CntrlName,
                icon = p.Icon
            });

            var returndata = new{_document, _subModule , _module };
            return returndata;

        }


        [Authorize(policy: nameof(enmDocumentMaster.Emp_Dashboard))]
        public IActionResult Privacy()
        {
            return View();
        }

        
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [Authorize(policy: nameof(enmDocumentMaster.Gateway_UploadKyc))]
        public IActionResult UploadKyc()
        {
            return View(new mdlKyc());
        }

    }
}
