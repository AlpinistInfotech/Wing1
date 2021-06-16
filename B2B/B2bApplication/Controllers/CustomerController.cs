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
using System.IO;

namespace B2bApplication.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly DBContext _context;
        private readonly ISettings _setting;
        private readonly IConfiguration _config;
        private readonly int _userid ;
        private readonly int _customerId ;
        private readonly ICurrentUsers _currentUsers;
        private readonly ICustomerMaster _customerMaster;        
        public CustomerController(ILogger<CustomerController> logger, DBContext context, ISettings setting, IConfiguration config,
            ICurrentUsers currentUsers,
            ICustomerMaster customerMaster)
        {
            _context = context;
            _logger = logger;
            _setting = setting;
            _config = config;
            _currentUsers = currentUsers;
            _customerId = _currentUsers.CustomerId;
            _userid = _currentUsers.UserId;
            _customerMaster = customerMaster;
        }

        [AcceptVerbs("Get")]
        public dynamic GetState(String Id)
        {

            if (Id!= null )
            {
                int CountryId=0;
                int.TryParse(Id, out CountryId);
               return _context.tblStateMaster.Where(p => p.CountryId == CountryId && p.IsActive).Select(p=>new { p.StateId,p.StateName } ).OrderBy(p=>p.StateName);
            }
            return BadRequest("Invalid Data");
        }


        [AcceptVerbs("Get", "Post")]
        public IActionResult CustomerCodeValidate(mdlCustomerMasterWraper mdl)
        {
            
            if (mdl != null && mdl.customerMaster != null)
            {   
                int CustomerId = mdl.customerMaster.CustomerId;
                if (_context.tblCustomerMaster.Any(p => p.Code == mdl.customerMaster.Code && p.Id != CustomerId))
                {
                    return Json(false);
                }                    
            }
            return Json(true);

        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult CustomerGSTNumberValidate(mdlCustomerMasterWraper mdl)
        {
            if (mdl != null && mdl.GSTDetails != null)
            {   
                int CustomerId_ = mdl.GSTDetails.CustomerId??0;
                if (_context.tblCustomerGSTDetails.Any(p => p.GstNumber == mdl.GSTDetails.GstNumber && p.CustomerId != CustomerId_))
                {
                    return Json(false);
                }
            }
            return Json(true);

        }


        #region ******************Customer Master *************************

        [HttpGet]
        [Authorize]
        public IActionResult CustomerDetail(string Id)
        {
            int CustomerId = _currentUsers.CustomerId;
            mdlCustomerMasterWraper mdl = new mdlCustomerMasterWraper();
            mdl.LoadData(CustomerId, _customerMaster, _config);
            mdl.SetWalletBalence(_context);
            if (_currentUsers.CustomerType == enmCustomerType.Admin)
            {
                mdl.LoadCustomer(_customerMaster);
            }
            return View(mdl);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult CustomerDetail(mdlCustomerMasterWraper mdl)
        {
            int CustomerId = 0;
            if (mdl == null)
            {
                mdl = new mdlCustomerMasterWraper();
               
            }
            CustomerId = mdl.CustomerId;
            mdl.LoadData(CustomerId, _customerMaster,_config);
            mdl.SetWalletBalence(_context);
            if (_currentUsers.CustomerType == enmCustomerType.Admin)
            {
                mdl.LoadCustomer(_customerMaster);
            }
            return View(mdl);
        }


        [HttpGet]
        public async Task<IActionResult> CustomerMaster(string Id,[FromServices]IMasters masters )
        {
            dynamic messagetype = TempData["MessageType"];            
            if (messagetype != null)
            {
                ViewBag.SaveStatus = (int)messagetype;
                ViewBag.Message = TempData["Message"];

            }

            int CustomerId=0;
            int.TryParse(Id, out CustomerId);
            mdlCustomerMasterWraper mdl = new mdlCustomerMasterWraper();
            mdl.LoadData(CustomerId, _customerMaster, _config);
            mdl.SetWalletBalence(_context);            
            mdl.SetCountryState(ViewBag, _context);
            return View(mdl);
        }

        [HttpPost]
        public async Task<IActionResult> CustomerMaster(mdlCustomerMasterWraper mdl, [FromServices] IMasters masters)
        {
            _customerMaster.CustomerId = mdl.CustomerId;
            mdl.DocumentPermission=_customerMaster.DocumentPermission;
            bool IsUpdate = mdl.CustomerId==0?false:true;
            bool HaveWriteData = false;
            if (!(mdl.customerMaster?.HaveGST ?? false))
            {
                foreach (var prop in mdl.GSTDetails.GetType().GetProperties())
                {
                    if (ModelState.Keys.Contains("GSTDetails." + prop.Name))
                    {
                        ModelState.Remove("GSTDetails." + prop.Name);
                    }                    
                }
                
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _customerMaster.BeginTransaction();
                    if (mdl.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_BasicDetail_Write) && mdl.customerMaster != null)
                    {
                        if (mdl.Logo != null)
                        {
                            string iconPath = _config["Organisation:IconPath"];
                            var path = Path.Combine(
                                 Directory.GetCurrentDirectory(),
                                 "wwwroot/" + iconPath);
                            bool exists = System.IO.Directory.Exists(path);
                            if (!exists)
                                System.IO.Directory.CreateDirectory(path);

                            var filename = Guid.NewGuid().ToString() + ".ico";
                            using (var stream = new FileStream(string.Concat(path, filename), FileMode.Create))
                            {
                                mdl.customerMaster.Logo = filename;
                                await mdl.Logo.CopyToAsync(stream);
                            }
                        }


                        

                        mdl.customerMaster.CustomerId = mdl.CustomerId;
                        if (await _customerMaster.SaveBasicDetailsAsync(mdl.customerMaster))
                        {   
                            HaveWriteData = true;
                            if (IsUpdate)
                            {
                                mdl.customerMaster.CustomerId = _customerMaster.CustomerId;
                                mdl.CustomerId = _customerMaster.CustomerId;
                            }
                        }

                    }
                    if (mdl.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Pan_Write) && mdl.pan != null)
                    {
                        mdl.pan.CustomerId = mdl.CustomerId;
                        if (await _customerMaster.SavePanDetailsAsync(mdl.pan))
                        {
                            HaveWriteData = true;                            
                        }
                    }
                    if (mdl.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Bank_Write) && mdl.banks != null)
                    {
                        mdl.banks.CustomerId = mdl.CustomerId;
                        if (await _customerMaster.SaveBankDetailsAsync(mdl.banks))
                        {
                            HaveWriteData = true;
                        }

                    }
                    if (mdl.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_GSTDetail_Write) && mdl.GSTDetails != null && mdl.customerMaster.HaveGST)
                    {
                        mdl.GSTDetails.CustomerId = mdl.CustomerId;
                        if (await _customerMaster.SaveGSTDetailsAsync(mdl.GSTDetails))
                        {
                            HaveWriteData = true;
                        }

                    }
                    if (mdl.DocumentPermission.Any(p => p == enmDocumentMaster.CustomerDetailsPermission_Setting_Write) && mdl.customerSetting != null)
                    {
                        mdl.customerSetting.CustomerId = mdl.CustomerId;
                        if (await _customerMaster.SaveSettingDetailsAsync(mdl.customerSetting))
                        {
                            HaveWriteData = true;
                        }
                    }

                    if ((_customerMaster.validationResultList?.Count ?? 0) > 0)
                    {
                        foreach (var err in _customerMaster.validationResultList)
                        {
                            ModelState.AddModelError(err.MemberNames.FirstOrDefault() ?? "", err.ErrorMessage);
                        }
                        ViewBag.SaveStatus = (int)enmMessageType.Error;
                        ViewBag.Message = _setting.GetErrorMessage(enmMessage.InvalidData);
                    }
                    else
                    {
                        if (HaveWriteData)
                        {
                            _customerMaster.ComitTransaction();
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);
                            return RedirectToAction("CustomerMaster","Customer" ,new {Id= mdl.CustomerId.ToString()} );
                        }
                        else
                        {
                            ViewBag.SaveStatus = (int)enmMessageType.Warning;
                            ViewBag.Message = _setting.GetErrorMessage(enmMessage.AccessDenied);
                        }
                    }
                    _customerMaster.RollbackTransaction();
                }
                catch (Exception ex)
                {
                    _customerMaster.RollbackTransaction();
                    ViewBag.SaveStatus = (int)enmMessageType.Error;
                    ViewBag.Message = ex.Message;
                }

            }
            else
            {
                ViewBag.SaveStatus = (int)enmMessageType.Error;
                ViewBag.Message = "Invalid Data";
            }



            mdl.SetCountryState(ViewBag, _context);
            mdl.SetWalletBalence(_context);
            return View(mdl);
        }



        #endregion

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
                    //mdl.markupid = markupdata.;
                }
            }
            if (_currentUsers.CustomerType == enmCustomerType.Admin) // if admin the show all customer id
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
        public async Task<IActionResult> CustomerWalletAsync(mdlCustomerWallet mdl, [FromServices] ICustomerWallet customerWallet)
        {
            {
                //_context.tblWalletDetailLedger.Add(new tblWalletDetailLedger
                //{
                //    CustomerId = Convert.ToInt32(mdl.CustomerID),
                //    Credit = mdl.creditDebit == 0 ? mdl.WalletAmt : 0,
                //    Debit = mdl.creditDebit == 0 ? 0 : mdl.WalletAmt,
                //    Remarks = mdl.Remarks,
                //    TransactionDetails = mdl.TransactionDetails,
                //    TransactionDt = mdl.TransactionDate,
                //    RequestedId=0
                //});

                //var ExistingData = _context.tblCustomerMaster.FirstOrDefault(p => p.Id == Convert.ToInt32(mdl.CustomerID));
                //{
                //    ExistingData.WalletBalence = mdl.creditDebit == 0 ? mdl.WalletAmt : 0 - mdl.WalletAmt;
                //    _context.tblCustomerMaster.Update(ExistingData);
                //}

                //await _context.SaveChangesAsync();
                if (mdl.creditDebit == 0)
                await  customerWallet.AddBalenceAsync(DateTime.Now, mdl.WalletAmt, enmTransactionType.WalletAmountUpdate, mdl.TransactionDetails, mdl.Remarks, 0);
                else
                await customerWallet.DeductBalenceAsync(DateTime.Now, mdl.WalletAmt, enmTransactionType.WalletAmountUpdate, mdl.TransactionDetails, mdl.Remarks, 0);

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
        
        public IActionResult CustomerChangePassword()
        {
            if (ModelState.IsValid)
            { 
                
            }
            return View();
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
                    //mdl.IPFilterId = ipfilterdata.Id;
                    mdl.IPAddess = string.Join(',',ipfilterdata.tblCustomerIPFilterDetails.Select(p=>p.IPAddress));
                    
                }
            }

            mdl.IPFilterReport = GetCustomerIPFilterList(_context, true, Convert.ToInt32(mdl.CustomerID));
            ViewBag.CustomerCodeList = new SelectList(GetCustomerMaster(_context, true, 0).Select(p => new { Code = p.Id, CustomerName = p.CustomerName + "(" + p.Code + ")" }), "Code", "CustomerName", mdl.CustomerID);
            return View(mdl);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CustomerIPFilterAsync(mdlCustomerIPFilter mdl, string submittype)
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
                    if (submittype == "deleteData")
                    {
                        var ExistingData_ipfilter = _context.tblCustomerIPFilter.FirstOrDefault(p => p.CustomerId== mdl.CustomerID);
                        if (ExistingData_ipfilter != null)
                        {
                            // run delete command
                            //ExistingData_ipfilter.IsDeleted = true;
                            ExistingData_ipfilter.ModifiedDt = DateTime.Now;
                            ExistingData_ipfilter.ModifiedBy = _userid;

                            await _context.SaveChangesAsync();
                            TempData["MessageType"] = (int)enmMessageType.Success;
                            TempData["Message"] = _setting.GetErrorMessage(enmMessage.DeleteSuccessfully);
                            return RedirectToAction("CustomerIPFilter");
                        }
                    }
                    else // save button call
                    {
                        if (mdl.allipapplicable == false && mdl.IPAddess.Trim().Length == 0)
                        {
                            TempData["MessageType"] = (int)enmMessageType.Warning;
                            TempData["Message"] = "Please enter IP Address";
                            return RedirectToAction("CustomerIPFilter");
                        }

                        var TobeUpdated = _context.tblCustomerIPFilter.Where(p => p.CustomerId == mdl.CustomerID).ToList();

                        using (var transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                TobeUpdated.ForEach(p =>
                                {
                                    //p.IsDeleted = true;
                                    p.ModifiedDt = DateTime.Now;
                                    p.ModifiedBy = _userid;
                                });

                                if (mdl.allipapplicable == false)
                                {
                                    tblCustomerIPFilter ipfilter_ = new tblCustomerIPFilter()
                                    {
                                        CustomerId = mdl.CustomerID,
                                        AllowedAllIp = mdl.allipapplicable,
                                        CreatedBy = _userid,
                                        CreatedDt = DateTime.Now,
                                        tblCustomerIPFilterDetails = mdl.IPAddess.Split(",").Select(p => new tblCustomerIPFilterDetails { IPAddress = p }).ToList()
                                    };
                                    _context.tblCustomerIPFilter.Add(ipfilter_);
                                }

                                else
                                {
                                    tblCustomerIPFilter ipfilter_ = new tblCustomerIPFilter()
                                    {
                                        CustomerId = mdl.CustomerID,
                                        AllowedAllIp = mdl.allipapplicable,
                                        CreatedBy = _userid,
                                        CreatedDt = DateTime.Now
                                    };
                                    _context.tblCustomerIPFilter.Add(ipfilter_);

                                }


                                await _context.SaveChangesAsync();
                                transaction.Commit();
                            }
                            catch
                            {
                                transaction.Rollback();
                            }
                        }
                    }


                    TempData["MessageType"] = (int)enmMessageType.Success;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);

                    return RedirectToAction("CustomerIPFilter");
                }
            }


            return View(mdl);
        }


        #endregion

        #region PaymentRequest

        [Authorize]
        public IActionResult PaymentRequest()
        {

            dynamic messagetype = TempData["MessageType"];
            mdlPaymentRequest mdl = new mdlPaymentRequest();
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
        public async Task<IActionResult> PaymentRequestAsync(mdlPaymentRequest mdl)
        {

            DateTime fromdate= Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));
            DateTime todate= Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("dd-MMM-yyyy"));
            var ExistingData = _context.tblPaymentRequest.FirstOrDefault(p => p.CustomerId == mdl.CustomerID && p.RequestedAmt == mdl.CreditAmt && p.CreatedDt >= fromdate && p.CreatedDt < todate);
            if (ExistingData!=null)
            {
                TempData["MessageType"] = (int)enmMessageType.Warning;
                TempData["Message"] = _setting.GetErrorMessage(enmMessage.RecordAlreadyExists);   
            }
            else
            {
                string filePath = _config["FileUpload:PaymentRequestFilePath"];

                var path = Path.Combine(
                         Directory.GetCurrentDirectory(),
                         "wwwroot/" + filePath);
                //if (mdl.UploadImages == null || mdl.UploadImages.Count == 0 || mdl.UploadImages[0] == null || mdl.UploadImages[0].Length == 0)
                //{
 
                //    TempData["MessageType"] = (int)enmSaveStatus.danger;
                //    TempData["Message"] = _setting.GetErrorMessage(enmMessage.InvalidDocument);
                //    return RedirectToAction("PaymentRequest");
                //}

                //List<string> AllFileName = new List<string>();

                //bool exists = System.IO.Directory.Exists(path);
                //if (!exists)
                //    System.IO.Directory.CreateDirectory(path);

                //foreach (var file in mdl.UploadImages)
                //{
                //    var filename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName) ;
                //    using (var stream = new FileStream(string.Concat(path, filename), FileMode.Create))
                //    {
                //        AllFileName.Add(filename);
                //        await file.CopyToAsync(stream);
                //    }
                //}

                _context.tblPaymentRequest.Add(new tblPaymentRequest
                {
                    CustomerId = Convert.ToInt32(mdl.CustomerID),
                    RequestedAmt = mdl.CreditAmt,
                    CreatedRemarks = mdl.Remarks,
                    RequestType=mdl.RequestType,
                    CreatedBy = _userid,
                    CreatedDt = DateTime.Now,
                    UploadImages = ""//string.Join<string>(",", AllFileName)
                });

                await _context.SaveChangesAsync();

                TempData["MessageType"] = (int)enmMessageType.Success;
                TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);
            }
                return RedirectToAction("PaymentRequest");

        }

        #endregion


        #region Credit Approval

        public IActionResult CreditApproval()
        {
            dynamic messagetype = TempData["MessageType"];
            mdlPaymentRequest mdl = new mdlPaymentRequest();
            if (messagetype != null)
            {
                ViewBag.SaveStatus = (int)messagetype;
                ViewBag.Message = TempData["Message"];
            }
            mdl.PaymentRequestList = GetPaymentRequest(_context, 0, 0);
            return View(mdl);
        }

        [HttpPost]
        public async Task<IActionResult> CreditApprovalAsync(mdlPaymentRequest mdl, [FromServices] ICustomerWallet customerWallet)
        {
            List<int> creditpending_checkedlist = mdl.PaymentRequestList.Where(p => p.paymentrequestid).Select(p => p.Id).ToList();
            var TobeUpdated = _context.tblPaymentRequest.Where(p => creditpending_checkedlist.Contains(p.Id) && p.Status == enmApprovalStatus.Pending).ToList();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                 TobeUpdated.ForEach(p =>
            {
                p.Status = mdl.Status;
                p.ModifiedDt = DateTime.Now;
                p.ModifiedBy = _userid;
                p.ModifiedRemarks = mdl.Remarks;
                if (mdl.Status == enmApprovalStatus.Approved)
                {
                    customerWallet.CustomerId = (int)p.CustomerId;
                    customerWallet.AddBalenceAsync(DateTime.Now, p.RequestedAmt, enmTransactionType.OnCreditUpdate, p.CreatedRemarks, mdl.Remarks,p.Id);
                }
            });
                    TempData["MessageType"] = (int)enmMessageType.Success;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.SaveSuccessfully);

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                }
                catch
                {
                    TempData["MessageType"] = (int)enmMessageType.Error;
                    TempData["Message"] = _setting.GetErrorMessage(enmMessage.InvalidData);

                    transaction.Rollback();
                }
            }
            mdl.PaymentRequestList = GetPaymentRequest(_context, 0, 0);
            return View(mdl);
        }

        #endregion


        public List<mdlPaymentRequestWraper> GetPaymentRequest(DBContext context, enmApprovalStatus status, int customerid)
        {
            string filePath ="wwwroot/"+ _config["FileUpload:PaymentRequestFilePath"];
                        
            return context.tblPaymentRequest.Where(p => p.Status == status).Select(p=>new mdlPaymentRequestWraper { Id= p.Id, CreatedDt=p.CreatedDt, CustomerId=p.CustomerId, RequestedAmt =p.RequestedAmt , CreatedRemarks=p.CreatedRemarks,CustomerName=p.tblCustomerMaster.CustomerName,Code=p.tblCustomerMaster.Code ,RequestType=p.RequestType,UploadImages= filePath+"/"+ p.UploadImages }).ToList();
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
            return context.tblCustomerIPFilter.Where(p => p.CustomerId == customerid ).Include(p => p.tblCustomerIPFilterDetails).ToList();
        }

        // need to join with ipfilter detail table
        public tblCustomerIPFilter GetIPFilterData(DBContext context, int ipfilterid)
        {
            return null;//context.tblCustomerIPFilter.Where(p => p.CustomerId == customerid).Include(p => p.tblCustomerIPFilterDetails).FirstOrDefault();
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
          return  context.tblCustomerMarkup.Where(p => p.CustomerId == markupid).FirstOrDefault();
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
