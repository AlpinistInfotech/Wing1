using B2bApplication.Models;
using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Models;
using B2BClasses.Services.Air;
using B2BClasses.Services.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Session;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
        private ICompositeViewEngine _viewEngine;

        public WingController(ILogger<WingController> logger, DBContext context, IBooking booking,
             ISettings setting, IMarkup markup, ICurrentUsers currentUsers, IConfiguration config, ICompositeViewEngine viewEngine
             )
        {
            _context = context;
            _logger = logger;
            _setting = setting;
            _config = config;
            _booking = booking;
            _markup = markup;
            _currentUsers = currentUsers;
            _viewEngine = viewEngine;
        }
        public string GetUserDetail([FromServices] ICurrentUsers currentUsers)
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
                    DepartureDt = DateTime.Now,
                    ReturnDt = DateTime.Now.AddDays(1),
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
            if (mdl.searchResponse.Results != null)
            {
                if (mdl.FlightSearchWraper.JourneyType == enmJourneyType.OneWay)
                {
                    //foreach (var some in mdl.searchResponse.Results.FirstOrDefault().ToList())
                    //{
                    //    some.traceid = mdl.searchResponse.TraceId;
                    //}

                    //mdl.searchResponse.Results.FirstOrDefault().FirstOrDefault().traceid = mdl.searchResponse?.TraceId;
                    var res = mdl.searchResponse?.Results.FirstOrDefault().OrderBy(x => x.TotalPriceList?.FirstOrDefault().TotalPrice).ToList();
                    
                    int nonstopcnt = res.Where(x => x.Segment.Count() == 1).Count();
                    int onestopcnt = res.Where(x => x.Segment.Count() == 2).Count();
                    int multistopcnt = res.Where(x => x.Segment.Count() > 2).Count();
                    var maxamt = res.OrderByDescending(x => x.TotalPriceList?.FirstOrDefault().TotalPrice).Select(x => x.TotalPriceList?.FirstOrDefault().TotalPrice).FirstOrDefault();
                    var minamt = res.OrderBy(x => x.TotalPriceList?.FirstOrDefault().TotalPrice).Select(x => x.TotalPriceList?.FirstOrDefault().TotalPrice).FirstOrDefault();
                    string currencysym = _currentUsers.cultureInfo.NumberFormat.CurrencySymbol;
                    //double maxamt = res.Select(x => x.TotalPriceList.Max(TotalPrice)).FirstOrDefault();
                    List<string> airlines = res.Select(x => x.Segment.FirstOrDefault()?.Airline?.Name).Distinct().ToList();
                    HttpContext.Session.SetObjectAsJson("flight", res);

                    PartialViewResult partialViewResult = PartialView("_NewFlightResult", res);
                    string viewContent = ConvertViewToString(this.ControllerContext, partialViewResult, _viewEngine);
                    return Json(new { searchtype = 1, error = 0, PartialView = viewContent, nonstopcnt = nonstopcnt, onestopcnt = onestopcnt, multistopcnt = multistopcnt, airlines = airlines, maxamt = maxamt, minamt = minamt, currency = currencysym,traceid= mdl.searchResponse.TraceId});
                    // return PartialView("_NewFlightResult", res);
                }
                else
                {
                    var res = mdl.searchResponse;
                    var res1 = mdl.searchResponse?.Results[0].OrderBy(x => x.TotalPriceList?.FirstOrDefault().TotalPrice).ToList();
                    var res2 = mdl.searchResponse?.Results[1].OrderBy(x => x.TotalPriceList?.FirstOrDefault().TotalPrice).ToList();

                    int nonstopcnt1 = res1.Where(x => x.Segment.Count() == 1).Count();
                    int nonstopcnt2 = res2.Where(x => x.Segment.Count() == 1).Count();
                    int onestopcnt1 = res1.Where(x => x.Segment.Count() == 2).Count();
                    int onestopcnt2 = res2.Where(x => x.Segment.Count() == 2).Count();
                    int multistopcnt1 = res1.Where(x => x.Segment.Count() > 2).Count();
                    int multistopcnt2 = res2.Where(x => x.Segment.Count() > 2).Count();
                    var maxamt1 = res1.OrderByDescending(x => x.TotalPriceList?.FirstOrDefault().TotalPrice).Select(x => x.TotalPriceList?.FirstOrDefault()?.TotalPrice ?? 0).FirstOrDefault();
                    var maxamt2 = res2.OrderByDescending(x => x.TotalPriceList?.FirstOrDefault().TotalPrice).Select(x => x.TotalPriceList?.FirstOrDefault()?.TotalPrice ?? 0).FirstOrDefault();
                    var minamt1 = res1.OrderBy(x => x.TotalPriceList?.FirstOrDefault().TotalPrice).Select(x => x.TotalPriceList?.FirstOrDefault()?.TotalPrice ?? 0).FirstOrDefault();
                    var minamt2 = res2.OrderBy(x => x.TotalPriceList?.FirstOrDefault().TotalPrice).Select(x => x.TotalPriceList?.FirstOrDefault()?.TotalPrice ?? 0).FirstOrDefault();

                    int nonstopcnt = nonstopcnt1 + nonstopcnt2;
                    int onestopcnt = onestopcnt1 + onestopcnt2;
                    int multistopcnt = multistopcnt1 + multistopcnt2;
                    var maxamt = Math.Max(maxamt1, maxamt2);
                    var minamt = Math.Min(minamt1, minamt2);
                    string currencysym = _currentUsers.cultureInfo.NumberFormat.CurrencySymbol;
                    //double maxamt = res.Select(x => x.TotalPriceList.Max(TotalPrice)).FirstOrDefault();
                    List<string> airlines1 = res1.Select(x => x.Segment.FirstOrDefault()?.Airline?.Name).Distinct().ToList();
                    List<string> airlines2 = res2.Select(x => x.Segment.FirstOrDefault()?.Airline?.Name).Distinct().ToList();
                    List<string> airlines = new List<string>();
                    airlines.AddRange(airlines1);
                    airlines.AddRange(airlines2);

                    HttpContext.Session.SetObjectAsJson("flight", res);

                    PartialViewResult partialViewResult = PartialView("_NewReturnFlightResult", res);
                    string viewContent = ConvertViewToString(this.ControllerContext, partialViewResult, _viewEngine);
                    return Json(new { searchtype = 2, error = 0, PartialView = viewContent, nonstopcnt = nonstopcnt, onestopcnt = onestopcnt, multistopcnt = multistopcnt, airlines = airlines.Distinct(), maxamt = maxamt, minamt = minamt, currency = currencysym, traceid = mdl.searchResponse.TraceId });
                    //return PartialView("_NewReturnFlightResult", res);
                }
            }
            else
            {
                return Json("no record");
            }
        }
        public string ConvertViewToString(ControllerContext controllerContext, PartialViewResult pvr, ICompositeViewEngine _viewEngine)
        {
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = _viewEngine.FindView(controllerContext, pvr.ViewName, false);
                ViewContext viewContext = new ViewContext(controllerContext, vResult.View, pvr.ViewData, pvr.TempData, writer, new HtmlHelperOptions());

                vResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }
        [AcceptVerbs("Get", "Post")]

        public async Task<IActionResult> OnewayFlightSearchfilter(int[] stop, string[] airline, string[] deptimearr, string sortname, string sortvalue)
        {
            List<mdlSearchResult> serch = new List<mdlSearchResult>();

            List<mdlSearchResult> serchstop = new List<mdlSearchResult>();
            if (HttpContext.Session.GetObjectFromJson<List<mdlSearchResult>>("flight") != null)
            {
                serch = HttpContext.Session.GetObjectFromJson<List<mdlSearchResult>>("flight");
            }
            if (string.IsNullOrEmpty(sortvalue))
            {
                sortvalue = "asc";
            }
            if (string.IsNullOrEmpty(sortname))
            {
                sortname = "price";
            }

            if (stop.Length > 0)
            {
                foreach (var stops in stop)
                {
                    if (stops == 1)
                        serchstop.AddRange(serch.Where(x => x.Segment.Count() == 1).ToList());
                    if (stops == 2)
                        serchstop.AddRange(serch.Where(x => x.Segment.Count() == 2).ToList());
                    if (stops == 3)
                        serchstop.AddRange(serch.Where(x => x.Segment.Count() > 2).ToList());
                }
                serch = serchstop;
            }
            if (airline.Length > 0)
            {
                serch = serch.Where(x => airline.Contains(x.Segment.FirstOrDefault()?.Airline.Name)).ToList();
            }
            if (deptimearr.Length > 0)
            {
                TimeSpan emornstart = TimeSpan.Parse("00:00"); // 12 AM
                TimeSpan emornend = TimeSpan.Parse("08:00");   // 8 AM
                TimeSpan mornstart = TimeSpan.Parse("08:01"); // 12 AM
                TimeSpan mornend = TimeSpan.Parse("12:00");   // 8 AM
                TimeSpan midstart = TimeSpan.Parse("12:01"); // 12 AM
                TimeSpan midend = TimeSpan.Parse("16:00");   // 8 AM
                TimeSpan evenstart = TimeSpan.Parse("16:01"); // 12 AM
                TimeSpan evenend = TimeSpan.Parse("20:00");   // 8 AM
                TimeSpan nigtstart = TimeSpan.Parse("20:01"); // 12 AM
                TimeSpan nigtend = TimeSpan.Parse("23:59");   // 8 AM
                int isfirst = 0;
                List<mdlSearchResult> serch2 = new List<mdlSearchResult>();
                foreach (var dept in deptimearr)
                {
                    isfirst++;
                    if (dept == "earlymorning")
                    {
                        serch2.AddRange(serch.Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= emornstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= emornend).ToList());

                    }
                    if (dept == "morning")
                    {
                        serch2.AddRange(serch.Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= mornstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= mornend).ToList());

                    }
                    if (dept == "midday")
                    {
                        serch2.AddRange(serch.Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= midstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= midend).ToList());

                    }
                    if (dept == "evening")
                    {
                        serch2.AddRange(serch.Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= evenstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= evenend).ToList());

                    }
                    if (dept == "night")
                    {
                        serch2.AddRange(serch.Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= nigtstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= nigtend).ToList());

                    }
                }
                serch = serch2;
            }
            if (sortname == "airline")
            {
                if (sortvalue == "desc")
                {
                    serch = serch.OrderByDescending(x => x.Segment.FirstOrDefault()?.Airline.Name).ToList();

                }
                else
                {
                    serch = serch.OrderBy(x => x.Segment.FirstOrDefault()?.Airline.Name).ToList();

                }
            }
            if (sortname == "departure")
            {
                if (sortvalue == "desc")
                {
                    serch = serch.OrderByDescending(x => x.Segment.FirstOrDefault()?.DepartureTime).ToList();

                }
                else
                {
                    serch = serch.OrderBy(x => x.Segment.FirstOrDefault()?.DepartureTime).ToList();

                }
            }
            if (sortname == "duration")
            {
                if (sortvalue == "desc")
                {
                    serch = serch.OrderByDescending(x => x.Segment.FirstOrDefault()?.Duration).ToList();

                }
                else
                {
                    serch = serch.OrderBy(x => x.Segment.FirstOrDefault()?.Duration).ToList();

                }
            }
            if (sortname == "arrival")
            {
                if (sortvalue == "desc")
                {
                    serch = serch.OrderByDescending(x => x.Segment.FirstOrDefault()?.ArrivalTime).ToList();

                }
                else
                {
                    serch = serch.OrderBy(x => x.Segment.FirstOrDefault()?.ArrivalTime).ToList();

                }
            }
            if (sortname == "price")
            {
                if (sortvalue == "desc")
                {
                    serch = serch.OrderByDescending(x => x.TotalPriceList.FirstOrDefault()?.TotalPrice).ToList();

                }
                else
                {
                    serch = serch.OrderBy(x => x.TotalPriceList.FirstOrDefault()?.TotalPrice).ToList();

                }
            }


            return PartialView("_NewFlightResult", serch);

        }

        [AcceptVerbs("Get", "Post")]

        public async Task<IActionResult> RoundwayFlightSearchfilter(int[] stop, string[] airline, string[] deptimearr, string sortname, string sortvalue)
        {
            mdlSearchResponse sres = new mdlSearchResponse();

            List<mdlSearchResult> serchstop = new List<mdlSearchResult>();
            List<mdlSearchResult> serchstop2 = new List<mdlSearchResult>();
            if (HttpContext.Session.GetObjectFromJson<mdlSearchResponse>("flight") != null)
            {
                sres = HttpContext.Session.GetObjectFromJson<mdlSearchResponse>("flight");

            }
            if (string.IsNullOrEmpty(sortvalue))
            {
                sortvalue = "asc";
               
            }
            if (string.IsNullOrEmpty(sortname))
            {
                sortname = "price";               
            }

            if (stop.Length > 0)
            {
                foreach (var stops in stop)
                {
                    if (stops == 1)
                    {
                        serchstop.AddRange(sres.Results[0].Where(x => x.Segment.Count() == 1).ToList());
                        serchstop2.AddRange(sres.Results[1].Where(x => x.Segment.Count() == 1).ToList());
                    }
                    if (stops == 2)
                    {
                        serchstop.AddRange(sres.Results[0].Where(x => x.Segment.Count() == 2).ToList());
                        serchstop2.AddRange(sres.Results[1].Where(x => x.Segment.Count() == 2).ToList());
                    }
                    if (stops == 3)
                    {
                        serchstop.AddRange(sres.Results[0].Where(x => x.Segment.Count() > 2).ToList());
                        serchstop2.AddRange(sres.Results[1].Where(x => x.Segment.Count() > 2).ToList());
                    }
                }
                sres.Results[0] = serchstop;
                sres.Results[1] = serchstop2;
            }
            if (airline.Length > 0)
            {
                sres.Results[0] = sres.Results[0].Where(x => airline.Contains(x.Segment.FirstOrDefault()?.Airline.Name)).ToList();
                sres.Results[1] = sres.Results[1].Where(x => airline.Contains(x.Segment.FirstOrDefault()?.Airline.Name)).ToList();
            }
            if (deptimearr.Length > 0)
            {
                TimeSpan emornstart = TimeSpan.Parse("00:00"); // 12 AM
                TimeSpan emornend = TimeSpan.Parse("08:00");   // 8 AM
                TimeSpan mornstart = TimeSpan.Parse("08:01"); // 12 AM
                TimeSpan mornend = TimeSpan.Parse("12:00");   // 8 AM
                TimeSpan midstart = TimeSpan.Parse("12:01"); // 12 AM
                TimeSpan midend = TimeSpan.Parse("16:00");   // 8 AM
                TimeSpan evenstart = TimeSpan.Parse("16:01"); // 12 AM
                TimeSpan evenend = TimeSpan.Parse("20:00");   // 8 AM
                TimeSpan nigtstart = TimeSpan.Parse("20:01"); // 12 AM
                TimeSpan nigtend = TimeSpan.Parse("23:59");   // 8 AM
                int isfirst = 0;
                List<mdlSearchResult> serchdept = new List<mdlSearchResult>();
                List<mdlSearchResult> serchdept2 = new List<mdlSearchResult>();
                foreach (var dept in deptimearr)
                {
                    isfirst++;
                    if (dept == "earlymorning")
                    {
                        serchdept.AddRange(sres.Results[0].Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= emornstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= emornend).ToList());
                        serchdept2.AddRange(sres.Results[1].Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= emornstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= emornend).ToList());

                    }
                    if (dept == "morning")
                    {
                        serchdept.AddRange(sres.Results[0].Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= mornstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= mornend).ToList());
                        serchdept2.AddRange(sres.Results[1].Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= mornstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= mornend).ToList());

                    }
                    if (dept == "midday")
                    {
                        serchdept.AddRange(sres.Results[0].Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= midstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= midend).ToList());
                        serchdept2.AddRange(sres.Results[1].Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= midstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= midend).ToList());
                    }
                    if (dept == "evening")
                    {
                        serchdept.AddRange(sres.Results[0].Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= evenstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= evenend).ToList());
                        serchdept2.AddRange(sres.Results[1].Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= evenstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= evenend).ToList());

                    }
                    if (dept == "night")
                    {
                        serchdept.AddRange(sres.Results[0].Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= nigtstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= nigtend).ToList());
                        serchdept2.AddRange(sres.Results[1].Where(x => x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay >= nigtstart && x.Segment.FirstOrDefault()?.DepartureTime.TimeOfDay <= nigtend).ToList());
                    }
                }
                sres.Results[0] = serchdept;
                sres.Results[1] = serchdept2;
            }
            if (sortname == "airline")
            {
                if (sortvalue == "desc")
                {
                    sres.Results[0] = sres.Results[0].OrderByDescending(x => x.Segment.FirstOrDefault()?.Airline.Name).ToList();
                }
                else
                {
                    sres.Results[0] = sres.Results[0].OrderBy(x => x.Segment.FirstOrDefault()?.Airline.Name).ToList();

                }
            }
            if (sortname == "departure")
            {
                if (sortvalue == "desc")
                {
                    sres.Results[0] = sres.Results[0].OrderByDescending(x => x.Segment.FirstOrDefault()?.DepartureTime).ToList();

                }
                else
                {
                    sres.Results[0] = sres.Results[0].OrderBy(x => x.Segment.FirstOrDefault()?.DepartureTime).ToList();

                }
            }
            if (sortname == "duration")
            {
                if (sortvalue == "desc")
                {
                    sres.Results[0] = sres.Results[0].OrderByDescending(x => x.Segment.FirstOrDefault()?.Duration).ToList();

                }
                else
                {
                    sres.Results[0] = sres.Results[0].OrderBy(x => x.Segment.FirstOrDefault()?.Duration).ToList();

                }
            }
            if (sortname == "arrival")
            {
                if (sortvalue == "desc")
                {
                    sres.Results[0] = sres.Results[0].OrderByDescending(x => x.Segment.FirstOrDefault()?.ArrivalTime).ToList();

                }
                else
                {
                    sres.Results[0] = sres.Results[0].OrderBy(x => x.Segment.FirstOrDefault()?.ArrivalTime).ToList();

                }
            }
            if (sortname == "price")
            {
                if (sortvalue == "desc")
                {
                    sres.Results[0] = sres.Results[0].OrderByDescending(x => x.TotalPriceList.FirstOrDefault()?.TotalPrice).ToList();

                }
                else
                {
                    sres.Results[0] = sres.Results[0].OrderBy(x => x.TotalPriceList.FirstOrDefault()?.TotalPrice).ToList();

                }
            }
            //return flight
            if (sortname == "airline2")
            {
                if (sortvalue == "desc")
                {
                    sres.Results[1] = sres.Results[1].OrderByDescending(x => x.Segment.FirstOrDefault()?.Airline.Name).ToList();
                }
                else
                {
                    sres.Results[1] = sres.Results[1].OrderBy(x => x.Segment.FirstOrDefault()?.Airline.Name).ToList();

                }
            }
            if (sortname == "departure2")
            {
                if (sortvalue == "desc")
                {
                    sres.Results[1] = sres.Results[1].OrderByDescending(x => x.Segment.FirstOrDefault()?.DepartureTime).ToList();

                }
                else
                {
                    sres.Results[1] = sres.Results[1].OrderBy(x => x.Segment.FirstOrDefault()?.DepartureTime).ToList();

                }
            }
            if (sortname == "duration2")
            {
                if (sortvalue == "desc2")
                {
                    sres.Results[1] = sres.Results[1].OrderByDescending(x => x.Segment.FirstOrDefault()?.Duration).ToList();

                }
                else
                {
                    sres.Results[1] = sres.Results[1].OrderBy(x => x.Segment.FirstOrDefault()?.Duration).ToList();

                }
            }
            if (sortname == "arrival2")
            {
                if (sortvalue == "desc")
                {
                    sres.Results[1] = sres.Results[1].OrderByDescending(x => x.Segment.FirstOrDefault()?.ArrivalTime).ToList();

                }
                else
                {
                    sres.Results[1] = sres.Results[1].OrderBy(x => x.Segment.FirstOrDefault()?.ArrivalTime).ToList();

                }
            }
            if (sortname == "price2")
            {
                if (sortvalue == "desc")
                {
                    sres.Results[1] = sres.Results[1].OrderByDescending(x => x.TotalPriceList.FirstOrDefault()?.TotalPrice).ToList();

                }
                else
                {
                    sres.Results[1] = sres.Results[1].OrderBy(x => x.TotalPriceList.FirstOrDefault()?.TotalPrice).ToList();

                }
            }

            return PartialView("_NewReturnFlightResult", sres);

        }
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> ClearOnewayFlightSearchfilter()
        {
            List<mdlSearchResult> serch = new List<mdlSearchResult>();

            if (HttpContext.Session.GetObjectFromJson<List<mdlSearchResult>>("flight") != null)
            {
                serch = HttpContext.Session.GetObjectFromJson<List<mdlSearchResult>>("flight");
            }
            return PartialView("_NewFlightResult", serch);
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> ClearRoundwayFlightSearchfilter()
        {
            mdlSearchResponse sres = new mdlSearchResponse();

            if (HttpContext.Session.GetObjectFromJson<mdlSearchResponse>("flight") != null)
            {
                sres = HttpContext.Session.GetObjectFromJson<mdlSearchResponse>("flight");

            }
            return PartialView("_NewFlightResult", sres);
        }
        [AcceptVerbs("Get")]
        [Authorize(policy: nameof(enmDocumentMaster.Flight))]
        public async Task<IActionResult> NewFlightReviewAsync()
        {
            var mdls = TempData["mdl_"] as string;
            mdlFlightReview mdl = JsonConvert.DeserializeObject<mdlFlightReview>(mdls);
            ViewBag.SaveStatus = (int)TempData["MessageType"];
            ViewBag.Message = TempData["Message"];
            if (mdl == null)
            {
                mdl = new mdlFlightReview();
            }
            await mdl.LoadFareQuotationAsync(_currentUsers.CustomerId, _booking, _markup, _context);

            //save Data
            await _booking.CustomerFlightDetailSave(mdl.FareQuoteRequest.TraceId, mdl.FareQuotResponse);
            mdl.BookingRequestDefaultData();
            return View(mdl);
        }

        [AcceptVerbs("Post")]
        [ValidateAntiForgeryToken]

        [Authorize(policy: nameof(enmDocumentMaster.Flight))]
        public async Task<IActionResult> NewFlightReview(mdlFlightReview mdl)
        {
            if (mdl == null)
            {
                mdl = new mdlFlightReview();
            }
            await mdl.LoadFareQuotationAsync(_currentUsers.CustomerId, _booking, _markup, _context);
            mdl.BookingRequestDefaultData();
            
            HttpContext.Session.SetObjectAsJson("flightreview", mdl);
            
            return View(mdl);
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> checkMpin(string mpin)
        {
            string CustomerMPin = _currentUsers.MPin ?? "0000";
            if (CustomerMPin != mpin)
            {
                return Json("invalid");
            }
            else
            {
                return Json("success");
            }
        }
        [AcceptVerbs("Get", "Post")]
        [ValidateAntiForgeryToken]
        [Authorize(policy: nameof(enmDocumentMaster.Flight))]
        public async Task<IActionResult> NewFlightBook(mdlFlightReview mdl, [FromServices] ICustomerWallet customerWallet)
        {
            mdlFlighBook mdlres = new mdlFlighBook() { FareQuotResponse = new List<mdlFareQuotResponse>(), IsSucess = new List<bool>(), BookingId = new List<string>(), PNR = new List<string>() };
            bool IsPriceChanged = false;
            if (HttpContext.Session.GetObjectFromJson<mdlFlightReview>("flightreview") != null)
            {
               var sres = HttpContext.Session.GetObjectFromJson<mdlFlightReview>("flightreview");
                for(int i=0;i<sres.FareQuotResponse.Count();i++)
                {
                   mdl.FareQuotResponse[i].Results = sres.FareQuotResponse[i].Results;
                }
            }

            for (int ti = 0; ti < mdl.travellerInfo.Count(); ti++)
            {
                if(mdl.travellerInfo[ti].ssrBaggageInfoslist!=null)
                {
                    foreach (var item in mdl.travellerInfo[ti].ssrBaggageInfoslist)
                    {
                        if(string.IsNullOrEmpty(item.key) && string.IsNullOrEmpty(item.code))
                        {
                            mdl.travellerInfo[ti].ssrBaggageInfoslist.Remove(item);                            
                        }
                        break;
                    }
                }
                if (mdl.travellerInfo[ti].ssrMealInfoslist != null)
                {
                    foreach (var item in mdl.travellerInfo[ti].ssrMealInfoslist)
                    {
                        if (string.IsNullOrEmpty(item.key) && string.IsNullOrEmpty(item.code))
                        {
                            mdl.travellerInfo[ti].ssrMealInfoslist.Remove(item);
                        }
                        break;
                    }
                }
                if (mdl.travellerInfo[ti].ssrSeatInfoslist != null)
                {
                    foreach (var item in mdl.travellerInfo[ti].ssrSeatInfoslist)
                    {
                        if (string.IsNullOrEmpty(item.key) && string.IsNullOrEmpty(item.code))
                        {
                            mdl.travellerInfo[ti].ssrSeatInfoslist.Remove(item);
                        }
                        break;
                    }
                }
                if (mdl.travellerInfo[ti].ssrExtraServiceInfoslist != null)
                {
                    foreach (var item in mdl.travellerInfo[ti].ssrExtraServiceInfoslist)
                    {
                        if (string.IsNullOrEmpty(item.key) && string.IsNullOrEmpty(item.code))
                        {
                            mdl.travellerInfo[ti].ssrExtraServiceInfoslist.Remove(item);
                        }
                        break;
                    }
                }
                //mdl.travellerInfo[ti].ssrBaggageInfoslist.Add(new mdlSSRS() { code = mdl.travellerInfo[ti].ssrBaggageInfos.code, key = mdl.travellerInfo[ti].ssrBaggageInfos.key });
            }

            if (!(mdl == null || mdl.FareQuoteRequest == null))
            {
                var s = JsonConvert.SerializeObject(mdl);
                if (mdl.contacts == null)
                {
                    TempData["mdl_"] = s;
                    TempData["MessageType"] = (int)enmMessageType.Warning;
                    TempData["Message"] = "Required Contact no";
                    return RedirectToAction("NewFlightReview");
                }
                else if (mdl.emails == null)
                {
                    TempData["mdl_"] = s;
                    TempData["MessageType"] = (int)enmMessageType.Warning;
                    TempData["Message"] = "Required Email Address";
                    return RedirectToAction("NewFlightReview");
                }

                //await mdl.LoadFareQuotationAsync(_currentUsers.CustomerId, _booking, _markup, _context);
                IsPriceChanged = mdl.FareQuotResponse.Any(p => p.IsPriceChanged);
                if (IsPriceChanged)
                {
                    TempData["mdl_"] = s;
                    TempData["MessageType"] = (int)enmMessageType.Warning;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.FlightPriceChanged);
                    return RedirectToAction("NewFlightReview");
                }

                customerWallet.CustomerId = _currentUsers.CustomerId;
                double WalletBalance = await customerWallet.GetBalanceAsync();
                if (WalletBalance < mdl.NetFare)
                {
                    TempData["mdl_"] = s;
                    TempData["MessageType"] = (int)enmMessageType.Warning;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.InsufficientWalletBalance);
                    return RedirectToAction("NewFlightReview");
                }
                else
                {
                    await customerWallet.DeductBalanceAsync(DateTime.Now, mdl.NetFare, enmTransactionType.FlightTicketBook, mdl.FareQuoteRequest.TraceId);
                }
                mdlBookingRequest mdlReq = new mdlBookingRequest();
                //if Price not chnage then Book the Flight
                for (int i = 0; i < mdl.FareQuotResponse.Count(); i++)
                {
                    
                    if (mdl.FareQuotResponse[i].ServiceProvider == enmServiceProvider.TBO)
                    {
                        
                        mdlReq.BookingId = mdl.FareQuotResponse[i].BookingId;
                        mdlReq.TraceId = mdl.FareQuotResponse[i].TraceId;
                        mdlReq.TokenId = mdl.FareQuotResponse[i].TokenId;
                        mdlReq.userip = mdl.FareQuotResponse[i].userip;
                        mdlReq.IsLCC = mdl.FareQuoteCondition.IsLCC;
                       if(mdl.FareQuoteRequest.ResultIndex != null)
                        {
                            string[] reindx = mdl.FareQuoteRequest.ResultIndex[0].Split('_');
                            if(reindx.Length>0)
                            mdlReq.resultindex = reindx[1];
                        }
                        //  mdlReq.travellerInfo = mdl.travellerInfo;
                        List<mdlTravellerinfo> pi = new List<mdlTravellerinfo>();
                        foreach (var item in mdl.travellerInfo)
                        {
                            var pxtype = 1;
                            if (item.passengerType == enmPassengerType.Adult)
                                pxtype = 1;
                            else if (item.passengerType == enmPassengerType.Child)
                                pxtype = 2;
                            else
                                pxtype = 0;
                            pi.Add(new mdlTravellerinfo
                            {
                                address1 = item.address1,
                                address2 = item.address2,
                                cellcountrycode = "+91",
                                countrycode = "IN",
                                countryname="INDIA",
                                city = "Delhi",                                
                                dob = item.dob,                                
                                FFAirlineCode = item.FFAirlineCode,
                                FFNumber = item.FFNumber,
                                FirstName = item.FirstName,
                                Gender = item.Gender,                                
                                IsLeadPax = item.IsLeadPax,
                                LastName = item.LastName,
                                nationality = "IN",
                                passengerType = item.passengerType,
                               PaxType= pxtype,
                                Title = item.Title,
                                ssrBaggageInfoslist = item.ssrBaggageInfoslist,
                                ssrExtraServiceInfoslist = item.ssrExtraServiceInfoslist,
                                ssrMealInfoslist = item.ssrMealInfoslist,
                                ssrSeatInfoslist = item.ssrSeatInfoslist,
                                Fare = (new mdlfare {
                                AdditionalTxnFeeOfrd= mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.AdditionalTxnFeeOfrd,
                                    AdditionalTxnFeePub = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.AdditionalTxnFeePub,
                                    AirTransFee = 0,
                                    BaseFare = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.BaseFare,
                                    Discount = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.Discount,
                                    OfferedFare = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.OfferedFare,
                                    OtherCharges = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.OtherCharges,
                                    PublishedFare = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.PublishedFare,
                                    ServiceFee = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.ServiceFee,
                                    Tax = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.Tax,
                                    TdsOnCommission = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.TdsOnCommission,
                                    TdsOnIncentive = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.TdsOnIncentive,
                                    TdsOnPLB = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.TdsOnPLB,
                                    TransactionFee =0,
                                    YQTax = mdl.FareQuotResponse[i].Results[0][0].TotalPriceList[0].ADULT.FareComponent.YQTax,
                                })
                            }) ;
                        }
                        mdlReq.travellerInfo = pi;
                        
                        List<string> cont = new List<string>();
                        cont.Add(mdl.contacts);
                        List<string> eml = new List<string>();
                        eml.Add(mdl.emails);
                        List<mdlPaymentInfos> pinfo = new List<mdlPaymentInfos>();
                        pinfo.Add(new mdlPaymentInfos() { amount = mdl.FareQuotResponse[i].TotalPriceInfo.TotalFare + mdl.OtherCharge });
                        
                        mdlReq.deliveryInfo = new mdlDeliveryinfo() { contacts = cont, emails = eml };
                        mdlReq.gstInfo = mdl.gstInfo;
                        mdlReq.paymentInfos = pinfo;
                        var Result = await _booking.BookingAsync(mdlReq);
                       
                        mdlres.FareQuotResponse.Add(mdl.FareQuotResponse[i]);
                        mdlres.IsSucess.Add(Result.ResponseStatus == 1 ? true : false);
                        mdlres.BookingId.Add(Result.ResponseStatus == 1 ? Result.bookingId : Result.Error.Message);
                        mdlres.PNR.Add(Result.ResponseStatus == 1 ? Result.PNR : Result.Error.Message);
                        mdlres.travellerInfo = mdl.travellerInfo;
                        mdlres.deliveryInfo = mdlReq.deliveryInfo;
                        if (!(Result.ResponseStatus == 1))
                        {
                            ViewBag.SaveStatus = (int)enmMessageType.Warning;
                            ViewBag.Message = Result.Error.Message;
                        }

                    }
                    else
                    {
                       
                        List<string> cont = new List<string>();
                        cont.Add(mdl.contacts);
                        List<string> eml = new List<string>();
                        eml.Add(mdl.emails);
                        List<mdlPaymentInfos> pi = new List<mdlPaymentInfos>();
                        pi.Add(new mdlPaymentInfos() { amount = mdl.FareQuotResponse[i].TotalPriceInfo.TotalFare + mdl.OtherCharge });
                        string bookid = mdl.FareQuotResponse[i].BookingId;
                        
                            mdlReq.TraceId = mdl.FareQuoteRequest.TraceId;
                            mdlReq.BookingId = bookid;
                            mdlReq.travellerInfo = mdl.travellerInfo;
                        mdlReq.deliveryInfo = new mdlDeliveryinfo() { contacts = cont, emails = eml };
                            mdlReq.gstInfo = mdl.gstInfo;
                            mdlReq.paymentInfos = pi;
                      
                   
                    var Result = await _booking.BookingAsync(mdlReq);
                  
                    mdlres.FareQuotResponse.Add(mdl.FareQuotResponse[i]);
                    mdlres.IsSucess.Add(Result.ResponseStatus == 1 ? true : false);
                    mdlres.BookingId.Add(Result.ResponseStatus == 1 ? Result.bookingId : Result.Error.Message);
                    mdlres.travellerInfo = mdl.travellerInfo;
                    mdlres.deliveryInfo = mdlReq.deliveryInfo;
                    if (!(Result.ResponseStatus == 1))
                    {
                        ViewBag.SaveStatus = (int)enmMessageType.Warning;
                        ViewBag.Message = Result.Error.Message;
                    }
                    }

                }
            }
            else
            {
                return RedirectToAction("NewFlightSearch", "Wing");
            }

            enmBookingStatus bookingStatus = enmBookingStatus.Pending;
            if (mdlres.IsSucess.Count > 0)
            {
                if (mdlres.IsSucess.Any(p => !p))
                {
                    if (mdlres.IsSucess.Any(p => p))
                    {
                        bookingStatus = enmBookingStatus.PartialBooked;
                    }
                    else
                    {
                        bookingStatus = enmBookingStatus.Failed;
                        //return the all Amount
                        await customerWallet.AddBalanceAsync(DateTime.Now, mdl.NetFare, enmTransactionType.FlightTicketBook, string.Concat("Booking Ids", string.Join(',', mdl.FareQuotResponse.Select(p => p.BookingId))));
                    }
                }
                else
                {
                    bookingStatus = enmBookingStatus.Booked;
                }
            }
            _booking.CompleteBooking(mdl.FareQuoteRequest.TraceId ?? "", bookingStatus);

            return View(mdlres);
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

            if ((mdl.SelectedLocation?.Count() ?? 0) > 0)
            {
                PackageData = PackageData.Where(p => mdl.SelectedLocation.Contains(p.LocationName)).ToList();
            }

            if (mdl.MaxPriceRange == 0)
            {
                mdl.MaxPriceRange = PackageData.Select(p => p.AdultPrice).Max();
            }
            if (mdl.MaxDays == 0)
            {
                mdl.MaxDays = PackageData.Select(p => p.NumberOfDay).Max();
            }


            PackageData = PackageData.Where(p => p.AdultPrice >= mdl.MinPriceRange && p.AdultPrice <= mdl.MaxPriceRange).ToList();
            PackageData = PackageData.Where(p => p.AdultPrice >= mdl.MinPriceRange && p.AdultPrice <= mdl.MaxPriceRange).ToList();

            if (mdl.OrderBy == 1)
            {
                mdl.PackageData = PackageData.OrderBy(p => p.AdultPrice).ToList();

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
        public async Task<IActionResult> PackageSearchAsync([FromServices] IBooking booking)
        {
            ViewBag.Message = TempData["Message"];
            if (ViewBag.Message != null)
            {
                ViewBag.SaveStatus = (int)TempData["MessageType"];
            }
            mdlPackageSearch mdl = null;
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
        public async Task<IActionResult> BookPackage(string Id, [FromServices] IBooking booking)
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
            var packagedata = (await booking.LoadPackage(PackageId, true, false, false, false)).FirstOrDefault();
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
            if (!(packagedata.IsActive))
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
            mdl.currentStep = 1;
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
            mdl = await CalculateTotalPrice(mdl, booking);
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
