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

namespace B2bApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _context;
        private readonly IBooking _booking;
        public HomeController(ILogger<HomeController> logger, DBContext context, IBooking booking

            )
        {
            _context = context;
            _logger = logger;
            _booking = booking;
        }

        [Authorize]
        public IActionResult Index()
        {
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


        [Authorize]
        public async Task<dynamic> GetMenuAsync([FromServices] ICurrentUsers currentUsers,[FromServices] IAccount account)
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
        public async Task<IActionResult> FlightSearch(mdlFlightSearch mdl)
        {
            int CustomerId = 1;
            if (ModelState.IsValid)
            {
                mdl.LoadDefaultSearchRequestAsync(_booking);
                _booking.CustomerId = CustomerId;
                mdl.searchResponse = (await _booking.SearchFlightMinPrices(mdl.searchRequest));                
            }
            await mdl.LoadAirportAsync(_booking);
            return View(mdl);
        }




        [HttpPost]        
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> FlightReview(mdlFlightReview mdl,bool IsPriceChanged,string Message)
        {
            int CustomerId = 1;
            
            _booking.CustomerId = CustomerId;

            mdl.FareQuotResponse = new List<mdlFareQuotResponse>();
            mdl.FareRule = new List<mdlFareRuleResponse>();
            

            if (!(mdl == null || mdl.FareQuoteRequest==null))
            {
                mdl.FareQuotResponse.AddRange( await _booking.FareQuoteAsync(mdl.FareQuoteRequest));
                //mdl.FareRule.AddRange(await _booking.FareRule(new mdlFareRuleRequest() { TraceId= mdl.FareQuoteRequest.TraceId, ResultIndex= mdl.FareQuoteRequest.ResultIndex }));
                mdl.BookingRequestDefaultData();
            }
            else
            {
                return RedirectToAction("FlightSearch", "Home");
            }
            return View(mdl);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> FlightBook(mdlFlightReview mdl, [FromServices]ICustomerWallet customerWallet )
        {
            mdlFlighBook mdlres = new mdlFlighBook() {FareQuotResponse= new List<mdlFareQuotResponse>(), IsSucess=new List<bool>() , BookingId= new List<string>()};
            
            int CustomerId = 1;
            _booking.CustomerId = CustomerId;
            mdl.FareQuotResponse = new List<mdlFareQuotResponse>();
            mdl.FareRule = new List<mdlFareRuleResponse>();
            bool IsPriceChanged = false;
            if (!(mdl == null || mdl.FareQuoteRequest == null))
            {
                mdl.FareQuotResponse.AddRange(await _booking.FareQuoteAsync(mdl.FareQuoteRequest));
                mdl.FareQuotResponse.Any(p => p.IsPriceChanged);
                if (IsPriceChanged)
                {
                    return RedirectToAction("FlightReview", "Home", new { mdl = mdl, IsPriceChanged = true,Message="Price changed" });
                }

                if (mdl.FareQuotResponse.Sum(p => p.TotalPriceInfo?.TotalFare) > mdl.TotalFare)
                {
                    return RedirectToAction("FlightReview", "Home", new { mdl = mdl, IsPriceChanged = true, Message = "Price changed" });
                }
                customerWallet.CustomerId = CustomerId;
                double Walletbalence=await customerWallet.GetBalenceAsync();
                if (Walletbalence < mdl.TotalFare)
                {
                    return RedirectToAction("FlightReview", "Home", new { mdl = mdl, IsPriceChanged = true, Message = "Insufficient Wallet balence" });
                }
                else
                {
                    await customerWallet.DeductBalenceAsync(DateTime.Now ,mdl.TotalFare,enmTransactionType.TicketBook, string.Concat("Booking Ids", string.Join( ',',mdl.FareQuotResponse.Select(p=>p.BookingId))) );
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
                        TraceId = mdl.FareQuotResponse[i].TraceId,
                        BookingId = mdl.FareQuotResponse[i].BookingId,
                        travellerInfo = mdl.travellerInfo,
                        deliveryInfo = new mdlDeliveryinfo() { contacts = cont, emails = eml },
                        gstInfo = mdl.gstInfo,
                        paymentInfos = pi
                    };
                    var Result=  await _booking.BookingAsync(mdlReq);
                    mdlres.FareQuotResponse.Add(mdl.FareQuotResponse[i]);
                    mdlres.IsSucess.Add(Result.ResponseStatus == 1 ? true : false);
                    mdlres.BookingId.Add(Result.bookingId);

                }
            }
            else
            {
                return RedirectToAction("FlightSearch", "Home");
            }
            return View(mdl);
        }




    }
}
