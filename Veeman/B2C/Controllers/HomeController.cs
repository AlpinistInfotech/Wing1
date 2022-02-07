using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using B2C.Classes;
using B2C.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace B2C.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICurrentUsers _currentUsers;
        private readonly IFlightSearch _flightSearch;


        public HomeController(ILogger<HomeController> logger, ICurrentUsers currentUsers, IFlightSearch flightSearch)
        {
            _logger = logger;
            _currentUsers = currentUsers;
            _flightSearch = flightSearch;
        }

        Task LoadAirport(string FromAirport, string ToAirport)
        {
            var data = _flightSearch.GetAirport(false, true, _currentUsers.Token);
            List<SelectListItem> FromAirports = new List<SelectListItem>();
            List<SelectListItem> ToAirports = new List<SelectListItem>();
            if (data.messageType == enmMessageType.Success)
            {
                FromAirports.AddRange(data.returnId.Select(q => new SelectListItem { Value = q.AirportCode, Text = q.AirportCode + ", " + q.CityName+", " +q.CountryCode, Selected=q.AirportCode==FromAirport}));
                ToAirports.AddRange(data.returnId.Select(q => new SelectListItem { Value = q.AirportCode, Text = q.AirportCode + ", " + q.CityName +", " + q.CountryCode,Selected= q.AirportCode==ToAirport }));
            }
            ViewBag.FromAirports = FromAirports;
            ViewBag.ToAirports = ToAirports;
            return Task.FromResult(0);
        }

        public async Task<IActionResult> IndexAsync()
        {
            await LoadAirport("DEL","BOM");
            return View();
        }

        void ValidateFlightSearchRequest(mdlFlightSearchRequest request)
        {
            DateTime CurrentDate = Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));

            if (request == null)
            {
                ModelState.AddModelError("", "Invalid data");
            }
            else
            {
                if (string.IsNullOrWhiteSpace( request.From))
                {
                    ModelState.AddModelError("", "Required Origin airport");
                }
                if (string.IsNullOrWhiteSpace(request.To))
                {
                    ModelState.AddModelError("", "Required Destination airport");
                }
                if (request.From.Equals(request.To, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", "Origin and destination should be different");
                }
                if(request.DepartureDt< CurrentDate)
                {
                    ModelState.AddModelError("",string.Format("Departure Date should be greater than {0}", CurrentDate.ToString("dd-MMM-yyyy")));
                }
                if (request.JourneyType == enmJourneyType.Return)
                {
                    if (request.DepartureDt > request.ReturnDt)
                    {
                        ModelState.AddModelError("", string.Format("Departure Date should be less than Return Date"));
                    }
                }
                if (request.AdultCount <= 0)
                {
                    ModelState.AddModelError("", string.Format("Adult Count should be greater then 0"));
                }
                if (request.ChildCount < 0)
                {
                    ModelState.AddModelError("", string.Format("Child Count should be greater then or equal to 0"));
                }
                if (request.InfantCount< 0)
                {
                    ModelState.AddModelError("", string.Format("Infant Count should be greater then or equal to 0"));
                }
                if (request.AdultCount+ request.ChildCount+ request.InfantCount>9)
                {
                    ModelState.AddModelError("", string.Format("upto 9 passenger can travel in single ticket"));
                }
            }
        }
        [Route("/Home/FlightSearch/{Orign}/{Destination}/{TravelDt}/{ReturnDt}/{CabinClass}/{JourneyType}/{AdultCount}/{ChildCount}/{InfantCount}")]
        [Route("/Home/FlightSearch/{Orign}/{Destination}/{TravelDt}/{CabinClass}/{AdultCount}/{ChildCount}/{InfantCount}")]
        public async Task<IActionResult> FlightSearchAsync(string Orign,string Destination,DateTime TravelDt, DateTime? ReturnDt=null ,
            enmCabinClass CabinClass=enmCabinClass.ECONOMY, int AdultCount=1, int ChildCount=0,int InfantCount=0,
            enmJourneyType JourneyType=enmJourneyType.OneWay)
        {
            mdlSearchResponse mdl = new mdlSearchResponse();
            mdlFlightSearchRequest request = new mdlFlightSearchRequest() {
                From = Orign,
                To = Destination,
                DepartureDt= TravelDt,
                ReturnDt = ReturnDt.HasValue ? ReturnDt.Value : TravelDt,
                CabinClass= CabinClass,
                JourneyType = JourneyType,
                AdultCount= AdultCount,
                ChildCount= ChildCount,
                InfantCount= InfantCount
               
            };

            await LoadAirport(Orign, Destination);
            ValidateFlightSearchRequest(request);
            if (ModelState.IsValid)
            {
                
                mdl = _flightSearch.Search(request, _currentUsers.Token);
                if (mdl.ResponseStatus == enmMessageType.Error)
                {
                    ModelState.AddModelError("", mdl.Error.Message);
                }
                
            }
            //Default data
            {
                mdl.From = request.From;
                mdl.To = request.To;
                mdl.DepartureDt = request.DepartureDt;                
                mdl.ReturnDt = request.ReturnDt;
                mdl.CabinClass = request.CabinClass;
                mdl.JourneyType = request.JourneyType;
                mdl.AdultCount = request.AdultCount;
                mdl.ChildCount = request.ChildCount;
                mdl.InfantCount = request.InfantCount;
            }

            return View(mdl);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
