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
            var PackageData = await booking.LoadPackage(0, true, true, false, false);

            mdl.AllLocatioin = PackageData.Select(p => p.LocationName).Distinct().OrderBy(p=>p).ToList();
            if ((mdl.SelectedLocation?.Count()??0) > 0)
            {
                PackageData =  PackageData.Where(p=> mdl.SelectedLocation.Contains( p.LocationName)).ToList();
            }

            if (mdl.MaxPriceRange == 0)
            {
                mdl.MaxPriceRange = PackageData.Select(p => p.AdultPrice).Max();
            }
            if (mdl.MaxDays == 0)
            {
                mdl.MaxDays = PackageData.Select(p => p.NumberOfDay).Max();
            }

            if (mdl.MinPriceRange > 0)
            {
                PackageData = PackageData.Where(p =>p.AdultPrice>= mdl.MinPriceRange && p.AdultPrice<= mdl.MaxPriceRange).ToList();
            }
            if (mdl.MinDays > 0 )
            {
                PackageData = PackageData.Where(p => p.AdultPrice >= mdl.MinPriceRange && p.AdultPrice <= mdl.MaxPriceRange).ToList();
            }

            mdl.PackageData = PackageData;

        }

        [HttpGet]
        public async Task<IActionResult> PackageSearchAsync([FromServices]IBooking booking)
        {
            mdlPackageSearch mdl=null;
            await GetPackageData(mdl, booking);
            return View(mdl);
        }


        [HttpPost]
        public async Task<IActionResult> PackageSearchAsync(mdlPackageSearch mdl, [FromServices] IBooking booking)
        {   
            await GetPackageData(mdl, booking);
            return View(mdl);
        }

    }
}
