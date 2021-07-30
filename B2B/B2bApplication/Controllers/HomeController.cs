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
using System.IO;

namespace B2bApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _context;
        private readonly IBooking _booking;
        private readonly ISettings _setting;
        private readonly IMarkup _markup;
        private readonly ICurrentUsers _currentUsers;

        public HomeController(ILogger<HomeController> logger, DBContext context, IBooking booking,
            ISettings setting, IMarkup markup, ICurrentUsers currentUsers
            )
        {
            _context = context;
            _logger = logger;
            _setting = setting;
            _booking = booking;
            _markup = markup;
            _currentUsers = currentUsers;
        }

        [Authorize]
        public IActionResult Index()
        {
            DateTime TodayFromDt = Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));
            DateTime TodayToDt = TodayFromDt.AddDays(1);
            ViewBag.FlightSearch = _context.tblFlightBookingMaster.Where(p => p.CustomerId == _currentUsers.CustomerId && p.CreatedDt >= TodayFromDt && p.CreatedDt < TodayToDt).Count();
            ViewBag.FlightBook = _context.tblFlightBookingMaster.Where(p => p.CustomerId == _currentUsers.CustomerId && p.BookingStatus== enmBookingStatus.Booked && p.CreatedDt >= TodayFromDt && p.CreatedDt < TodayToDt).Count();

            DateTime WeekFromDt = TodayFromDt.AddDays(0- TodayFromDt.DayOfWeek);
            DateTime WeekToDt = TodayToDt;
            ViewBag.FlightSearchWeek = _context.tblFlightBookingMaster.Where(p => p.CustomerId == _currentUsers.CustomerId && p.CreatedDt >= WeekFromDt && p.CreatedDt < WeekToDt).Count();
            ViewBag.FlightBookWeek = _context.tblFlightBookingMaster.Where(p => p.CustomerId == _currentUsers.CustomerId && p.BookingStatus == enmBookingStatus.Booked && p.CreatedDt >= WeekFromDt && p.CreatedDt < WeekToDt).Count();

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

        public IActionResult Hotel()
        {
            return View();
        }
        public IActionResult TrainBuses()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PageSearch()
        {
            mdlPageSearch mdl = new mdlPageSearch();
            return View(mdl);
        }

        [HttpPost]
        public async Task<IActionResult> PageSearchAsync(mdlPageSearch mdl, [FromServices] IAccount account)
        {
            bool LoadAll = false;
            if (string.IsNullOrEmpty(mdl.Pagename) || string.IsNullOrWhiteSpace(mdl.Pagename))
            {
                LoadAll = true;
            }

            List<Document> alldocument = new List<Document>();
            var eDms = (await account.GetEnmDocumentsAsync(_currentUsers.UserId));
            foreach (var doc in eDms)
            {
                var DD = doc.GetDocumentDetails();
                if (DD.DocumentType.HasFlag(enmDocumentType.DisplayMenu))
                {
                    if (LoadAll)
                    {
                        alldocument.Add(DD);
                    }
                    else
                    {
                        if (DD.Name.Trim().ToLower().Contains(mdl.Pagename.Trim().ToLower()) || DD.Description.Trim().ToLower().Contains(mdl.Pagename.Trim().ToLower()) || doc.ToString().Trim().ToLower().Contains(mdl.Pagename.Trim().ToLower()))
                        {
                            alldocument.Add(DD);
                        }
                    }
                    
                }
            }

            mdl.alldocument = alldocument;

            return View(mdl);
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


        [Authorize(policy: nameof(enmDocumentMaster.Flight))]        
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
        [Authorize(policy: nameof(enmDocumentMaster.Flight))]
        [ValidateAntiForgeryToken]        
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
                _markup.WingDiscountAmount(mdl.searchResponse.Results, mdl.FlightSearchWraper.AdultCount, mdl.FlightSearchWraper.ChildCount, mdl.FlightSearchWraper.InfantCount);

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
        [Authorize(policy: nameof(enmDocumentMaster.Flight))]
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
            await mdl.LoadFareQuotationAsync(_currentUsers.CustomerId, _booking, _markup, _context);

            //save Data
            await _booking.CustomerFlightDetailSave(mdl.FareQuoteRequest.TraceId, mdl.FareQuotResponse);
            mdl.BookingRequestDefaultData();
            return View(mdl);
        }



        [AcceptVerbs("Post")]
        [ValidateAntiForgeryToken]
        
        [Authorize(policy: nameof(enmDocumentMaster.Flight))]
        public async Task<IActionResult> FlightReview(mdlFlightReview mdl)
        {
            
            if (mdl == null)
            {
                mdl = new mdlFlightReview();
            }
            await mdl.LoadFareQuotationAsync(_currentUsers.CustomerId, _booking, _markup, _context);            
            mdl.BookingRequestDefaultData();
            return View(mdl);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]     
        [Route("/Home/FlightCancel")]
        public async Task<mdlStatus> FlightCancelAsync(mdlFlightCancel mdl,  [FromServices] ICurrentUsers currentUsers,[FromServices] ICustomerWallet customerWallet)
        {
            mdlStatus returnData= new mdlStatus()
            {
                httpStatus = 200,
                message = null,
                success = false
            };

            if (mdl.passengers== null || mdl.passengers.Count==0 || mdl.passengers.All(p=>p.check==false))
            {
                returnData.message = "Please select a Passenger";
                return returnData;
            }

            if (currentUsers.HaveClaim(enmDocumentMaster.Booking_Flight_Cancel))
            {

                var flightDetails=_context.tblFlightBookingMaster.Where(p => p.Id == mdl.traceId).FirstOrDefault();
                if (flightDetails == null)
                {
                    returnData.message = "Invalid Request";
                    return returnData;
                }
                if (!(currentUsers.CustomerType == enmCustomerType.Admin || currentUsers.CustomerId == flightDetails.CustomerId))
                {
                    returnData.message = "Unauthorize Request";
                    return returnData;
                }
                var SegmentData=  _context.tblFlightBookingSegmentMaster.Where(p => p.TraceId == mdl.traceId && p.SegmentDisplayOrder == mdl.segementDisplayOrder).FirstOrDefault();
                if (SegmentData == null)
                {
                    returnData.message = "Invalid Request";
                    return returnData;
                }
                if (SegmentData.BookingStatus == enmBookingStatus.Refund)
                {
                    returnData.message = "Already Refunded";
                    return returnData;
                }
                if (SegmentData.BookingStatus != enmBookingStatus.Booked)
                {
                    returnData.message = "Ticket not booked";
                    return returnData;
                }

                List<int> PasengerId = mdl.passengers.Where(p => p.check).Select(p => p.pid).ToList();
                var passengerList=_context.tblFlightBookingPassengerDetails.Where(p => PasengerId.Contains(p.Id));

                mdlCancellationTripDetail _mdlCancellationTripDetail = new mdlCancellationTripDetail()
                {
                    srcAirport = SegmentData.Origin,
                    destAirport = SegmentData.Destination,
                    departureDate = SegmentData.TravelDt,
                    travellers = passengerList.Select(q => new mdlTravellerBasicInfo { FirstName = q.FirstName, LastName = q.LastName }).ToArray()
                };
                mdlCancellationTripDetail[] _mdlCancellationTripDetails = { _mdlCancellationTripDetail };

                mdlCancellationRequest mdlRq = new mdlCancellationRequest()
                {
                    bookingId = SegmentData.BookingId,
                    remarks = mdl.remarks,
                    TraceId = mdl.traceId,
                    type = "CANCELLATION",
                    trips = _mdlCancellationTripDetails,
                };
                mdlFlightCancellationResponse res = await _booking.CancelationAsync(mdlRq, customerWallet);
                if (res.ResponseStatus == 1)
                {
                    returnData.success = true;
                    returnData.message = "Cancel Successfully " + res.amendmentId;
                    return returnData;
                }
                else
                {
                    returnData.message = res.Error?.Message;
                    return returnData;
                }
            }
            else
            {
                return new mdlStatus()
                {
                    httpStatus = 401,
                    message = "Unautorize Access",
                    success = false
                };
            }
            //return new mdlStatus()
            //{
            //    httpStatus = 401,
            //    message = "Unautorize Access",
            //    success = false
            //};


        }


        
        [Authorize]
        [HttpPost]
        [Route("/Home/FlightCancelDetails")]
        public async Task<IActionResult> FlightCancelDetailsAsync(mdlFlightCancel mdl, [FromServices] ICurrentUsers currentUsers)
        {
            ViewBag.HaveError = false;
            ViewBag.ErrorMessage = "";
            var flightDetails = _context.tblFlightBookingMaster.Where(p => p.Id == mdl.traceId).FirstOrDefault();
            if (flightDetails == null)
            {
                ViewBag.HaveError = true; ViewBag.ErrorMessage = "Invalid Request";
                return PartialView("_FlightCancelDetail", null);

            }
            if (!(currentUsers.CustomerType == enmCustomerType.Admin || currentUsers.CustomerId == flightDetails.CustomerId))
            {
                ViewBag.HaveError = true; ViewBag.ErrorMessage = "Unauthorize Request";
                return PartialView("_FlightCancelDetail", null);
            }
            var SegmentData = _context.tblFlightBookingSegmentMaster.Where(p => p.TraceId == mdl.traceId && p.SegmentDisplayOrder == mdl.segementDisplayOrder).FirstOrDefault();
            if (SegmentData == null)
            {
                ViewBag.HaveError = true; ViewBag.ErrorMessage = "Invalid Request";
                return PartialView("_FlightCancelDetail", null);
            }
            if (SegmentData.BookingStatus != enmBookingStatus.Booked)
            {
                ViewBag.HaveError = true; ViewBag.ErrorMessage = "Ticket not booked";
                return PartialView("_FlightCancelDetail", null);
            }

            List<int> PasengerId = mdl.passengers.Where(p => p.check).Select(p => p.pid).ToList();
            var passengerList = _context.tblFlightBookingPassengerDetails.Where(p => PasengerId.Contains(p.Id));

            mdlCancellationTripDetail _mdlCancellationTripDetail = new mdlCancellationTripDetail()
            {
                srcAirport = SegmentData.Origin,
                destAirport = SegmentData.Destination,
                departureDate = SegmentData.TravelDt,
                travellers = passengerList.Select(q => new mdlTravellerBasicInfo { FirstName = q.FirstName, LastName = q.LastName }).ToArray()
            };
            mdlCancellationTripDetail[] _mdlCancellationTripDetails = { _mdlCancellationTripDetail };

            mdlCancellationRequest mdlRq = new mdlCancellationRequest()
            {
                bookingId = SegmentData.BookingId,
                remarks = mdl.remarks,
                TraceId = mdl.traceId,
                type = "CANCELLATION",
                trips = _mdlCancellationTripDetails,
            };
            var mdlRes=await _booking.CancelationChargeAsync(mdlRq);
            return PartialView("_FlightCancelDetail", mdlRes);
        }

        [AcceptVerbs("Get", "Post")]
        [ValidateAntiForgeryToken]
        [Authorize(policy: nameof(enmDocumentMaster.Flight))]
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

                string CustomerMPin=_context.tblCustomerBalance.Where(p => p.CustomerId == _currentUsers.CustomerId).FirstOrDefault()?.MPin ?? "0000";
                if (CustomerMPin != mdl.Mpin)
                {
                    TempData["mdl_"] = s;
                    TempData["MessageType"] = (int)enmMessageType.Error;
                    TempData["Message"] =_setting.GetErrorMessage(enmMessage.MpinNotMatch);
                    return RedirectToAction("FlightReview");
                }

                await mdl.LoadFareQuotationAsync(_currentUsers.CustomerId, _booking, _markup,_context);
                IsPriceChanged = mdl.FareQuotResponse.Any(p => p.IsPriceChanged);
                if (IsPriceChanged)
                {
                    
                    TempData["mdl_"] = s;
                    TempData["MessageType"] = (int)enmMessageType.Warning;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.FlightPriceChanged);
                    return RedirectToAction("FlightReview");                    
                }
                
                customerWallet.CustomerId = _currentUsers.CustomerId;
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
                    await customerWallet.DeductBalanceAsync(DateTime.Now, mdl.NetFare, enmTransactionType.FlightTicketBook, mdl.FareQuoteRequest.TraceId);
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


        [HttpGet]
        [Authorize(policy: nameof(enmDocumentMaster.Markup))]
        public IActionResult WingMarkup(string Id,[FromServices] IMarkup markup)
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
                    mdl.WingMarkup = markup.LoadMarkup(ID).FirstOrDefault();
                }
                
            }
            if (mdl.WingMarkup == null)
            {
                mdl.WingMarkup = new mdlWingMarkup();
            }
            
            mdl.SetDefaultDropDown(_context);
            return View(mdl);
        }

        [Authorize(policy: nameof(enmDocumentMaster.Markup))]
        [HttpPost]
        public IActionResult WingMarkup(mdlWingMarkupWraper mdl,string submitData, [FromServices]IMarkup markup)
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
                    if (markup.RemoveMarkup(mdl.WingMarkup.Id, _currentUsers.UserId ))
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
                        if (markup.RemoveMarkup(mdl.WingMarkup.Id, _currentUsers.UserId) && markup.AddMarkup(mdl.WingMarkup, _currentUsers.UserId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);
                            return RedirectToAction("WingMarkup");
                        }
                    }
                    else
                    {
                        if (markup.AddMarkup(mdl.WingMarkup, _currentUsers.UserId))
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


        [HttpGet]
        [Authorize(policy: nameof(enmDocumentMaster.ConvenienceFee))]
        public IActionResult Convenience(string Id, [FromServices] IMarkup markup)
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
                    mdl.WingMarkup = markup.LoadConvenience(ID).FirstOrDefault();
                }

            }
            if (mdl.WingMarkup == null)
            {
                mdl.WingMarkup = new mdlWingMarkup();
            }

            mdl.SetDefaultDropDown(_context);
            return View(mdl);
        }

        [Authorize(policy: nameof(enmDocumentMaster.ConvenienceFee))]
        [HttpPost]
        public IActionResult Convenience(mdlWingMarkupWraper mdl, string submitData, [FromServices] IMarkup markup)
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
                    if (markup.RemoveConvenience(mdl.WingMarkup.Id, _currentUsers.UserId))
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
                        if (markup.RemoveConvenience(mdl.WingMarkup.Id, _currentUsers.UserId) && markup.AddConvenience(mdl.WingMarkup, _currentUsers.UserId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);
                            return RedirectToAction("Convenience");
                        }
                    }
                    else
                    {
                        if (markup.AddConvenience(mdl.WingMarkup, _currentUsers.UserId))
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

        [HttpGet]
        [Authorize(policy: nameof(enmDocumentMaster.Discount))]
        public IActionResult Discount(string Id, [FromServices] IMarkup markup)
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
                    mdl.WingMarkup = markup.LoadDiscount(ID).FirstOrDefault();
                }

            }
            if (mdl.WingMarkup == null)
            {
                mdl.WingMarkup = new mdlWingMarkup();
            }

            mdl.SetDefaultDropDown(_context);
            return View(mdl);
        }

        [Authorize(policy: nameof(enmDocumentMaster.Discount))]
        [HttpPost]
        public IActionResult Discount(mdlWingMarkupWraper mdl, string submitData, [FromServices] IMarkup markup)
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
                    if (markup.RemoveDiscount(mdl.WingMarkup.Id, _currentUsers.UserId))
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
                        if (markup.RemoveDiscount(mdl.WingMarkup.Id, _currentUsers.UserId) && markup.AddDiscount(mdl.WingMarkup, _currentUsers.UserId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);
                            return RedirectToAction("Discount");
                        }
                    }
                    else
                    {
                        if (markup.AddDiscount(mdl.WingMarkup, _currentUsers.UserId))
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
        public IActionResult CustomerFlightAPI(string Id, [FromServices] IMarkup markup)
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
                    mdl.WingMarkup = markup.LoadCustomerFlightAPI(ID).FirstOrDefault();
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
        public IActionResult CustomerFlightAPI(mdlWingMarkupWraper mdl, string submitData, [FromServices] IMarkup markup)
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
                    if (markup.RemoveCustomerFlightAPI(mdl.WingMarkup.Id, _currentUsers.UserId))
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
                        if (markup.RemoveCustomerFlightAPI(mdl.WingMarkup.Id, _currentUsers.UserId) && markup.AddCustomerFlightAPI(mdl.WingMarkup, _currentUsers.UserId))
                        {
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);
                            return RedirectToAction("CustomerFlightAPI");
                        }
                    }
                    else
                    {
                        if (markup.AddCustomerFlightAPI(mdl.WingMarkup, _currentUsers.UserId))
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

        public IActionResult ProviderSettings()
        {
           
            return View();
        }
        [HttpPost]
        public IActionResult ProviderSettings(mdlProviderSettings mdl,tblActiveSerivceProvider provider)
        {

            
            mdl.ModifiedDt = System.DateTime.Now;
            mdl.ModifiedBy = _currentUsers.Name;
            //tblActiveSerivceProvider provider = new tblActiveSerivceProvider();

            if (ModelState.IsValid)
            {

                var dbdata = _context.tblActiveSerivceProvider.Where(x => x.ServiceProvider == mdl.ServiceProvider ).FirstOrDefault();
                if (dbdata != null)
                {
                    dbdata.IsEnabled = provider.IsEnabled;
                    dbdata.Remarks = provider.Remarks;
                    dbdata.ModifiedDt = mdl.ModifiedDt;
                    dbdata.ModifiedBy = _currentUsers.UserId;
                    _context.Entry(dbdata).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    //_context.tblActiveSerivceProvider.Add(provider);
                    //_context.SaveChanges();
                    //ViewBag.Success = "Data Inserted";
                }
                else
                {
                    ViewBag.Success = "Data not inserted";
                }

            }
            return View();
        }


        [Authorize(nameof(enmDocumentMaster.PackageReport))]
        public IActionResult PackageReport()
        {
            mdlPackageReports mdl = new mdlPackageReports();
            mdl.Packagedata = new List<tblPackageMaster>();
            return View(mdl);
        }
        [HttpPost]
        [Authorize(nameof(enmDocumentMaster.PackageReport))]
        public async Task<IActionResult> PackageReport(mdlPackageReports mdl)
        {
            _booking.FromDate =Convert.ToDateTime( mdl.FromDate.ToString("dd-MMM-yyyy"));
            _booking.ToDate = Convert.ToDateTime(mdl.ToDate.AddDays(1).AddSeconds(-1).ToString("dd-MMM-yyyy"));
            mdl.Packagedata = (await _booking.LoadPackage(0,false, false, true,true)).OrderByDescending(p => p.CreatedDt).ToList();
            return View(mdl);
        }

        [Authorize(nameof(enmDocumentMaster.CreatePackage))]
        public async Task<IActionResult> CreatePackage(string Id,[FromServices]IConfiguration configuration)
        {
            ViewBag.Message = TempData["Message"];
            if (ViewBag.Message != null)
            {
                ViewBag.SaveStatus = (int)TempData["MessageType"];
            }

            string filePath = configuration["FileUpload:PackageFilePath"];

            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + filePath);
            mdlPackageMaster mdl = new mdlPackageMaster();
            if (Id != null)
            {
                int PackageId = 0;
                int.TryParse(Id, out PackageId);
                if (PackageId > 0)
                {


                    var pdata= (await _booking.LoadPackage(PackageId, false, false, false, false)).FirstOrDefault();
                    if (pdata != null)
                    {
                        mdl.PackageId = pdata.PackageId;
                        mdl.PackageName = pdata.PackageName;
                        mdl.LocationName = pdata.LocationName;
                        mdl.IsDomestic = pdata.IsDomestic;
                        mdl.ShortDescription = pdata.ShortDescription;
                        mdl.LongDescription = pdata.LongDescription;
                        mdl.ThumbnailImage = pdata.ThumbnailImage;
                        mdl.AllImage = pdata.AllImage;
                        mdl.EffectiveFromDt = pdata.EffectiveFromDt;
                        mdl.EffectiveToDt = pdata.EffectiveToDt;
                        mdl.AdultPrice = pdata.AdultPrice;
                        mdl.ChildPrice = pdata.ChildPrice;
                        mdl.InfantPrice = pdata.InfantPrice;
                        mdl.IsActive = pdata.IsActive;
                        mdl.NumberOfDay = pdata.NumberOfDay;
                        mdl.NumberOfNight = pdata.NumberOfNight;
                        mdl.fileDataPackageImage = new List<byte[]>();
                        var files = pdata.AllImage.Split(",");
                        foreach (var file in files)
                        {
                            mdl.fileDataPackageImage.Add(System.IO.File.ReadAllBytes(string.Concat(path, file)));
                        }
                        mdl.fileDataThumbnail = System.IO.File.ReadAllBytes(string.Concat(path, pdata.ThumbnailImage));

                    }
                }
            }
            return View(mdl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(nameof(enmDocumentMaster.CreatePackage))]
        public async Task<IActionResult> CreatePackage(mdlPackageMaster mdl, [FromServices] IConfiguration configuration)
        {
            
            string filePath = configuration["FileUpload:PackageFilePath"];

            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + filePath);
            if (mdl.PackageId == 0)
            {
                if (mdl.UploadPackageImage == null || mdl.UploadPackageImage.Count == 0 || mdl.UploadPackageImage[0] == null || mdl.UploadPackageImage[0].Length == 0)
                {
                    ModelState.AddModelError("UploadPackageImage", "Invalid Files");
                    ViewBag.SaveStatus = enmSaveStatus.danger;
                    ViewBag.Message = "Invalid Thumbnail";
                    //RedirectToAction("CreatePackage",new {Id= mdl.PackageId});
                }
                if (mdl.UploadPackageThumbnail == null || mdl.UploadPackageThumbnail.Length == 0)
                {
                    ModelState.AddModelError("UploadPackageThumbnail", "Invalid Files");
                    ViewBag.SaveStatus = enmSaveStatus.danger;
                    ViewBag.Message = "Invalid Thumbnail";
                    //RedirectToAction("CreatePackage", new { Id = mdl.PackageId });
                }

            }
            

            if (ModelState.IsValid)
            {
                List<string> AllFileName = new List<string>();
                string thumbnail = string.Empty;
                bool exists = System.IO.Directory.Exists(path);
                if (!exists)
                    System.IO.Directory.CreateDirectory(path);
                if (mdl.UploadPackageImage != null)
                {
                    foreach (var file in mdl.UploadPackageImage)
                    {
                        if (file == null || file.Length == 0)
                        {
                            continue;
                        }
                        var filename = Guid.NewGuid().ToString() + ".jpeg";
                        using (var stream = new FileStream(string.Concat(path, filename), FileMode.Create))
                        {
                            AllFileName.Add(filename);
                            await file.CopyToAsync(stream);
                        }
                    }
                }
                if  (!(mdl.UploadPackageThumbnail == null || mdl.UploadPackageThumbnail.Length == 0))
                {
                    thumbnail = Guid.NewGuid().ToString() + ".jpeg";
                    using (var stream = new FileStream(string.Concat(path, thumbnail), FileMode.Create))
                    {
                        await mdl.UploadPackageThumbnail.CopyToAsync(stream);
                    }
                }
                if (mdl.PackageId > 0)
                {
                    var pData = _context.tblPackageMaster.Where(p => p.PackageId == mdl.PackageId).FirstOrDefault();
                    if (pData == null)
                    {
                        ViewBag.SaveStatus = enmSaveStatus.danger;
                        ViewBag.Message = "Invalid Thumbnail";
                    }
                    else
                    {

                        if (AllFileName.Count > 0)
                        {
                            pData.AllImage = string.Join(",", AllFileName);
                        }
                        if (thumbnail != string.Empty)
                        {
                            pData.ThumbnailImage = thumbnail;
                        }
                        pData.PackageName = mdl.PackageName;
                        pData.LocationName = mdl.LocationName;
                        pData.IsDomestic = mdl.IsDomestic;
                        pData.ShortDescription = mdl.ShortDescription;
                        pData.LongDescription = mdl.LongDescription;
                        pData.EffectiveFromDt = Convert.ToDateTime(mdl.EffectiveFromDt.ToString("dd-MMM-yyyy"));
                        pData.EffectiveToDt = Convert.ToDateTime(mdl.EffectiveToDt.ToString("dd-MMM-yyyy"));
                        pData.NumberOfDay = mdl.NumberOfDay;
                        pData.NumberOfNight = mdl.NumberOfNight==0? mdl.NumberOfDay-1: mdl.NumberOfNight;
                        pData.AdultPrice = mdl.AdultPrice;
                        pData.ChildPrice = mdl.ChildPrice;
                        pData.InfantPrice = mdl.InfantPrice;
                        pData.IsActive = mdl.IsActive;
                        pData.ModifiedDt = DateTime.Now;
                        pData.ModifiedBy = _currentUsers.UserId;
                        _context.tblPackageMaster.Update(pData);
                        await _context.SaveChangesAsync();
                        TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);
                        TempData["MessageType"] =(int) enmSaveStatus.success;
                        return RedirectToAction("CreatePackage");
                    }
                }
                else
                {
                    var pData = new tblPackageMaster();
                    if (AllFileName.Count > 0)
                    {
                        pData.AllImage = string.Join(",", AllFileName);
                    }
                    if (thumbnail != string.Empty)
                    {
                        pData.ThumbnailImage = thumbnail;
                    }
                    pData.PackageName = mdl.PackageName;
                    pData.LocationName = mdl.LocationName;
                    pData.IsDomestic = mdl.IsDomestic;
                    pData.ShortDescription = mdl.ShortDescription;
                    pData.LongDescription = mdl.LongDescription;
                    pData.EffectiveFromDt = Convert.ToDateTime( mdl.EffectiveFromDt.ToString("dd-MMM-yyyy"));
                    pData.EffectiveToDt = Convert.ToDateTime(mdl.EffectiveToDt.ToString("dd-MMM-yyyy")) ;
                    pData.NumberOfDay = mdl.NumberOfDay;
                    pData.NumberOfNight = mdl.NumberOfNight;
                    pData.AdultPrice = mdl.AdultPrice;
                    pData.ChildPrice = mdl.ChildPrice;
                    pData.InfantPrice = mdl.InfantPrice;
                    pData.IsActive = mdl.IsActive;
                    pData.ModifiedDt = DateTime.Now;
                    pData.ModifiedBy = _currentUsers.UserId;
                    pData.CreatedBy = pData.ModifiedBy.Value;
                    pData.CreatedDt= pData.ModifiedDt.Value;
                    _context.tblPackageMaster.Add(pData);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);
                    TempData["MessageType"] = (int)enmSaveStatus.success;
                    return RedirectToAction("CreatePackage");
                }
                
            }
            
            return View(mdl);
        }
    }
}
