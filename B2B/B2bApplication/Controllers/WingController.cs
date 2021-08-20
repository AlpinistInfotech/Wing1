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

        private readonly IBooking _booking;
        private readonly ISettings _setting;
        private readonly IMarkup _markup;
        private readonly ICurrentUsers _currentUsers;
        public WingController(ILogger<WingController> logger, DBContext context, IBooking booking,
             ISettings setting, IMarkup markup, ICurrentUsers currentUsers,IConfiguration config
             )
        {
            _context = context;
            _logger = logger;
            _setting = setting;
            _config = config;
            _booking = booking;
            _markup = markup;
            _currentUsers = currentUsers;
        }

        public string GetUserDetail([FromServices] ICurrentUsers currentUsers )
        {
            return currentUsers.Name;
        }

        public IActionResult FlightSearch()
        {
            return View();
        }
        [Authorize(policy: nameof(enmDocumentMaster.Flight))]
        public async Task<IActionResult> NewFlightSearch()
        {
            mdlFlightSearch flightSearch = new mdlFlightSearch()
            {
                FlightSearchWraper = new mdlFlightSearchWraper()
                {
                    AdultCount = 1,
                    ChildCount = 0,
                    InfantCount = 0,
                    CabinClass = enmCabinClass.ECONOMY,
                    DepartureDt = null,
                    ReturnDt = null,
                    From = "DEL",
                    To = "BOM",
                    JourneyType = enmJourneyType.OneWay,

                }
            };
            await flightSearch.LoadAirportAsync(_booking);
            return View(flightSearch);
        }

        [Authorize(policy: nameof(enmDocumentMaster.Flight))]
        public async Task<IActionResult> NewFlightSearchFilter(int type, string condition, List<B2BClasses.Services.Air.mdlSearchResult> model)
        {
            mdlFlightSearch flightSearch = new mdlFlightSearch()
            {
                FlightSearchWraper = new mdlFlightSearchWraper()
                {
                    AdultCount = 1,
                    ChildCount = 0,
                    InfantCount = 0,
                    CabinClass = enmCabinClass.ECONOMY,
                    DepartureDt = null,
                    ReturnDt = null,
                    From = "DEL",
                    To = "BOM",
                    JourneyType = enmJourneyType.OneWay,

                }
            };
            await flightSearch.LoadAirportAsync(_booking);
            return View(flightSearch);
        }

        [HttpPost]
       [Authorize(policy: nameof(enmDocumentMaster.Flight))]
       [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewFlightSearch(mdlFlightSearch mdl, [FromServices] IConfiguration configuration)
        {
            //mdlFlightSearch mdl = new mdlFlightSearch();
           //mdl.FlightSearchWraper= mdlsearch;
           
            int CustomerId = 1;
            int PassengerMaxLimit = 5;
            int.TryParse(_config["PassengerMaxLimit"], out PassengerMaxLimit);
            await mdl.LoadAirportAsync(_booking);
            if (mdl.FlightSearchWraper.AdultCount + mdl.FlightSearchWraper.ChildCount + mdl.FlightSearchWraper.InfantCount > PassengerMaxLimit)
            {
                ViewBag.SaveStatus = (int)enmSaveStatus.danger;
                ViewBag.Message = _setting.GetErrorMessage(enmMessage.PassengerLimitExceed);
                ModelState.AddModelError(nameof(mdl.FlightSearchWraper.AdultCount), ViewBag.Message);
                return View(mdl);
            }
            if (ModelState.IsValid)
            {
                mdl.LoadDefaultSearchRequestAsync(_booking);
                _booking.CustomerId = CustomerId;
                mdl.searchResponse = (await _booking.SearchFlightMinPrices(mdl.searchRequest));
                if (mdl.searchResponse.Results != null)
                {
                    _markup.CustomerMarkup(mdl.searchResponse.Results, CustomerId);
                    _markup.WingMarkupAmount(mdl.searchResponse.Results, mdl.FlightSearchWraper.AdultCount, mdl.FlightSearchWraper.ChildCount, mdl.FlightSearchWraper.InfantCount);
                    _markup.WingDiscountAmount(mdl.searchResponse.Results, mdl.FlightSearchWraper.AdultCount, mdl.FlightSearchWraper.ChildCount, mdl.FlightSearchWraper.InfantCount);

                    _markup.CalculateTotalPriceAfterMarkup(mdl.searchResponse.Results, mdl.FlightSearchWraper.AdultCount, mdl.FlightSearchWraper.ChildCount, mdl.FlightSearchWraper.InfantCount, "search");
                }
                if ((mdl?.searchResponse?.Results?.Count() ?? 0) == 0)
                {
                    ViewBag.SaveStatus = (int)enmSaveStatus.danger;
                    ViewBag.Message = _setting.GetErrorMessage(enmMessage.NoFlightDataFound);
                }
            }
            if(mdl.searchResponse.Results!=null)
            {
                if (mdl.FlightSearchWraper.JourneyType == enmJourneyType.OneWay)
                {
                    var res = mdl.searchResponse?.Results.FirstOrDefault();
                    return PartialView("_NewFlightResult", res);
                }
                else
                {
                    var res = mdl.searchResponse;
                    return PartialView("_NewReturnFlightResult", res);
                }
            }
            return View(mdl);
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

            mdl.AllLocatioin = PackageData.Select(p => p.LocationName).Distinct().OrderBy(p => p).ToList();

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

            
                PackageData = PackageData.Where(p =>p.AdultPrice>= mdl.MinPriceRange && p.AdultPrice<= mdl.MaxPriceRange).ToList();
                PackageData = PackageData.Where(p => p.AdultPrice >= mdl.MinPriceRange && p.AdultPrice <= mdl.MaxPriceRange).ToList();

            if (mdl.OrderBy == 1)
            {
                mdl.PackageData = PackageData.OrderBy(p=>p.AdultPrice).ToList();
                
            }
            else if (mdl.OrderBy == 2)
            {
                mdl.PackageData = PackageData.OrderByDescending(p => p.AdultPrice).ToList();
            }
            else
            {
                mdl.PackageData = PackageData;
            }

          
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
            mdl.PackageId = packagedata.PackageId;
            mdl.packageData = packagedata;
            mdl.AdultPrice = packagedata.AdultPrice;
            mdl.ChildPrice = packagedata.ChildPrice;
            mdl.InfantPrice = packagedata.InfantPrice;
            mdl.Email = null;
            mdl.PhoneNumber = null;
            mdl.Discount = 0;
            mdl.NetPrice = packagedata.AdultPrice;
            mdl.AdultCount = 1;
            mdl.currentStep =1;
            mdl.PassengerDetails = new List<mdlPackageBookingPassengerDetails>();
            mdl.PassengerDetails.Add(new mdlPackageBookingPassengerDetails() { dob = null, FirstName = string.Empty, LastName = string.Empty, passengerType = enmPassengerType.Adult, PassportExpiryDate = null, PassportIssueDate = null, Pid = 0, pNum = null, Title = "Mr" });            
            return View(mdl);
        }

        async Task<mdlPackageBook> CalculateTotalPrice(mdlPackageBook mdl, IBooking booking)
        {
            var packagedata = (await booking.LoadPackage(mdl.PackageId, true, false, false, false)).FirstOrDefault();
            if (packagedata != null)
            {
                mdl.packageData = packagedata;
                mdl.TotalAdultPrice = mdl.AdultCount * mdl.AdultPrice;
                mdl.TotalChildPrice = mdl.ChildCount * mdl.ChildPrice;
                mdl.TotalInfantPrice = mdl.InfantCount * mdl.InfantPrice;
                mdl.TotalPrice = mdl.TotalAdultPrice + mdl.TotalChildPrice + mdl.TotalInfantPrice;
                mdl.NetPrice = mdl.TotalPrice - mdl.Discount;
            }
            return mdl;
        }


        [HttpPost]
        public async Task<IActionResult> BookPackage(mdlPackageBook mdl, [FromServices] IBooking booking, [FromServices] ICurrentUsers currentUsers, [FromServices] ICustomerWallet customerWallet)
        {
            if (mdl.PackageId == 0)
            {
                TempData["Message"] = "Invalid Package";
                TempData["MessageType"] = (int)enmMessageType.Warning;
                return RedirectToAction("PackageSearch");
            }
            if (mdl.currentStep == 2)
            {
                mdl.PassengerDetails = new List<mdlPackageBookingPassengerDetails>();
                for (int i = 0; i < mdl.AdultCount; i++)
                {
                    mdl.PassengerDetails.Add(new mdlPackageBookingPassengerDetails() { dob = null, FirstName = string.Empty, LastName = string.Empty, passengerType = enmPassengerType.Adult, PassportExpiryDate = null, PassportIssueDate = null, Pid = 0, pNum = null, Title = "Mr" });
                }
                for (int i = 0; i < mdl.ChildCount; i++)
                {
                    mdl.PassengerDetails.Add(new mdlPackageBookingPassengerDetails() { dob = null, FirstName = string.Empty, LastName = string.Empty, passengerType = enmPassengerType.Child, PassportExpiryDate = null, PassportIssueDate = null, Pid = 0, pNum = null, Title = "Master" });
                }
                for (int i = 0; i < mdl.InfantCount; i++)
                {
                    mdl.PassengerDetails.Add(new mdlPackageBookingPassengerDetails() { dob = null, FirstName = string.Empty, LastName = string.Empty, passengerType = enmPassengerType.Infant, PassportExpiryDate = null, PassportIssueDate = null, Pid = 0, pNum = null, Title = "Master" });
                }
            }
            mdl=await CalculateTotalPrice(mdl,booking);
            if (mdl.currentStep == 4 && mdl.stepDirection == "next")
            {
                if (currentUsers.MPin == mdl.Mpin)
                {
                    
                    await booking.BookPackage(mdl, currentUsers.CustomerId, customerWallet);
                }
                else
                {
                    ViewBag.Message = "Invalid MPin";
                    ViewBag.SaveStatus = (int)enmSaveStatus.danger;
                }
                
            }

            if (mdl.stepDirection == "next")
            {
                mdl.currentStep = mdl.currentStep + 1;
            }
            else
            {
                mdl.currentStep = mdl.currentStep - 1;
            }
            
            return View(mdl);
        }

     }
}
