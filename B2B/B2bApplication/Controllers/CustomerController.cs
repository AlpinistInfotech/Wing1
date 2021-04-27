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
        private readonly ISettings _setting;
        int _userid = 1;
        public CustomerController(ILogger<CustomerController> logger, DBContext context, ISettings setting)
        {
            _context = context;
            _logger = logger;
            _setting = setting;
        }

        
        
        [Authorize]
        public IActionResult AddCustomer()
        {
            dynamic messagetype = TempData["MessageType"];
            mdlAddCustomer mdl = new mdlAddCustomer();
            if (messagetype != null)
            {

                ViewBag.SaveStatus = (int)messagetype;
                ViewBag.Message = TempData["Message"];
                
            }

            return View(mdl);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCustomerAsync(mdlAddCustomer mdl)
        {
            if (ModelState.IsValid)
            {
                
                {
                    _context.tblCustomerMaster.Add(new tblCustomerMaster
                    {
                        Code = mdl.CustomerCode,
                        CustomerName = mdl.CustomerName,
                        CustomerType=mdl.customerType,
                        Address=mdl.Address,
                        ContactNo=mdl.MobileNo,
                        AlternateNo=mdl.AlternateMobileNo,
                        Email=mdl.Email,
                        CreatedBy=_userid,
                        CreatedDt=DateTime.Now,
                        CreditBalence=0,
                        WalletBalence=0,
                        IsActive=true,
                    });
                }

                await _context.SaveChangesAsync();

                TempData["MessageType"] = (int)enmMessageType.Success;
                TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);

                return RedirectToAction("AddCustomer");
            }



            return View(mdl);
        }


    }
}
