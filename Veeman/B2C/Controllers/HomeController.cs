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
            await LoadAirport("","");
            return View();
        }

        void ValidateFlightSearchRequest(mdlFlightSearchRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("", "Invalid data");
            }
            else
            {
                if (request.Orign.Equals(request.Destination, StringComparison.OrdinalIgnoreCase)) ;
            }
        }


        [Route("FlightSearch/{Orign}/{Destination}/{TravelDt}/{ReturnDt}/{CabinClass}/{JourneyType}/{AdultCount}/{ChildCount}/{InfantCount}")]
        [Route("FlightSearch/{Orign}/{Destination}/{TravelDt}/{CabinClass}/{AdultCount}/{ChildCount}/{InfantCount}")]
        public IActionResult FlightSearch(string Orign,string Destination,DateTime TravelDt, DateTime? ReturnDt=null ,
            enmCabinClass CabinClass=enmCabinClass.ECONOMY, int AdultCount=1, int ChildCount=0,int InfantCount=0,
            enmJourneyType JourneyType=enmJourneyType.OneWay)
        {
            mdlFlightSearchRequest request = new mdlFlightSearchRequest() {
                Orign = Orign,
                Destination = Destination,
                TravelDt = TravelDt,
                ReturnDt = ReturnDt.HasValue ? ReturnDt.Value : TravelDt,
                CabinClass= CabinClass,
                JourneyType = JourneyType,
                AdultCount= AdultCount,
                ChildCount= ChildCount,
                InfantCount= InfantCount
               
            };
            
            return View();
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
