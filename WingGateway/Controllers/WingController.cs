using Database;
using Database.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WingGateway.Classes;
using WingGateway.Models;

namespace WingGateway.Controllers
{
    [Authorize]
    public class WingController : Controller
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        private readonly ICurrentUsers _currentUsers;
        
        public WingController(DBContext context, IConfiguration config, ICurrentUsers currentUsers)
        {
            _context = context;
            _config = config;
            _currentUsers = currentUsers;
        }

        [Authorize(policy:nameof( enmDocumentMaster.Emp_Dashboard))]
        public IActionResult Index()
        {
            return View();
        }


        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_BankDetails))]
        public IActionResult BankDetails()
        {
            mdlTcBankReportWraper returnDataMdl = new mdlTcBankReportWraper();
            returnDataMdl.FilterModel = new mdlFilterModel() {dateFilter= new mdlDateFilter() , idFilter=new mdlIdFilter(),IsReport=true };
            returnDataMdl.TcBankWrapers = new List<mdlTcBankWraper>();
            return View(returnDataMdl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_BankDetails))]
        public IActionResult BankDetails(mdlFilterModel mdl, enmLoadData submitdata, [FromServices]IConsProfile consProfile)
        {
            mdlTcBankReportWraper returnData = new mdlTcBankReportWraper();
            if (mdl.dateFilter == null)
            {
                mdl.dateFilter = new mdlDateFilter();
            }
            if (mdl.idFilter == null)
            {
                mdl.idFilter = new mdlIdFilter();
            }
            mdl.dateFilter.FromDt =Convert.ToDateTime( mdl.dateFilter.FromDt.ToString("dd-MMM-yyyy"));
            mdl.dateFilter.ToDt = Convert.ToDateTime(mdl.dateFilter.ToDt.AddDays(1).ToString("dd-MMM-yyyy"));            
            returnData.TcBankWrapers = consProfile.GetBankDetails(submitdata, mdl, 0, false);
            returnData.FilterModel = mdl;
            return View(returnData);
        }


        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_BankApproval))]
        public IActionResult BankApproval(enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }
            mdlTcBankWraper mdl = new mdlTcBankWraper();
            ModelState.Clear();
            return View(mdl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_BankApproval))]
        public async Task<IActionResult> BankApprovalAsync(mdlTcBankWraper mdl, string submitdata, [FromServices] IConsProfile consProfile)
        {
            try
            {
                ModelState.Clear();
                if (submitdata == "LoadData")
                {   
                    if (mdl.TcId == "")
                    {
                        ModelState.AddModelError(nameof(mdl.TcId), "TcId Required");
                    }
                    else
                    {
                        mdl = consProfile.GetBankDetails(enmLoadData.ByID, new mdlFilterModel() { idFilter = new mdlIdFilter() { TcId = mdl.TcId } }, 0, true).FirstOrDefault();
                    }
                    return View(mdl);
                }
                else if (submitdata == "Approve" || submitdata == "Reject")
                {
                    bool HaveModelError = false;
                    if (mdl.DetailId == 0)
                    {
                        HaveModelError = true;
                        ModelState.AddModelError("", "Invalid Data");
                    }
                    if (submitdata == "Reject" && (string.IsNullOrWhiteSpace(mdl.ApprovalRemarks)))
                    {
                        HaveModelError = true;
                        ModelState.AddModelError(nameof(mdl.ApprovalRemarks), "Remarks Required");
                    }
                    
                    if (!HaveModelError)
                    {
                        var data=_context.tblTcBankDetails.Where(p => p.DetailId == mdl.DetailId).FirstOrDefault();
                        if (data == null)
                        {
                            HaveModelError = true;
                            ModelState.AddModelError("", "Invalid Data");
                        }
                        else
                        {
                            data.IsApproved = submitdata == "Approve" ? enmApprovalType.Approved : enmApprovalType.Rejected;
                            data.ApprovalRemarks = mdl.ApprovalRemarks;
                            data.ApprovedBy = _currentUsers.EmpId;
                            data.ApprovedDt = DateTime.Now;
                            _context.Update(data);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("BankApproval",
                                 new { _enmSaveStatus = enmSaveStatus.success,  _enmMessage = submitdata == "Approve" ? enmMessage.ApprovedSucessfully: enmMessage.RejectSucessfully });

                        }
                    }
                    if (HaveModelError)
                    {   
                        ViewBag.SaveStatus = (int)enmSaveStatus.danger;
                        ViewBag.Message = enmMessage.InvalidData;                        
                    }

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            if (mdl == null)
            {
                mdl = new mdlTcBankWraper();
            }
            return View(mdl);
        }


        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_PANDetails))]
        public IActionResult PANDetails()
        {
            mdlTcPANReportWraper returnDataMdl = new mdlTcPANReportWraper();
            returnDataMdl.FilterModel = new mdlFilterModel() { dateFilter = new mdlDateFilter(), idFilter = new mdlIdFilter(), IsReport = true };
            returnDataMdl.TcPANWrapers = new List<mdlTcPANWraper>();
            return View(returnDataMdl);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_PANDetails))]
        public IActionResult PANDetails(mdlFilterModel mdl, enmLoadData submitdata, [FromServices] IConsProfile consProfile)
        {
            mdlTcPANReportWraper returnData = new mdlTcPANReportWraper();
            if (mdl.dateFilter == null)
            {
                mdl.dateFilter = new mdlDateFilter();
            }
            if (mdl.idFilter == null)
            {
                mdl.idFilter = new mdlIdFilter();
            }
            mdl.dateFilter.FromDt = Convert.ToDateTime(mdl.dateFilter.FromDt.ToString("dd-MMM-yyyy"));
            mdl.dateFilter.ToDt = Convert.ToDateTime(mdl.dateFilter.ToDt.AddDays(1).ToString("dd-MMM-yyyy"));
            returnData.TcPANWrapers = consProfile.GetPANDetails(submitdata, mdl, 0, false);
            returnData.FilterModel = mdl;
            return View(returnData);
        }

        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_PANApproval))]
        public IActionResult PANApproval(enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }
            mdlTcPANWraper mdl = new mdlTcPANWraper();
            ModelState.Clear();
            return View(mdl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_PANApproval))]
        public async Task<IActionResult> PANApprovalAsync(mdlTcPANWraper mdl, string submitdata, [FromServices] IConsProfile consProfile)
        {
            try
            {
                ModelState.Clear();
                if (submitdata == "LoadData")
                {
                    if (mdl.TcId == "")
                    {
                        ModelState.AddModelError(nameof(mdl.TcId), "TcId Required");
                    }
                    else
                    {
                        mdl = consProfile.GetPANDetails(enmLoadData.ByID, new mdlFilterModel() { idFilter = new mdlIdFilter() { TcId = mdl.TcId } }, 0, true).FirstOrDefault();
                    }
                    return View(mdl);
                }
                else if (submitdata == "Approve" || submitdata == "Reject")
                {
                    bool HaveModelError = false;
                    if (mdl.DetailId == 0)
                    {
                        HaveModelError = true;
                        ModelState.AddModelError("", "Invalid Data");
                    }
                    if (submitdata == "Reject" && (string.IsNullOrWhiteSpace(mdl.ApprovalRemarks)))
                    {
                        HaveModelError = true;
                        ModelState.AddModelError(nameof(mdl.ApprovalRemarks), "Remarks Required");
                    }

                    if (!HaveModelError)
                    {
                        var data = _context.TblTcPanDetails.Where(p => p.DetailId == mdl.DetailId).FirstOrDefault();
                        if (data == null)
                        {
                            HaveModelError = true;
                            ModelState.AddModelError("", "Invalid Data");
                        }
                        else
                        {
                            data.IsApproved = submitdata == "Approve" ? enmApprovalType.Approved : enmApprovalType.Rejected;
                            data.ApprovalRemarks = mdl.ApprovalRemarks;
                            data.ApprovedBy = _currentUsers.EmpId;
                            data.ApprovedDt = DateTime.Now;
                            _context.Update(data);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("PANApproval",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = submitdata == "Approve" ? enmMessage.ApprovedSucessfully : enmMessage.RejectSucessfully });

                        }
                    }
                    if (HaveModelError)
                    {
                        ViewBag.SaveStatus = (int)enmSaveStatus.danger;
                        ViewBag.Message = enmMessage.InvalidData;
                    }

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            if (mdl == null)
            {
                mdl = new mdlTcPANWraper();
            }
            return View(mdl);
        }



        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_Details))]
        public IActionResult TCDetails()
        {
            mdlTcReportWraper returnDataMdl = new mdlTcReportWraper();
            returnDataMdl.FilterModel = new mdlFilterModel() { dateFilter = new mdlDateFilter(), idFilter = new mdlIdFilter(), IsReport = true };
            returnDataMdl.TcWrapers = new List<ProcRegistrationSearch>();
            return View(returnDataMdl);
        }

        


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_Details))]
        public IActionResult TCDetails(mdlFilterModel mdl, enmLoadData submitdata, [FromServices] IConsProfile consProfile)
        {
            mdlTcReportWraper returnData = new mdlTcReportWraper();
            if (mdl.dateFilter == null)
            {
                mdl.dateFilter = new mdlDateFilter();
            }
            if (mdl.idFilter == null)
            {
                mdl.idFilter = new mdlIdFilter();
            }
            mdl.dateFilter.FromDt = Convert.ToDateTime(mdl.dateFilter.FromDt.ToString("dd-MMM-yyyy"));
            mdl.dateFilter.ToDt = Convert.ToDateTime(mdl.dateFilter.ToDt.AddDays(1).ToString("dd-MMM-yyyy"));
            returnData.TcWrapers = consProfile.GetTCDetails(submitdata, mdl, 0,0, false);
            returnData.FilterModel = mdl;
            return View(returnData);
        }

        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_Approval))]
        public IActionResult TCApproval(enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }
            ProcRegistrationSearch mdl = new ProcRegistrationSearch();
            ModelState.Clear();
            return View(mdl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_Approval))]
        public async Task<IActionResult> TCApprovalAsync(ProcRegistrationSearch mdl, string submitdata, [FromServices] IConsProfile consProfile)
        {
            try
            {
                ModelState.Clear();
                if (submitdata == "LoadData")
                {
                    if (mdl.TCID == "")
                    {
                        ModelState.AddModelError(nameof(mdl.TCID), "TcId Required");
                    }
                    else
                    {
                        mdl = consProfile.GetTCDetails(enmLoadData.ByID, new mdlFilterModel() { idFilter = new mdlIdFilter() { TcId = mdl.TCID } }, 0,0, true).FirstOrDefault();
                    }
                    return View(mdl);
                }
                else if (submitdata == "Approve" || submitdata == "Reject")
                {
                    bool HaveModelError = false;
                    if (mdl.tcnid == 0)
                    {
                        HaveModelError = true;
                        ModelState.AddModelError("", "Invalid Data");
                    }
                    if (submitdata == "Reject" && (string.IsNullOrWhiteSpace(mdl.approve_remarks)))
                    {
                        HaveModelError = true;
                        ModelState.AddModelError(nameof(mdl.approve_remarks), "Remarks Required");
                    }

                    if (!HaveModelError)
                    {
                        var data = _context.tblRegistration.Where(p => p.Nid== mdl.tcnid).FirstOrDefault();
                        if (data == null)
                        {
                            HaveModelError = true;
                            ModelState.AddModelError("", "Invalid Data");
                        }
                        else
                        {
                            tblTCStatus tblTCstatus = new tblTCStatus()
                            {
                                action_remarks = mdl.approve_remarks,
                                TcNid = mdl.tcnid,
                                action = submitdata == "Approve" ? enmApprovalType.Approved : enmApprovalType.Rejected,
                                action_type = (enmTCStatus)1,
                                action_by = _currentUsers.EmpId,
                                action_datetime = DateTime.Now,
                            };

                            _context.tblTCStatus.Add(tblTCstatus);

                            data.is_active = submitdata == "Approve" ? enmApprovalType.Approved : enmApprovalType.Rejected;
                            _context.Update(data);
                            
                            await _context.SaveChangesAsync();
                            return RedirectToAction("TCApproval",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = submitdata == "Approve" ? enmMessage.ApprovedSucessfully : enmMessage.RejectSucessfully });

                        }
                    }
                    if (HaveModelError)
                    {
                        ViewBag.SaveStatus = (int)enmSaveStatus.danger;
                        ViewBag.Message = enmMessage.InvalidData;
                    }

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            if (mdl == null)
            {
                mdl = new ProcRegistrationSearch();
            }
            return View(mdl);
        }


        #region Add Wallet

        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Add_Wallet))]
        public IActionResult AddWallet(enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            mdlWallet mdl = new mdlWallet();
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }

            return View(mdl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Add_Wallet))]
        public async Task<IActionResult> AddWalletAsync([FromServices] ICurrentUsers currentUsers, mdlWallet mdl,[FromServices] IConsolidatorProfile consolidator)
        {
            if (ModelState.IsValid)
            {
                int TcNiD = 0;
                if (mdl.SpTcId != null && mdl.SpTcId.Length > 0)
                {
                    TcNiD = consolidator.GetNId(mdl.SpTcId, true);
                    if (TcNiD == 0)
                    {
                        ModelState.AddModelError("", "Invalid TC ID !!!");
                    }
                }

                decimal credit_amt = mdl.TransactionType == (enmWalletTransactiontype)1 ? mdl.WalletAmt : 0;
                decimal debit_amt = mdl.TransactionType == (enmWalletTransactiontype)2 ? mdl.WalletAmt : 0;
                DateTime todaydate = Convert.ToDateTime(  DateTime.Now.ToString("dd-MMM-yyyy") );
                var data = _context.tblTCwalletlog.Where(p => p.TcNid == TcNiD && p.credit == credit_amt && p.debit == debit_amt && p.createddatetime >= todaydate && p.createddatetime <= todaydate.AddDays(1)).FirstOrDefault();
                if (data != null)
                {
                    ModelState.AddModelError("", "Same Date with same Amount entry already exists !!!");
                    return View(mdl);
                }



                var ExistingData = _context.tblTCwallet.FirstOrDefault(p => p.TcNid ==TcNiD);
                if (ExistingData != null)
                {
                    ExistingData.TcNid = TcNiD;
                    ExistingData.walletamt = ExistingData.walletamt + mdl.WalletAmt;
                    _context.tblTCwallet.Update(ExistingData);
                }
                else
                {
                    _context.tblTCwallet.Add(new tblTCWallet
                    {
                        TcNid = TcNiD,
                        walletamt = mdl.WalletAmt,
                    });
                }
                _context.tblTCwalletlog.Add(new tblTCWalletLog()
                {
                    TcNid = TcNiD,
                    credit = credit_amt,
                    debit = debit_amt,
                    createdby = currentUsers.EmpId,
                    createddatetime = DateTime.Now,
                    remarks = mdl.Remarks,
                    reqno = 0,
                    groupid = 0,
                });

                await _context.SaveChangesAsync();
                    return RedirectToAction("AddWallet",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.SaveSucessfully });
                }

            

            return View(mdl);
        }


        #endregion

        #region Wallet Statement

        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Wallet_Statement))]
        public IActionResult WalletStatement()
        {
            mdlTcWalletReportWraper returnDataMdl = new mdlTcWalletReportWraper();
            returnDataMdl.mdlTcWalletWraper = new List<ProcWalletSearch>();
            return View(returnDataMdl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Wallet_Statement))]
        public IActionResult WalletStatement(mdlTcWalletReportWraper mdl, enmLoadData submitdata, [FromServices] IConsProfile consProfile)
        {
            mdl.mdlTcWalletWraper = consProfile.GetTCWalletStatement(mdl, 0, 0, true);
            return View(mdl);
        }


        #endregion

        #region Holiday Package


        [AcceptVerbs("Get")]

        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Holiday_Package))]
        public IActionResult HolidayPackageNew([FromServices] ICurrentUsers currentUsers, enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            mdlHolidayPackage mdl = new mdlHolidayPackage();
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }

            return View();
        }

        [HttpPost]
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Holiday_Package))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HolidayPackageNewAsync([FromServices] ICurrentUsers currentUsers, mdlHolidayPackage mdl)
        {
            string filePath_HolidayPackageImage = _config["FileUpload:HolidayPackageImage"];

            var path_holidaypackageimages = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + filePath_HolidayPackageImage);


            string filePath_HolidayOtherImage = _config["FileUpload:HolidayOtherImage"];

            var path_holidayotherimages = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + filePath_HolidayOtherImage);


            if (mdl.UploadPackageImage == null || mdl.UploadPackageImage.Count == 0 || mdl.UploadPackageImage[0] == null || mdl.UploadPackageImage[0].Length == 0)
            {
                ModelState.AddModelError("IDDocumentUpload", "Invalid Files");
                ViewBag.SaveStatus = enmSaveStatus.danger;
                ViewBag.Message = enmMessage.InvalidData.GetDescription();
            }

            if (ModelState.IsValid)
            {
                List<string> AllFileName_packageimages = new List<string>();
                List<string> AllFileName_otherimages = new List<string>();

                bool exists = System.IO.Directory.Exists(path_holidaypackageimages);
                if (!exists)
                    System.IO.Directory.CreateDirectory(path_holidaypackageimages);

                foreach (var file in mdl.UploadPackageImage)
                {
                    var filename = Guid.NewGuid().ToString() + ".jpeg";
                    using (var stream = new FileStream(string.Concat(path_holidaypackageimages, filename), FileMode.Create))
                    {
                        AllFileName_packageimages.Add(filename);
                        await file.CopyToAsync(stream);
                    }
                }

                foreach (var file in mdl.UploadOtherImage)
                {
                    var filename = Guid.NewGuid().ToString() + ".jpeg";
                    using (var stream = new FileStream(string.Concat(path_holidayotherimages, filename), FileMode.Create))
                    {
                        AllFileName_otherimages.Add(filename);
                        await file.CopyToAsync(stream);
                    }
                }

                var ExistingData = _context.tblHolidayPackageMaster.FirstOrDefault(p => !p.Isdeleted && p.DetailId == 0);
                if (ExistingData != null)
                {

                    ExistingData.PackageName = mdl.PackageName;
                    ExistingData.PackageType = mdl.PackageType;
                    ExistingData.PackageFromDate = mdl.PackageFromDate;
                    ExistingData.PackageToDate = mdl.PackageToDate;
                    ExistingData.PriceFrom = mdl.PriceFrom;
                    ExistingData.PriceTo = mdl.PriceTo;
                    ExistingData.MemberCount = mdl.MemberCount;
                    ExistingData.DaysCount = mdl.DaysCount;
                    ExistingData.country_id = mdl.country_id;
                    ExistingData.state_id = mdl.state_id;
                    ExistingData.PackageDescription = mdl.PackageDescription;
                    ExistingData.SpecialNote = mdl.SpecialNote;
                    ExistingData.lastModifiedBy = currentUsers.EmpId;
                    ExistingData.LastModifieddate = DateTime.Now;
                    ExistingData.UploadPackageImage = string.Join<string>(",", AllFileName_packageimages);
                    ExistingData.UploadOtherImage = string.Join<string>(",", AllFileName_otherimages);
                    ExistingData.is_active = mdl.is_active;
                    _context.tblHolidayPackageMaster.Update(ExistingData);
                    _context.SaveChanges();
                    return RedirectToAction("HolidayPackageNew",
                                     new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.UpdateSucessfully });

                }


                else
                {
                    _context.tblHolidayPackageMaster.Add(new tblHolidayPackageMaster
                    {
                        PackageName = mdl.PackageName,
                        PackageType = mdl.PackageType,
                        PackageFromDate = mdl.PackageFromDate,
                        PackageToDate = mdl.PackageToDate,
                        PriceFrom = mdl.PriceFrom,
                        PriceTo = mdl.PriceTo,
                        MemberCount = mdl.MemberCount,
                        DaysCount = mdl.DaysCount,
                        country_id = mdl.country_id,
                        state_id = mdl.state_id,
                        PackageDescription = mdl.PackageDescription,
                        SpecialNote = mdl.SpecialNote,
                        UploadPackageImage = string.Join<string>(",", AllFileName_packageimages),
                        UploadOtherImage = string.Join<string>(",", AllFileName_otherimages),
                        CreatedBy = currentUsers.EmpId,
                        CreatedDt = DateTime.Now,
                        Isdeleted = false,
                        is_active=0,
                    });
                    _context.SaveChanges();
                    return RedirectToAction("HolidayPackageNew",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.SaveSucessfully });
                }

            }

            return View(mdl);
        }

        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Holiday_Package_Report))]
        public IActionResult HolidayPackageReport()
        {
            mdlHolidayPackageReportWraper returnDataMdl = new mdlHolidayPackageReportWraper();
            returnDataMdl.FilterModel = new mdlFilterModel() { dateFilter = new mdlDateFilter(), idFilter = new mdlIdFilter(), IsReport = true };
            returnDataMdl.HolidayPackageWrapers = new List<ProcHolidayPackageSearch>();
            return View(returnDataMdl);
        }
        [HttpPost]
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Holiday_Package_Report))]
        public IActionResult HolidayPackageReport(mdlFilterModel mdl, enmLoadData submitdata)
        {
            mdlHolidayPackageReportWraper returnData = new mdlHolidayPackageReportWraper();
            if (mdl.dateFilter == null)
            {
                mdl.dateFilter = new mdlDateFilter();
            }
            if (mdl.idFilter == null)
            {
                mdl.idFilter = new mdlIdFilter();
            }
            mdl.dateFilter.FromDt = Convert.ToDateTime(mdl.dateFilter.FromDt.ToString("dd-MMM-yyyy"));
            mdl.dateFilter.ToDt = Convert.ToDateTime(mdl.dateFilter.ToDt.AddDays(1).ToString("dd-MMM-yyyy"));
            WingGateway.Classes.ConsProfile consProfile = new Classes.ConsProfile(_context, _config);
            returnData.HolidayPackageWrapers = consProfile.GetHolidayPackageDetails(submitdata, mdl, 0,0, false);
            returnData.FilterModel = mdl;
            return View(returnData);
        }

        #endregion
    }
}
