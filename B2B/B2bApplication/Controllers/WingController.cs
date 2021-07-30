using B2bApplication.Models;
using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Models;
using B2BClasses.Services.Enums;
using Microsoft.AspNetCore.Authorization;
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
        private async Task<mdlPackageSearch> GetPackageData(mdlPackageSearch mdl, IBooking booking)
        {
            if (mdl == null)
            {
                mdl = new mdlPackageSearch();
            }
            if (mdl.AllLocatioin == null)
            {
                mdl.AllLocatioin = new List<string>();
            }
            if (mdl.SelectedLocation == null)
            {
                mdl.SelectedLocation = new List<string>();
            }
            if (mdl.PackageData == null)
            {
                mdl.PackageData = new List<tblPackageMaster>();
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
            return mdl;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PackageSearchAsync([FromServices]IBooking booking)
        {
            ViewBag.Message = TempData["Message"];
            if (ViewBag.Message != null)
            {
                ViewBag.SaveStatus = (int)TempData["MessageType"];
            }
            mdlPackageSearch mdl=null;
            mdl = await GetPackageData(mdl, booking);
            return View(mdl);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PackageSearchAsync(mdlPackageSearch mdl, [FromServices] IBooking booking)
        {
            mdl = await GetPackageData(mdl, booking);
            return View(mdl);
        }


        [HttpGet]
        public async Task<IActionResult> BookPackage(string Id,[FromServices] IBooking booking)
        {
            int PackageId = 0;
            int.TryParse(Id, out PackageId);
            if (PackageId == 0)
            {
                TempData["Message"] = "Invalid Holiday Package";
                TempData["MessageType"] = (int)enmMessageType.Error;
                return RedirectToAction("PackageSearch");
            }
            mdlPackageBook mdl = new mdlPackageBook();
            var packagedata=(await booking.LoadPackage(PackageId, true, false, false, false)).FirstOrDefault();
            if (packagedata == null)
            {
                TempData["Message"] = "Invalid Holiday Package";
                TempData["MessageType"] = (int)enmMessageType.Error;
                return RedirectToAction("PackageSearch");
            }
            if (!(packagedata.EffectiveFromDt <= DateTime.Now && packagedata.EffectiveToDt >= DateTime.Now))
            {
                TempData["Message"] = "Holiday Package Expired";
                TempData["MessageType"] = (int)enmMessageType.Warning;
                return RedirectToAction("PackageSearch");
            }
            if (!(packagedata.IsActive ))
            {
                TempData["Message"] = "Deactive package";
                TempData["MessageType"] = (int)enmMessageType.Warning;
                return RedirectToAction("PackageSearch");
            }
            mdl.packageData = packagedata;
            mdl.AdultPrice = packagedata.AdultPrice;
            mdl.ChildPrice = packagedata.ChildPrice;
            mdl.InfantPrice = packagedata.InfantPrice;
            mdl.Discount = 0;
            mdl.NetPrice = packagedata.AdultPrice;
            mdl.PassengerDetails = new List<mdlPackageBookingPassengerDetails>();

            return View(mdl);
        }

        [HttpPost]
        public async Task<IActionResult> BookPackage(mdlPackageBook mdl, [FromServices] IBooking booking)
        {
            return RedirectToAction("BookPackage");
            
        }

     }
}
