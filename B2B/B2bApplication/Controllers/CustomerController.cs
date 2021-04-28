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

        [AcceptVerbs("Get", "Post")]
        public async  Task<IActionResult> CustomerCodeValidate(string CustomerCode)
        {
            if (CustomerCode  != null)
            {
                var validSp = false;
                if (_context.tblCustomerMaster.Any(p => p.Code == CustomerCode))
                    validSp = true;

                return Json(validSp);
            }

            return Json(true);

        }


        #region Add Customer        
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
                if (_context.tblCustomerMaster.Any(p => p.Code == mdl.CustomerCode))
                {
                    TempData["MessageType"] = (int)enmMessageType.Warning;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.RecordAlreadyExists);
                }

                else
                {
                    _context.tblCustomerMaster.Add(new tblCustomerMaster
                    {
                        Code = mdl.CustomerCode,
                        CustomerName = mdl.CustomerName,
                        CustomerType = mdl.customerType,
                        Address = mdl.Address,
                        ContactNo = mdl.MobileNo,
                        AlternateNo = mdl.AlternateMobileNo,
                        Email = mdl.Email,
                        CreatedBy = _userid,
                        CreatedDt = DateTime.Now,
                        CreditBalence = 0,
                        WalletBalence = 0,
                        IsActive = true,
                    });


                    await _context.SaveChangesAsync();

                    TempData["MessageType"] = (int)enmMessageType.Success;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);

                    return RedirectToAction("AddCustomer");
                }
            }



            return View(mdl);
        }

        #endregion


        #region Add Customer User
        [Authorize]
        public IActionResult AddUser()
        {
            dynamic messagetype = TempData["MessageType"];
            mdlAddCustomerUser mdl = new mdlAddCustomerUser();
            if (messagetype != null)
            {
                ViewBag.SaveStatus = (int)messagetype;
                ViewBag.Message = TempData["Message"];

            }

            return View(mdl);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUserAsync(mdlAddCustomerUser mdl)
        {
            if (ModelState.IsValid)
            {
                if (_context.tblUserMaster.Any(p => p.UserName == mdl.UserName))
                {
                    TempData["MessageType"] = (int)enmMessageType.Warning;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.RecordAlreadyExists);
                }

                
                var data =  _context.tblCustomerMaster.Where(p => p.Code == mdl.CustomerCode).FirstOrDefault();
                if (data == null)
                {
                    TempData["MessageType"] = (int)enmMessageType.Error;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.InvalidData);
                }

                else
                {
                    _context.tblUserMaster.Add(new tblUserMaster
                    {
                        CustomerId = data.Id,
                        UserName = mdl.UserName,
                        Password = mdl.Password,
                        IsActive = mdl.Status,
                        CreatedBy = _userid,
                        CreatedDt = DateTime.Now

                    });


                    await _context.SaveChangesAsync();

                    TempData["MessageType"] = (int)enmMessageType.Success;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);

                    return RedirectToAction("AddUser");
                }
            }



            return View(mdl);
        }

        #endregion

    }
}
