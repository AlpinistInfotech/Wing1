using B2bApplication.Models;
using B2BClasses;
using B2BClasses.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2bApplication.Controllers
{
    public class WingController : Controller
    {
        private readonly ILogger<WingController> _logger;
        private readonly DBContext _context;
        private readonly IConfiguration _config;

        public WingController(ILogger<WingController> logger, DBContext context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }

        public string GetUserDetail([FromServices] ICurrentUsers currentUsers )
        {
            return currentUsers.Name;
        }

        public IActionResult FlightSearch()
        {
            return View();
        }
        private async Task GetPackageData(mdlPackageSearch mdl, IBooking booking)
        {
            if (mdl == null)
            {
                mdl = new mdlPackageSearch();
            }
            

            var PackageData =await booking.LoadPackage(0, true, true, false, false);
            if ((mdl.SelectedLocation?.Count()??0) > 0)
            {
               // PackageData = PackageData.Where(p=> mdl.SelectedLocation.Contains( p.LocationName));
            }

        }

        [HttpGet]
        public IActionResult PackageSearch([FromServices]IBooking booking)
        {
            
            return View();
        }


    }
}
