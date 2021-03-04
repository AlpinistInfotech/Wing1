using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WingGateway.Controllers
{
    [Authorize]
    public class WingController : Controller
    {

        [Authorize(policy:nameof( enmDocumentMaster.Gateway_Dashboard))]
        public IActionResult Index()
        {
            return View();
        }
    }
}
