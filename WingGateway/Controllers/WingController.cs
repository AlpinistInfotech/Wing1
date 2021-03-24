using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WingGateway.Models;

namespace WingGateway.Controllers
{
    [Authorize]
    public class WingController : Controller
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public WingController(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
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
        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_BankDetails))]
        public IActionResult BankDetails(mdlFilterModel mdl, enmLoadData submitdata)
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
            WingGateway.Classes.ConsProfile consProfile = new Classes.ConsProfile(_context, _config);
            returnData.TcBankWrapers = consProfile.GetBankDetails(submitdata, mdl, 0, false);
            returnData.FilterModel = mdl;
            return View(returnData);
        }


        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_BankApproval))]
        public IActionResult BankApproval()
        {
            mdlTcBankApprovalWraper returnDataMdl = new mdlTcBankApprovalWraper();
            returnDataMdl.approval= new mdlApprovalForm() { };
            return View(returnDataMdl);
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
        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_PANDetails))]
        public IActionResult PANDetails(mdlFilterModel mdl, enmLoadData submitdata)
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
            WingGateway.Classes.ConsProfile consProfile = new Classes.ConsProfile(_context, _config);
            returnData.TcPANWrapers = consProfile.GetPANDetails(submitdata, mdl, 0, false);
            returnData.FilterModel = mdl;
            return View(returnData);
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
        [Authorize(policy: nameof(enmDocumentMaster.Emp_Tc_Details))]
        public IActionResult TCDetails(mdlFilterModel mdl, enmLoadData submitdata)
        {
            mdlTcReportWraper returnData = new mdlTcReportWraper();
            WingGateway.Classes.ConsProfile consProfile = new Classes.ConsProfile(_context, _config);
            returnData.TcWrapers = consProfile.GetTCDetails(submitdata, mdl, 0,0, false);
            returnData.FilterModel = mdl;
            return View(returnData);
        }



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
                List<string> AllFileName = new List<string>();

                bool exists = System.IO.Directory.Exists(path_holidaypackageimages);
                if (!exists)
                    System.IO.Directory.CreateDirectory(path_holidaypackageimages);

                foreach (var file in mdl.UploadPackageImage)
                {
                    var filename = Guid.NewGuid().ToString() + ".jpeg";
                    using (var stream = new FileStream(string.Concat(path_holidaypackageimages, filename), FileMode.Create))
                    {
                        AllFileName.Add(filename);
                        await file.CopyToAsync(stream);
                    }
                }

                foreach (var file in mdl.UploadOtherImage)
                {
                    var filename = Guid.NewGuid().ToString() + ".jpeg";
                    using (var stream = new FileStream(string.Concat(path_holidayotherimages, filename), FileMode.Create))
                    {
                        AllFileName.Add(filename);
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
                    ExistingData.lastModifiedBy = 0;
                    ExistingData.LastModifieddate = DateTime.Now;
                    _context.tblHolidayPackageMaster.Update(ExistingData);
                    _context.SaveChanges();
                    return RedirectToAction("HolidayPackageMaster",
                                     new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.UpdateSucessfully });

                }


                else
                {
                    _context.tblHolidayPackageMaster.Add(new tblHolidayPackageMaster
                    {
                    PackageName = mdl.PackageName ,
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
                        CreatedBy = 0,
                        CreatedDt = DateTime.Now,
                        Isdeleted = false,
                    });
                    _context.SaveChanges();
                    return RedirectToAction("HolidayPackageMaster",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.SaveSucessfully });
                }

            }

            return View(mdl);
        }

        #endregion
    }
}
