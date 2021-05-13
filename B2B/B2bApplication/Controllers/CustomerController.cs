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
        public IActionResult AddCustomer(string Id)
        {
            dynamic messagetype = TempData["MessageType"];
            mdlAddCustomer mdl = new mdlAddCustomer();
            if (messagetype != null)
            {
                ViewBag.SaveStatus = (int)messagetype;
                ViewBag.Message = TempData["Message"];
                
            }

            if (Id != null)
            {
                
                var userdata =  GetCustomerData(_context,Convert.ToInt32(Id));

                if (userdata != null)
                {
                    mdl.CustomerCode = userdata.Code;
                    mdl.CustomerName = userdata.CustomerName;
                    mdl.customerType = userdata.CustomerType;
                    mdl.Address = userdata.Address;
                    mdl.MobileNo = userdata.ContactNo;
                    mdl.AlternateMobileNo = userdata.AlternateNo;
                    mdl.Email = userdata.Email;
                    mdl.customerid = userdata.Id;
                    mdl.Status = userdata.IsActive;

                }
            }
                return View(mdl);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCustomerAsync(mdlAddCustomer mdl)
        {
            if (ModelState.IsValid)
            {
                //if (_context.tblCustomerMaster.Any(p => p.Code == mdl.CustomerCode))
                //{
                //    TempData["MessageType"] = (int)enmMessageType.Warning;
                //    TempData["Message"] = _setting.GetErrorMessage(enmMessage.RecordAlreadyExists);
                //}

                var ExistingData = _context.tblCustomerMaster.FirstOrDefault(p => p.Code == mdl.CustomerCode);
                if (ExistingData != null)
                {
                    if (ExistingData.Id != mdl.customerid) // already exists
                    {
                        TempData["MessageType"] = (int)enmMessageType.Warning;
                        TempData["Message"] = _setting.GetErrorMessage(enmMessage.RecordAlreadyExists);
                        ViewBag.SaveStatus = (int)TempData["MessageType"];
                        ViewBag.Message = TempData["Message"];
                    }
                    else  // update 
                    {
                         
                        ExistingData.Address = mdl.Address;
                        ExistingData.ContactNo = mdl.MobileNo;
                        ExistingData.AlternateNo = mdl.AlternateMobileNo;
                        ExistingData.Email = mdl.Email;
                        ExistingData.IsActive = mdl.Status;
                        
                         _context.tblCustomerMaster.Update(ExistingData);
                        await _context.SaveChangesAsync();
                        TempData["MessageType"] = (int)enmMessageType.Success;
                        TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);

                        return RedirectToAction("AddCustomer");
                    }

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
                        IsActive = mdl.Status,
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
        public IActionResult AddUser(string Id)
        {

            dynamic messagetype = TempData["MessageType"];
            mdlAddCustomerUser mdl = new mdlAddCustomerUser();
            if (messagetype != null)
            {
                ViewBag.SaveStatus = (int)messagetype;
                ViewBag.Message = TempData["Message"];

            }

            if (Id != null)
            {
                var userdata =   GetCustomerUserData(_context, Convert.ToInt32(Id));

                if (userdata != null)
                {
                    mdl.CustomerID =Convert.ToString( userdata.CustomerId);
                    mdl.UserName = userdata.UserName;
                    mdl.Password = userdata.Password;
                    mdl.Status = userdata.IsActive;
                    mdl.userid = userdata.Id;

                }

                 mdl.UserMasters =  GetCustomerUserList(_context, true, Convert.ToInt32(mdl.CustomerID));
            }
            ViewBag.CustomerCodeList = new SelectList( GetCustomerMaster(_context, true,0).Select(p=> new {Code=p.Id,CustomerName=p.CustomerName+"("+p.Code+")"}), "Code", "CustomerName", mdl.CustomerID);
            return View(mdl);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUserAsync(mdlAddCustomerUser mdl,string submittype)
        {
            if (ModelState.IsValid)
            {
                
                if (submittype == "LoadData")
                {
                    ViewBag.CustomerCodeList = new SelectList( GetCustomerMaster(_context, true,0).Select(p => new { Code = p.Id, CustomerName = p.CustomerName + "(" + p.Code + ")" }), "Code", "CustomerName", mdl.CustomerID);
                    mdl.UserMasters =  GetCustomerUserList(_context, false, Convert.ToInt32(mdl.CustomerID));

                }
                else
                {
                    if (mdl.UserName == null || mdl.UserName == "")
                    {
                        CallCustomerUserDefaultSetting(0,mdl);
                    }

                    else if (mdl.Password == null || mdl.Password == "")
                    {
                         CallCustomerUserDefaultSetting(1, mdl);
                    }
                    else
                    {
                        var ExistingData = _context.tblUserMaster.FirstOrDefault(p => p.UserName == mdl.UserName);
                        if (ExistingData != null)
                        {
                            if (ExistingData.Id != mdl.userid) // already exists
                            {
                                CallCustomerUserDefaultSetting(2, mdl);
                            }
                            else  // update 
                            {
                                ExistingData.UserName = mdl.UserName;
                                ExistingData.Password = mdl.Password;
                                ExistingData.IsActive = mdl.Status;
                                _context.tblUserMaster.Update(ExistingData);
                                await _context.SaveChangesAsync();
                                TempData["MessageType"] = (int)enmMessageType.Success;
                                TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);

                                return RedirectToAction("AddUser");
                            }




                        }

                        else
                        {
                            _context.tblUserMaster.Add(new tblUserMaster
                            {
                                CustomerId = Convert.ToInt32(mdl.CustomerID),
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
                }
            }
          
            return View(mdl);
        }

        private void CallCustomerUserDefaultSetting(int value, mdlAddCustomerUser mdl)
        {
            if (value == 0)
            {
                TempData["MessageType"] = (int)enmMessageType.Error;
                TempData["Message"] = "Please enter User Name";
            }
            else if (value == 1)
            {
                TempData["MessageType"] = (int)enmMessageType.Error;
                TempData["Message"] = "Please enter Password";
            }
            else if (value == 2)
            {
                TempData["MessageType"] = (int)enmMessageType.Warning;
                TempData["Message"] = _setting.GetErrorMessage(enmMessage.RecordAlreadyExists);
            }
           
            ViewBag.CustomerCodeList = new SelectList(GetCustomerMaster(_context, true, 0).Select(p => new { Code = p.Id, CustomerName = p.CustomerName + "(" + p.Code + ")" }), "Code", "CustomerName", mdl.CustomerID);
            mdl.UserMasters = GetCustomerUserList(_context, false, Convert.ToInt32(mdl.CustomerID));
            ViewBag.SaveStatus = (int)TempData["MessageType"];
            ViewBag.Message = TempData["Message"];
        }

        #endregion

        #region Customer Markup
        [Authorize]
        public IActionResult CustomerMarkUp(string Id)
        {

            dynamic messagetype = TempData["MessageType"];
            mdlCustomerMarkup mdl = new mdlCustomerMarkup();
            if (messagetype != null)
            {
                ViewBag.SaveStatus = (int)messagetype;
                ViewBag.Message = TempData["Message"];

            }

            if (Id != null)
            {
                var markupdata = GetCustomerMarkUpData(_context, Convert.ToInt32(Id));// fetch data through query string

                if (markupdata != null)
                {
                    mdl.CustomerID = Convert.ToString(markupdata.CustomerId);
                    mdl.MarkupValue = markupdata.MarkupAmt;
                    mdl.markupid = markupdata.Id;
                }
            }
            if (_userid == 1) // if admin the show all customer id
            {
                mdl.MarkupData = GetCustomerMarkUpList(_context, 0);
                ViewBag.CustomerCodeList = new SelectList(GetCustomerMaster(_context, true, 0).Select(p => new { Code = p.Id, CustomerName = p.CustomerName + "(" + p.Code + ")" }), "Code", "CustomerName", mdl.CustomerID);
            }
            else
            {
                mdl.MarkupData = GetCustomerMarkUpList(_context, _userid);
                ViewBag.CustomerCodeList = new SelectList(GetCustomerMaster(_context, true, _userid).Select(p => new { Code = p.Id, CustomerName = p.CustomerName + "(" + p.Code + ")" }), "Code", "CustomerName", mdl.CustomerID);
            }
            return View(mdl);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CustomerMarkUpAsync(mdlCustomerMarkup mdl )
        {
            if (ModelState.IsValid)
            {
                        var ExistingData = _context.tblCustomerMarkup.FirstOrDefault(p => p.CustomerId == Convert.ToInt32(mdl.CustomerID));
                        if (ExistingData != null)
                        {
                            //if (ExistingData.Id != mdl.markupid) // already exists
                            //{
                            //    TempData["MessageType"] = (int)enmMessageType.Warning;
                            //    TempData["Message"] = _setting.GetErrorMessage(enmMessage.RecordAlreadyExists);
                            //    ViewBag.SaveStatus = (int)TempData["MessageType"];
                            //    ViewBag.Message = TempData["Message"];
                            //}
                            //else  // update 
                            {
                                ExistingData.MarkupAmt = mdl.MarkupValue;
                                  
                                ExistingData.ModifiedBy= _userid;
                                ExistingData.CustomerId = Convert.ToInt32(mdl.CustomerID);
                                ExistingData.ModifiedDt = DateTime.Now;
                                _context.tblCustomerMarkup.Update(ExistingData);
                                await _context.SaveChangesAsync();
                                TempData["MessageType"] = (int)enmMessageType.Success;
                                TempData["Message"] = _setting.GetErrorMessage(enmMessage.UpdateSuccessfully);

                                return RedirectToAction("CustomerMarkUp");
                            }
                        }

                        else
                        {
                            _context.tblCustomerMarkup.Add(new tblCustomerMarkup
                            {
                                CustomerId = Convert.ToInt32(mdl.CustomerID),
                                MarkupAmt = mdl.MarkupValue,
                                CreatedBy = _userid,
                                CreatedDt = DateTime.Now

                            });


                            await _context.SaveChangesAsync();

                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);

                            return RedirectToAction("CustomerMarkUp");
                        }
                    }
            return View(mdl);
        }


        #endregion


        #region  Customer Details
        [Authorize]
        public IActionResult CustomerDetails()
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
        public async Task<IActionResult> CustomerDetailsAsync(mdlAddCustomer mdl)
        {
            mdl.CustomerMasters = GetCustomerMaster(_context, mdl.Status, 0);
            return View(mdl);
        }

        #endregion


        #region Customer Wallet
        [Authorize]
        public IActionResult CustomerWallet()
        {

            dynamic messagetype = TempData["MessageType"];
            mdlCustomerWallet mdl = new mdlCustomerWallet();
            if (messagetype != null)
            {
                ViewBag.SaveStatus = (int)messagetype;
                ViewBag.Message = TempData["Message"];

            }

            ViewBag.CustomerCodeList = new SelectList(GetCustomerMaster(_context, true, 0).Select(p => new { Code = p.Id, CustomerName = p.CustomerName + "(" + p.Code + ")" }), "Code", "CustomerName", mdl.CustomerID);
            return View(mdl);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CustomerWalletAsync(mdlCustomerWallet mdl)
        {
            {
                _context.tblWalletDetailLedger.Add(new tblWalletDetailLedger
                {
                    CustomerId = Convert.ToInt32(mdl.CustomerID),
                    Credit = mdl.creditDebit == 0 ? mdl.WalletAmt : 0,
                    Debit = mdl.creditDebit == 0 ? 0 : mdl.WalletAmt,
                    Remarks = mdl.Remarks,
                    TransactionDetails = mdl.TransactionDetails,
                    TransactionDt = mdl.TransactionDate
                });

                var ExistingData = _context.tblCustomerMaster.FirstOrDefault(p => p.Id == Convert.ToInt32(mdl.CustomerID));
                {
                    ExistingData.WalletBalence = mdl.creditDebit == 0 ? mdl.WalletAmt : 0 - mdl.WalletAmt;
                    _context.tblCustomerMaster.Update(ExistingData);
                }

                await _context.SaveChangesAsync();

                TempData["MessageType"] = (int)enmMessageType.Success;
                TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);

                return RedirectToAction("CustomerWallet");

         //       return View(mdl);
            }
        }


        #endregion

        public List<tblCustomerMaster> GetCustomerMaster(DBContext context, bool OnlyActive, int customerid)
        {
            if (customerid > 0)
            {
                return context.tblCustomerMaster.Where(p => p.Id == customerid).ToList();
            }
            else
            {
                if (OnlyActive)
                {
                    return context.tblCustomerMaster.Where(p => p.IsActive).ToList();
                }
                else
                {
                    return context.tblCustomerMaster.ToList();
                }
            }

        }

        public List<tblUserMaster> GetCustomerUserList(DBContext context, bool OnlyActive, int customerid)
        {
            if (OnlyActive)
            {
                return context.tblUserMaster.Where(p => p.IsActive && p.CustomerId == customerid).ToList();
            }
            else
            {
                return context.tblUserMaster.Where(p => p.CustomerId == customerid).ToList();
            }

        }

        public List<tblCustomerMarkup> GetCustomerMarkUpList(DBContext context, int customerid)
        {

            if (customerid > 0)
                return context.tblCustomerMarkup.Where(p => p.CustomerId == customerid).ToList();
            else
                return context.tblCustomerMarkup.ToList();

        }

        public tblUserMaster GetCustomerUserData(DBContext context, int userid)
        {
            return context.tblUserMaster.Where(p => p.Id == userid).FirstOrDefault();
        }

        public tblCustomerMarkup GetCustomerMarkUpData(DBContext context, int markupid)
        {
            return context.tblCustomerMarkup.Where(p => p.Id == markupid).FirstOrDefault();
        }
        public tblCustomerMaster GetCustomerData(DBContext context, int customerid)
        {
            return context.tblCustomerMaster.Where(p => p.Id == customerid).FirstOrDefault();
        }

    }
}
