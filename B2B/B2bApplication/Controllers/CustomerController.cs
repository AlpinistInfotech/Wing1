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
using Database;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace B2bApplication.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly DBContext _context;
        private readonly ISettings _setting;
        private readonly IConfiguration _config;
        int _userid = 0;
        int _customerId = 0;
        public CustomerController(ILogger<CustomerController> logger, DBContext context, ISettings setting,IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _setting = setting;
            _config = config;
            _customerId = 1;
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
            }
            mdl.UserMasters = GetCustomerUserList(_context, true, Convert.ToInt32(mdl.CustomerID));
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

        #region Wallet Statement

        public IActionResult CustomerwalletReport()
        {
            mdlCustomerWallet mdl = new mdlCustomerWallet();
            ViewBag.CustomerCodeList = new SelectList(GetCustomerMaster(_context, true, 0).Select(p => new { Code = p.Id, CustomerName = p.CustomerName + "(" + p.Code + ")" }), "Code", "CustomerName", mdl.CustomerID);
            
            mdlCustomerWalletReport returnDataMdl = new mdlCustomerWalletReport();
            returnDataMdl.mdlTcWalletWraper = new List<ProcWalletSearch>();
            return View(returnDataMdl);
            //return View(mdl);

        }

        [HttpPost]
        public IActionResult CustomerwalletReport(mdlCustomerWalletReport mdl, string submitdata)
        {
            ViewBag.CustomerCodeList = new SelectList(GetCustomerMaster(_context, true, 0).Select(p => new { Code = p.Id, CustomerName = p.CustomerName + "(" + p.Code + ")" }), "Code", "CustomerName", mdl.CustomerID);

            mdl.mdlTcWalletWraper = GetTCWalletStatement(mdl, 0, 0, true);
            return View(mdl);
        }

        #endregion

        public List<ProcWalletSearch> GetTCWalletStatement(mdlCustomerWalletReport mdl, int Nid, int spmode, bool LoadImage)
        {
            List<ProcWalletSearch> returnData = new List<ProcWalletSearch>();

            using (SqlConnection sqlconnection = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand("proc_wallet_search", sqlconnection))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Add(new SqlParameter("datefrom", mdl.FromDt));
                    sqlcmd.Parameters.Add(new SqlParameter("dateto", mdl.ToDt));
                    sqlcmd.Parameters.Add(new SqlParameter("id", mdl.CustomerID));
                    sqlcmd.Parameters.Add(new SqlParameter("session_nid", Nid));
                    sqlcmd.Parameters.Add(new SqlParameter("spmode", spmode));


                    sqlconnection.Open();
                    SqlDataReader rd = sqlcmd.ExecuteReader();
                    while (rd.Read())
                    {
                        returnData.Add(new ProcWalletSearch()
                        {
                            Date = Convert.ToString(rd["date_"]),
                            Particulars = Convert.ToString(rd["Particulars"]),
                            Credit = Convert.ToDecimal(rd["Credit"]),
                            Debit = Convert.ToDecimal(rd["Debit"]),
                            Balance = Convert.ToDecimal(rd["Balance"]),
                            current_ewallet_amt = Convert.ToDecimal(rd["current_ewallet_amt"]),
                        });
                    }
                }

            }

            return returnData;

        }

        #region CustomerIPFilter
        public IActionResult CustomerIPFilter(string Id)
        {
            mdlCustomerIPFilter mdl = new mdlCustomerIPFilter();
            dynamic messagetype = TempData["MessageType"];
            if (messagetype != null)
            {
                ViewBag.SaveStatus = (int)messagetype;
                ViewBag.Message = TempData["Message"];
            }

            
            if (Id != null)
            {
                var ipfilterdata = GetIPFilterData(_context, Convert.ToInt32(Id));

                if (ipfilterdata  != null)
                {
                    mdl.CustomerID = Convert.ToInt32(ipfilterdata.CustomerId);
                    mdl.allipapplicable = ipfilterdata.AllowedAllIp;
                    mdl.IPFilterId = ipfilterdata.Id;
                    mdl.IPAddess = string.Join(',',ipfilterdata.tblCustomerIPFilterDetails.Select(p=>p.IPAddress));
                    
                }
            }

            mdl.IPFilterReport = GetCustomerIPFilterList(_context, true, Convert.ToInt32(mdl.CustomerID));
            ViewBag.CustomerCodeList = new SelectList(GetCustomerMaster(_context, true, 0).Select(p => new { Code = p.Id, CustomerName = p.CustomerName + "(" + p.Code + ")" }), "Code", "CustomerName", mdl.CustomerID);
            return View(mdl);
        }

     
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CustomerIPFilterAsync(mdlCustomerIPFilter mdl,string submittype)
        {
            if (ModelState.IsValid)
            {

                if (submittype == "LoadData")
                {
                    mdl.IPFilterReport = GetCustomerIPFilterList(_context, true, Convert.ToInt32(mdl.CustomerID));
                    ViewBag.CustomerCodeList = new SelectList(GetCustomerMaster(_context, true, 0).Select(p => new { Code = p.Id, CustomerName = p.CustomerName + "(" + p.Code + ")" }), "Code", "CustomerName", mdl.CustomerID);
                }
                else
                {
                    {
                        var ExistingData_ipfilter = _context.tblCustomerIPFilter.FirstOrDefault(p => p.Id == mdl.IPFilterId);
                        if (ExistingData_ipfilter != null)
                        {
                            // run delete command
                            ExistingData_ipfilter.IsDeleted = true;
                            ExistingData_ipfilter.ModifiedDt = DateTime.Now;
                            ExistingData_ipfilter.ModifiedBy = _userid;

                            await _context.SaveChangesAsync();
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.DeleteSuccessfully);
                            return RedirectToAction("CustomerIPFilter");
                        }
                        else // save button call
                        {

                            if (mdl.allipapplicable == false && mdl.IPAddess.Trim().Length == 0)
                            {
                                TempData["MessageType"] = (int)enmMessageType.Warning;
                                TempData["Message"] = "Please enter IP Address";
                                return RedirectToAction("CustomerIPFilter");
                            }


                            var ExistingData_check = _context.tblCustomerIPFilter.FirstOrDefault(p => p.CustomerId == mdl.CustomerID);
                            if (ExistingData_check != null)
                            {
                                ExistingData_check.IsDeleted = true;
                                ExistingData_check.ModifiedDt = DateTime.Now;
                                ExistingData_check.ModifiedBy = _userid;
                                _context.tblCustomerIPFilter.Update(ExistingData_check);
                            }

                            tblCustomerIPFilter ipfilter_ = new tblCustomerIPFilter()
                            {
                                CustomerId = mdl.CustomerID,
                                AllowedAllIp = mdl.allipapplicable,
                                CreatedBy = _userid,
                                CreatedDt = DateTime.Now,
                                tblCustomerIPFilterDetails = mdl.IPAddess.Split(",").Select(p => new tblCustomerIPFilterDetails { IPAddress = p }).ToList() };
                                 _context.tblCustomerIPFilter.Add(ipfilter_);
                            }

                        await _context.SaveChangesAsync();

                        TempData["MessageType"] = (int)enmMessageType.Success;
                        TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);

                        return RedirectToAction("CustomerIPFilter");
                    }

                            
                        }
                    }

            return View(mdl);
        }


        #endregion

        #region CreditRequest

        [Authorize]
        public IActionResult CreditRequest()
        {

            dynamic messagetype = TempData["MessageType"];
            mdlCreditRequest mdl = new mdlCreditRequest();
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
        public async Task<IActionResult> CreditRequestAsync(mdlCreditRequest mdl)
        {

            DateTime fromdate= Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));
            DateTime todate= Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("dd-MMM-yyyy"));
            var ExistingData = _context.tblCreditRequest.FirstOrDefault(p => p.CustomerId == mdl.CustomerID && p.CreditAmt == mdl.CreditAmt && p.CreatedDt >= fromdate && p.CreatedDt < todate);
            if (ExistingData!=null)
            {
                TempData["MessageType"] = (int)enmMessageType.Warning;
                TempData["Message"] = _setting.GetErrorMessage(enmMessage.RecordAlreadyExists);   
            }
            else
            {
                _context.tblCreditRequest.Add(new tblCreditRequest
                {
                    CustomerId = Convert.ToInt32(mdl.CustomerID),
                    CreditAmt = mdl.CreditAmt,
                    CreatedRemarks = mdl.Remarks,
                    CreatedBy = _userid,
                    CreatedDt = DateTime.Now
                });

                await _context.SaveChangesAsync();

                TempData["MessageType"] = (int)enmMessageType.Success;
                TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);
            }
                return RedirectToAction("CreditRequest");

        }

        #endregion


        #region Credit Approval

        public IActionResult CreditApproval()
        {
            dynamic messagetype = TempData["MessageType"];
            mdlCreditRequest mdl = new mdlCreditRequest();
            if (messagetype != null)
            {
                ViewBag.SaveStatus = (int)messagetype;
                ViewBag.Message = TempData["Message"];
            }
            mdl.CreditRequestList = GetCreditRequest(_context, 0, 0);
            return View(mdl);
        }

        [HttpPost]
        public IActionResult CreditApproval(mdlCreditRequest mdl, string submitdata)
        {

            return View(mdl);
        }

        #endregion


        public List<tblCreditRequest> GetCreditRequest(DBContext context, enmApprovalStatus status, int customerid)
        {
            return context.tblCreditRequest.Where(p => p.Status == status).ToList();
        }

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

        // need to join with ipfilter detail table
        //
        public List<tblCustomerIPFilter> GetCustomerIPFilterList(DBContext context, bool OnlyActive, int customerid)
        {
            return context.tblCustomerIPFilter.Where(p => p.CustomerId == customerid && !p.IsDeleted).Include(p => p.tblCustomerIPFilterDetails).ToList();
        }

        // need to join with ipfilter detail table
        public tblCustomerIPFilter GetIPFilterData(DBContext context, int ipfilterid)
        {
            return context.tblCustomerIPFilter.Where(p => p.Id == ipfilterid && !p.IsDeleted).Include(p => p.tblCustomerIPFilterDetails).FirstOrDefault();
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


        #region ********************* Customer Flight Booking Report *****************************
        [Authorize]
        [HttpGet]
        public IActionResult FlightBookingReport()
        {
            mdlFlightBookingReport mdl = new mdlFlightBookingReport();
            return View(mdl);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FlightBookingReport(mdlFlightBookingReport mdl,[FromServices]IBooking _booking)
        {
            if (ModelState.IsValid)
            {
                mdl.loadBookingData(_booking,_customerId);
            }
            return View(mdl);
        }

        #endregion

    }
}
