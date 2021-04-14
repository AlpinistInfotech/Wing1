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
            mdlFlightSearch flightSearch = new mdlFlightSearch();
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


        


        


    }
}
