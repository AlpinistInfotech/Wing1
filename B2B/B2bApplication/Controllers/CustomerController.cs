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

namespace B2bApplication.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly DBContext _context;
        private readonly IBooking _booking;
        private readonly ISettings _setting;
        public CustomerController(ILogger<CustomerController> logger, DBContext context, IBooking booking,
            ISettings setting
            )
        {
            _context = context;
            _logger = logger;
            _setting = setting;
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
        public IActionResult AddCustomer(enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            mdlAddCustomer mdl = new mdlAddCustomer();
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }

            return View(mdl);
        }


    }
}
