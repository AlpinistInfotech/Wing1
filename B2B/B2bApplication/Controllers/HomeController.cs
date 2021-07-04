using B2bApplication.Models;
using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Services.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using B2BClasses.Services.Air;
using B2BClasses.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace B2bApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _context;
        private readonly IBooking _booking;
        private readonly ISettings _setting;
        private readonly IMarkup _markup;
        int _customerId,_userId;
        public HomeController(ILogger<HomeController> logger, DBContext context, IBooking booking,
            ISettings setting, IMarkup markup
            )
        {
            _context = context;
            _logger = logger;
            _setting = setting;
            _booking = booking;
            _markup = markup;
            _customerId = 1;
            _userId = 1;
        }

        [Authorize]
        public IActionResult Index()
        {
            DateTime TodayFromDt = Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));
            DateTime TodayToDt = TodayFromDt.AddDays(1);
            ViewBag.FlightSearch = _context.tblFlightBookingMaster.Where(p => p.CustomerId == _customerId && p.CreatedDt >= TodayFromDt && p.CreatedDt < TodayToDt).Count();
            ViewBag.FlightBook = _context.tblFlightBookingMaster.Where(p => p.CustomerId == _customerId && p.BookingStatus== enmBookingStatus.Booked && p.CreatedDt >= TodayFromDt && p.CreatedDt < TodayToDt).Count();

            DateTime WeekFromDt = TodayFromDt.AddDays(0- TodayFromDt.DayOfWeek);
            DateTime WeekToDt = TodayToDt;
            ViewBag.FlightSearchWeek = _context.tblFlightBookingMaster.Where(p => p.CustomerId == _customerId && p.CreatedDt >= WeekFromDt && p.CreatedDt < WeekToDt).Count();
            ViewBag.FlightBookWeek = _context.tblFlightBookingMaster.Where(p => p.CustomerId == _customerId && p.BookingStatus == enmBookingStatus.Booked && p.CreatedDt >= WeekFromDt && p.CreatedDt < WeekToDt).Count();

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

        public IActionResult Error404()
        {
            return View();
        }


        [Authorize]
        public async Task<dynamic> GetMenuAsync([FromServices]ICurrentUsers currentUsers,[FromServices] IAccount account)
        {   
            List<Document> alldocument = new List<Document>();
            List<Module> allModule = new List<Module>();
            List<SubModule> allSubModule = new List<SubModule>();
            var eDms = (await account.GetEnmDocumentsAsync(currentUsers.UserId));            
            foreach (var doc in eDms)
            {
                var DD=doc.GetDocumentDetails();
                if (DD.DocumentType.HasFlag(enmDocumentType.DisplayMenu))
                {
                    alldocument.Add(DD);
                }                
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


            var _document = alldocument.Select(p => new {
                id = p.Id,
                name = p.Name,
                subModuleId = p.EnmSubModule.HasValue ? p.EnmSubModule.Value : 0,
                moduleId = p.EnmModule.HasValue ? p.EnmModule.Value : 0,
                displayOrder = p.DisplayOrder,
                actionName = p.ActionName,
                icon = p.Icon
            }).OrderBy(p => p.displayOrder);
            var _subModule = allSubModule.Select(p => new {
                id = p.Id,
                name = p.Name,
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

            var returndata = new { _document, _subModule, _module };
            return returndata;

        }


        //[Authorize(policy: nameof(enmDocumentMaster.Flight))]
        [Authorize]
        public async Task<IActionResult> FlightSearch()
        {
            mdlFlightSearch flightSearch = new mdlFlightSearch() { 
                FlightSearchWraper= new mdlFlightSearchWraper() { 
                    AdultCount=1,
                    ChildCount=0,
                    InfantCount=0,
                    CabinClass= enmCabinClass.ECONOMY,
                    DepartureDt= DateTime.Now,
                    ReturnDt=null,
                    From ="DEL",
                    To="BOM",
                    JourneyType=enmJourneyType.OneWay,
                    
                }
            };
            await flightSearch.LoadAirportAsync(_booking);            
            return View(flightSearch);
        }

        [HttpPost]
        //[Authorize(policy: nameof(enmDocumentMaster.Flight))]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> FlightSearch(mdlFlightSearch mdl, [FromServices]IConfiguration configuration)
        {
            int CustomerId = 1;
            int PassengerMaxLimit = 5;
            int.TryParse(configuration["PassengerMaxLimit"], out PassengerMaxLimit);
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
                _markup.CustomerMarkup(mdl.searchResponse.Results);
                _markup.WingMarkupAmount(mdl.searchResponse.Results, mdl.FlightSearchWraper.AdultCount, mdl.FlightSearchWraper.ChildCount, mdl.FlightSearchWraper.InfantCount);
                _markup.CalculateTotalPriceAfterMarkup(mdl.searchResponse.Results, mdl.FlightSearchWraper.AdultCount, mdl.FlightSearchWraper.ChildCount, mdl.FlightSearchWraper.InfantCount);
                if ((mdl?.searchResponse?.Results?.Count() ?? 0) == 0)
                {
                    ViewBag.SaveStatus = (int)enmSaveStatus.danger;
                    ViewBag.Message = _setting.GetErrorMessage(enmMessage.NoFlightDataFound);
                }
            }
           
            return View(mdl);
        }



        [AcceptVerbs("Get")]
        public async Task<IActionResult> FlightReviewAsync()
        {
            var mdls = TempData["mdl_"] as string;
            mdlFlightReview mdl = JsonConvert.DeserializeObject<mdlFlightReview>(mdls);
            ViewBag.SaveStatus = (int)TempData["MessageType"];
            ViewBag.Message = TempData["Message"];
            if (mdl == null)
            {
                mdl = new mdlFlightReview();
            }
            await mdl.LoadFareQuotationAsync(_customerId, _booking, _markup);

            //save Data
            await _booking.CustomerFlightDetailSave(mdl.FareQuoteRequest.TraceId, mdl.FareQuotResponse);
            mdl.BookingRequestDefaultData();
            return View(mdl);
        }



        [AcceptVerbs("Post")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> FlightReview(mdlFlightReview mdl)
        {
            
            if (mdl == null)
            {
                mdl = new mdlFlightReview();
            }
            await mdl.LoadFareQuotationAsync(_customerId, _booking, _markup);
            //save Data
            await _booking.CustomerFlightDetailSave(mdl.FareQuoteRequest.TraceId,mdl.FareQuotResponse);
            mdl.BookingRequestDefaultData();
            return View(mdl);
        }



        [AcceptVerbs("Get", "Post")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> FlightBook(mdlFlightReview mdl, [FromServices]ICustomerWallet customerWallet )
        {
            mdlFlighBook mdlres = new mdlFlighBook() {FareQuotResponse= new List<mdlFareQuotResponse>(), IsSucess=new List<bool>() , BookingId= new List<string>()};
            
            

            bool IsPriceChanged = false;
            if (!(mdl == null || mdl.FareQuoteRequest == null))
            {
                var s = JsonConvert.SerializeObject(mdl);
                if (mdl.contacts == null)
                {
                    TempData["mdl_"] = s;
                    TempData["MessageType"] = (int)enmMessageType.Warning;
                    TempData["Message"] = "Required Contact no";
                    return RedirectToAction("FlightReview");
                }
                else if (mdl.emails == null)
                {
                    TempData["mdl_"] = s;
                    TempData["MessageType"] = (int)enmMessageType.Warning;
                    TempData["Message"] = "Required Email Address";
                    return RedirectToAction("FlightReview");
                }



                await mdl.LoadFareQuotationAsync(_customerId, _booking, _markup);
                IsPriceChanged = mdl.FareQuotResponse.Any(p => p.IsPriceChanged);
                if (IsPriceChanged)
                {
                    
                    TempData["mdl_"] = s;
                    TempData["MessageType"] = (int)enmMessageType.Warning;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.FlightPriceChanged);
                    return RedirectToAction("FlightReview");                    
                }
                
                customerWallet.CustomerId = _customerId;
                double WalletBalance=await customerWallet.GetBalanceAsync();
                if (WalletBalance < mdl.NetFare)
                {                    
                    TempData["mdl_"] = s;
                    TempData["MessageType"] = (int)enmMessageType.Warning;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.InsufficientWalletBalance);
                    return RedirectToAction("FlightReview");                    
                }
                else
                {   
                    await customerWallet.DeductBalanceAsync(DateTime.Now ,mdl.TotalFare,enmTransactionType.TicketBook, string.Concat("Booking Ids", string.Join( ',',mdl.FareQuotResponse.Select(p=>p.BookingId))) );
                }

                //if Price not chnage then Book the Flight
                for (int i = 0; i < mdl.FareQuotResponse.Count(); i++)
                {
                    List<string> cont = new List<string>();
                    cont.Add(mdl.contacts);
                    List<string> eml = new List<string>();
                    eml.Add(mdl.emails);
                    List<mdlPaymentInfos> pi = new List<mdlPaymentInfos>();
                    pi.Add(new mdlPaymentInfos() { amount = mdl.FareQuotResponse[i].TotalPriceInfo.TotalFare });

                    mdlBookingRequest mdlReq = new mdlBookingRequest()
                    {
                        TraceId = mdl.FareQuoteRequest.TraceId,
                        BookingId = mdl.FareQuotResponse[i].BookingId,
                        travellerInfo = mdl.travellerInfo,
                        deliveryInfo = new mdlDeliveryinfo() { contacts = cont, emails = eml },
                        gstInfo = mdl.gstInfo,
                        paymentInfos = pi
                    };
                    var Result=  await _booking.BookingAsync(mdlReq);
                    mdlres.FareQuotResponse.Add(mdl.FareQuotResponse[i]);
                    mdlres.IsSucess.Add(Result.ResponseStatus == 1 ? true : false);
                    mdlres.BookingId.Add(Result.ResponseStatus == 1 ?Result.bookingId: Result.Error.Message);
                    if (!(Result.ResponseStatus == 1))
                    {


                        ViewBag.SaveStatus = (int)enmMessageType.Warning;
                        ViewBag.Message = Result.Error.Message;
                    }
                    
                }
            }
            else
            {
                return RedirectToAction("FlightSearch", "Home");
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
                        await customerWallet.AddBalanceAsync(DateTime.Now, mdl.TotalFare, enmTransactionType.TicketBook, string.Concat("Booking Ids", string.Join(',', mdl.FareQuotResponse.Select(p => p.BookingId))));
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

        
        
        [Authorize]
        public async Task<IActionResult> WingMarkup(string Id,[FromServices] IMarkup markup)
        {   
            ViewBag.Message = TempData["Message"];
            if (ViewBag.Message != null)
            {
                ViewBag.SaveStatus = (int)TempData["MessageType"];
            }
            mdlWingMarkupWraper mdl = new mdlWingMarkupWraper();
            if (Id != null)
            {
                int ID = 0;
                int.TryParse(Id, out ID);
                if (ID > 0)
                {
                    mdl.WingMarkup = markup.LoadMarkup(0).FirstOrDefault();
                }
                
            }
            if (mdl.WingMarkup == null)
            {
                mdl.WingMarkup = new mdlWingMarkup();
            }
            
            mdl.SetDefaultDropDown(_context);
            return View(mdl);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> WingMarkup(mdlWingMarkupWraper mdl,string submitData, [FromServices]IMarkup markup)
        {
            bool IsUpdated = false;
            if (mdl.WingMarkup.Id>0)
            {
                IsUpdated = true;
            }
            if (mdl.WingMarkup == null)
            {
                ModelState.AddModelError("", "Invalid Data");
            }
            else
            {
                if (!mdl.WingMarkup.IsAllCustomerType)
                {
                    if (mdl.WingMarkup.MarkupCustomerType == null || mdl.WingMarkup.MarkupCustomerType.Count == 0)
                    {
                        ModelState.AddModelError("WingMarkup.IsAllCustomerType", "Select Customer Type");
                    }
                }
                if (!mdl.WingMarkup.IsAllCustomer)
                {
                    if (mdl.WingMarkup.MarkupCustomerDetail == null || mdl.WingMarkup.MarkupCustomerDetail.Count == 0)
                    {
                        ModelState.AddModelError("WingMarkup.IsAllCustomer", "Select Customer");
                    }
                }
                if (!mdl.WingMarkup.IsAllPessengerType)
                {
                    if (mdl.WingMarkup.MarkupPassengerType == null || mdl.WingMarkup.MarkupPassengerType.Count == 0)
                    {
                        ModelState.AddModelError("WingMarkup.IsAllPessengerType", "Select Passenger Type");
                    }
                }
                if (!mdl.WingMarkup.IsAllProvider)
                {
                    if (mdl.WingMarkup.MarkupServiceProvider == null || mdl.WingMarkup.MarkupServiceProvider.Count == 0)
                    {
                        ModelState.AddModelError("WingMarkup.IsAllProvider", "Select Service Provider");
                    }
                }
                if (!mdl.WingMarkup.IsAllFlightClass)
                {
                    if (mdl.WingMarkup.MarkupCabinClass == null || mdl.WingMarkup.MarkupCabinClass.Count == 0)
                    {
                        ModelState.AddModelError("WingMarkup.IsAllFlightClass", "Select Flight Class");
                    }
                }
                if (!mdl.WingMarkup.IsAllAirline)
                {
                    if (mdl.WingMarkup.MarkupAirline == null || mdl.WingMarkup.MarkupAirline.Count == 0)
                    {
                        ModelState.AddModelError("WingMarkup.IsAllAirline", "Select Airlines");
                    }
                }
                if (mdl.WingMarkup.Id == 0)
                {
                    if (mdl.WingMarkup.EffectiveFromDt < Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy")))
                    {
                        ModelState.AddModelError("WingMarkup.EffectiveFromDt", "Effective FromDate should be greater then Today");
                    }
                }
                
                if (mdl.WingMarkup.Amount <= 0)
                {
                    ModelState.AddModelError("WingMarkup.Amount", "Amount Should be Greater then 0");
                }
            }
            if (ModelState.IsValid)
            {
                if (submitData == "deleteData")
                {
                    if (markup.RemoveMarkup(mdl.WingMarkup.Id, _userId))
                    {
                        TempData["MessageType"] = (int)enmMessageType.Success;
                        TempData["Message"] = _setting.GetErrorMessage(enmMessage.DeleteSuccessfully);
                        return RedirectToAction("WingMarkup");
                    }
                }
                else
                {
                    if (IsUpdated)
                    {
                        if (markup.RemoveMarkup(mdl.WingMarkup.Id, _userId) && markup.AddMarkup(mdl.WingMarkup, _userId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);
                            return RedirectToAction("WingMarkup");
                        }
                    }
                    else
                    {
                        if (markup.AddMarkup(mdl.WingMarkup, _userId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);
                            return RedirectToAction("WingMarkup");
                        }
                    }
                }

                
            }
            else 
            {
                ViewBag.MessageType =(int) enmMessageType.Warning;
                ViewBag.Message = "Data not valid";
            }
            mdl.SetDefaultDropDown(_context);
            return View(mdl);
        }


        [Authorize]
        public async Task<IActionResult> Convenience(string Id, [FromServices] IMarkup markup)
        {
            ViewBag.Message = TempData["Message"];
            if (ViewBag.Message != null)
            {
                ViewBag.SaveStatus = (int)TempData["MessageType"];
            }
            mdlWingMarkupWraper mdl = new mdlWingMarkupWraper();
            if (Id != null)
            {
                int ID = 0;
                int.TryParse(Id, out ID);
                if (ID > 0)
                {
                    mdl.WingMarkup = markup.LoadConvenience(0).FirstOrDefault();
                }

            }
            if (mdl.WingMarkup == null)
            {
                mdl.WingMarkup = new mdlWingMarkup();
            }

            mdl.SetDefaultDropDown(_context);
            return View(mdl);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Convenience(mdlWingMarkupWraper mdl, string submitData, [FromServices] IMarkup markup)
        {
            bool IsUpdated = false;
            if (mdl.WingMarkup.Id > 0)
            {
                IsUpdated = true;
            }
            if (mdl.WingMarkup == null)
            {
                ModelState.AddModelError("", "Invalid Data");
            }
            else
            {
                if (!mdl.WingMarkup.IsAllCustomerType)
                {
                    if (mdl.WingMarkup.MarkupCustomerType == null || mdl.WingMarkup.MarkupCustomerType.Count == 0)
                    {
                        ModelState.AddModelError("Convenience.IsAllCustomerType", "Select Customer Type");
                    }
                }
                if (!mdl.WingMarkup.IsAllCustomer)
                {
                    if (mdl.WingMarkup.MarkupCustomerDetail == null || mdl.WingMarkup.MarkupCustomerDetail.Count == 0)
                    {
                        ModelState.AddModelError("Convenience.IsAllCustomer", "Select Customer");
                    }
                }
                if (!mdl.WingMarkup.IsAllPessengerType)
                {
                    if (mdl.WingMarkup.MarkupPassengerType == null || mdl.WingMarkup.MarkupPassengerType.Count == 0)
                    {
                        ModelState.AddModelError("Convenience.IsAllPessengerType", "Select Passenger Type");
                    }
                }
                if (!mdl.WingMarkup.IsAllProvider)
                {
                    if (mdl.WingMarkup.MarkupServiceProvider == null || mdl.WingMarkup.MarkupServiceProvider.Count == 0)
                    {
                        ModelState.AddModelError("Convenience.IsAllProvider", "Select Service Provider");
                    }
                }
                if (!mdl.WingMarkup.IsAllFlightClass)
                {
                    if (mdl.WingMarkup.MarkupCabinClass == null || mdl.WingMarkup.MarkupCabinClass.Count == 0)
                    {
                        ModelState.AddModelError("Convenience.IsAllFlightClass", "Select Flight Class");
                    }
                }
                if (!mdl.WingMarkup.IsAllAirline)
                {
                    if (mdl.WingMarkup.MarkupAirline == null || mdl.WingMarkup.MarkupAirline.Count == 0)
                    {
                        ModelState.AddModelError("Convenience.IsAllAirline", "Select Airlines");
                    }
                }
                if (mdl.WingMarkup.Id == 0)
                {
                    if (mdl.WingMarkup.EffectiveFromDt < Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy")))
                    {
                        ModelState.AddModelError("Convenience.EffectiveFromDt", "Effective FromDate should be greater then Today");
                    }
                }

                if (mdl.WingMarkup.Amount <= 0)
                {
                    ModelState.AddModelError("Convenience.Amount", "Amount Should be Greater then 0");
                }
            }
            if (ModelState.IsValid)
            {
                if (submitData == "deleteData")
                {
                    if (markup.RemoveConvenience(mdl.WingMarkup.Id, _userId))
                    {
                        TempData["MessageType"] = (int)enmMessageType.Success;
                        TempData["Message"] = _setting.GetErrorMessage(enmMessage.DeleteSuccessfully);
                        return RedirectToAction("Convenience");
                    }
                }
                else
                {
                    if (IsUpdated)
                    {
                        if (markup.RemoveConvenience(mdl.WingMarkup.Id, _userId) && markup.AddConvenience(mdl.WingMarkup, _userId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);
                            return RedirectToAction("Convenience");
                        }
                    }
                    else
                    {
                        if (markup.AddConvenience(mdl.WingMarkup, _userId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);
                            return RedirectToAction("Convenience");
                        }
                    }
                }


            }
            else
            {
                ViewBag.MessageType = (int)enmMessageType.Warning;
                ViewBag.Message = "Data not valid";
            }
            mdl.SetDefaultDropDown(_context);
            return View(mdl);
        }


        [Authorize]
        public async Task<IActionResult> Discount(string Id, [FromServices] IMarkup markup)
        {
            ViewBag.Message = TempData["Message"];
            if (ViewBag.Message != null)
            {
                ViewBag.SaveStatus = (int)TempData["MessageType"];
            }
            mdlWingMarkupWraper mdl = new mdlWingMarkupWraper();
            if (Id != null)
            {
                int ID = 0;
                int.TryParse(Id, out ID);
                if (ID > 0)
                {
                    mdl.WingMarkup = markup.LoadDiscount(0).FirstOrDefault();
                }

            }
            if (mdl.WingMarkup == null)
            {
                mdl.WingMarkup = new mdlWingMarkup();
            }

            mdl.SetDefaultDropDown(_context);
            return View(mdl);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Discount(mdlWingMarkupWraper mdl, string submitData, [FromServices] IMarkup markup)
        {
            bool IsUpdated = false;
            if (mdl.WingMarkup.Id > 0)
            {
                IsUpdated = true;
            }
            if (mdl.WingMarkup == null)
            {
                ModelState.AddModelError("", "Invalid Data");
            }
            else
            {
                if (!mdl.WingMarkup.IsAllCustomerType)
                {
                    if (mdl.WingMarkup.MarkupCustomerType == null || mdl.WingMarkup.MarkupCustomerType.Count == 0)
                    {
                        ModelState.AddModelError("Discount.IsAllCustomerType", "Select Customer Type");
                    }
                }
                if (!mdl.WingMarkup.IsAllCustomer)
                {
                    if (mdl.WingMarkup.MarkupCustomerDetail == null || mdl.WingMarkup.MarkupCustomerDetail.Count == 0)
                    {
                        ModelState.AddModelError("Discount.IsAllCustomer", "Select Customer");
                    }
                }
                if (!mdl.WingMarkup.IsAllPessengerType)
                {
                    if (mdl.WingMarkup.MarkupPassengerType == null || mdl.WingMarkup.MarkupPassengerType.Count == 0)
                    {
                        ModelState.AddModelError("Discount.IsAllPessengerType", "Select Passenger Type");
                    }
                }
                if (!mdl.WingMarkup.IsAllProvider)
                {
                    if (mdl.WingMarkup.MarkupServiceProvider == null || mdl.WingMarkup.MarkupServiceProvider.Count == 0)
                    {
                        ModelState.AddModelError("Discount.IsAllProvider", "Select Service Provider");
                    }
                }
                if (!mdl.WingMarkup.IsAllFlightClass)
                {
                    if (mdl.WingMarkup.MarkupCabinClass == null || mdl.WingMarkup.MarkupCabinClass.Count == 0)
                    {
                        ModelState.AddModelError("Discount.IsAllFlightClass", "Select Flight Class");
                    }
                }
                if (!mdl.WingMarkup.IsAllAirline)
                {
                    if (mdl.WingMarkup.MarkupAirline == null || mdl.WingMarkup.MarkupAirline.Count == 0)
                    {
                        ModelState.AddModelError("Discount.IsAllAirline", "Select Airlines");
                    }
                }
                if (mdl.WingMarkup.Id == 0)
                {
                    if (mdl.WingMarkup.EffectiveFromDt < Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy")))
                    {
                        ModelState.AddModelError("Discount.EffectiveFromDt", "Effective FromDate should be greater then Today");
                    }
                }

                if (mdl.WingMarkup.Amount <= 0)
                {
                    ModelState.AddModelError("Discount.Amount", "Amount Should be Greater then 0");
                }
            }
            if (ModelState.IsValid)
            {
                if (submitData == "deleteData")
                {
                    if (markup.RemoveDiscount(mdl.WingMarkup.Id, _userId))
                    {
                        TempData["MessageType"] = (int)enmMessageType.Success;
                        TempData["Message"] = _setting.GetErrorMessage(enmMessage.DeleteSuccessfully);
                        return RedirectToAction("Discount");
                    }
                }
                else
                {
                    if (IsUpdated)
                    {
                        if (markup.RemoveDiscount(mdl.WingMarkup.Id, _userId) && markup.AddDiscount(mdl.WingMarkup, _userId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);
                            return RedirectToAction("Discount");
                        }
                    }
                    else
                    {
                        if (markup.AddDiscount(mdl.WingMarkup, _userId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);
                            return RedirectToAction("Discount");
                        }
                    }
                }


            }
            else
            {
                ViewBag.MessageType = (int)enmMessageType.Warning;
                ViewBag.Message = "Data not valid";
            }
            mdl.SetDefaultDropDown(_context);
            return View(mdl);
        }


        [Authorize]
        public async Task<IActionResult> CustomerFlightAPI(string Id, [FromServices] IMarkup markup)
        {
            ViewBag.Message = TempData["Message"];
            if (ViewBag.Message != null)
            {
                ViewBag.SaveStatus = (int)TempData["MessageType"];
            }
            mdlWingMarkupWraper mdl = new mdlWingMarkupWraper();
            if (Id != null)
            {
                int ID = 0;
                int.TryParse(Id, out ID);
                if (ID > 0)
                {
                    mdl.WingMarkup = markup.LoadCustomerFlightAPI(0).FirstOrDefault();
                }

            }
            if (mdl.WingMarkup == null)
            {
                mdl.WingMarkup = new mdlWingMarkup();
            }

            mdl.SetDefaultDropDown(_context);
            return View(mdl);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CustomerFlightAPI(mdlWingMarkupWraper mdl, string submitData, [FromServices] IMarkup markup)
        {
            bool IsUpdated = false;
            if (mdl.WingMarkup.Id > 0)
            {
                IsUpdated = true;
            }
            if (mdl.WingMarkup == null)
            {
                ModelState.AddModelError("", "Invalid Data");
            }
            else
            {
                if (!mdl.WingMarkup.IsAllCustomerType)
                {
                    if (mdl.WingMarkup.MarkupCustomerType == null || mdl.WingMarkup.MarkupCustomerType.Count == 0)
                    {
                        ModelState.AddModelError("Discount.IsAllCustomerType", "Select Customer Type");
                    }
                }
                if (!mdl.WingMarkup.IsAllCustomer)
                {
                    if (mdl.WingMarkup.MarkupCustomerDetail == null || mdl.WingMarkup.MarkupCustomerDetail.Count == 0)
                    {
                        ModelState.AddModelError("Discount.IsAllCustomer", "Select Customer");
                    }
                }
                
                if (!mdl.WingMarkup.IsAllProvider)
                {
                    if (mdl.WingMarkup.MarkupServiceProvider == null || mdl.WingMarkup.MarkupServiceProvider.Count == 0)
                    {
                        ModelState.AddModelError("Discount.IsAllProvider", "Select Service Provider");
                    }
                }
                if (!mdl.WingMarkup.IsAllFlightClass)
                {
                    if (mdl.WingMarkup.MarkupCabinClass == null || mdl.WingMarkup.MarkupCabinClass.Count == 0)
                    {
                        ModelState.AddModelError("Discount.IsAllFlightClass", "Select Flight Class");
                    }
                }
                if (!mdl.WingMarkup.IsAllAirline)
                {
                    if (mdl.WingMarkup.MarkupAirline == null || mdl.WingMarkup.MarkupAirline.Count == 0)
                    {
                        ModelState.AddModelError("Discount.IsAllAirline", "Select Airlines");
                    }
                }
                if (mdl.WingMarkup.Id == 0)
                {
                    if (mdl.WingMarkup.EffectiveFromDt < Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy")))
                    {
                        ModelState.AddModelError("Discount.EffectiveFromDt", "Effective FromDate should be greater then Today");
                    }
                }

                 
            }
            if (ModelState.IsValid)
            {
                if (submitData == "deleteData")
                {
                    if (markup.RemoveCustomerFlightAPI(mdl.WingMarkup.Id, _userId))
                    {
                        TempData["MessageType"] = (int)enmMessageType.Success;
                        TempData["Message"] = _setting.GetErrorMessage(enmMessage.DeleteSuccessfully);
                        return RedirectToAction("CustomerFlightAPI");
                    }
                }
                else
                {
                    if (IsUpdated)
                    {
                        if (markup.RemoveCustomerFlightAPI(mdl.WingMarkup.Id, _userId) && markup.AddCustomerFlightAPI(mdl.WingMarkup, _userId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);
                            return RedirectToAction("CustomerFlightAPI");
                        }
                    }
                    else
                    {
                        if (markup.AddCustomerFlightAPI(mdl.WingMarkup, _userId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);
                            return RedirectToAction("CustomerFlightAPI");
                        }
                    }
                }


            }
            else
            {
                ViewBag.MessageType = (int)enmMessageType.Warning;
                ViewBag.Message = "Data not valid";
            }
            mdl.SetDefaultDropDown(_context);
            return View(mdl);
        }

    }
}
