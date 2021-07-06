
using Database;
using Database.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WingGateway.Models;

namespace WingGateway.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;        
        private readonly DBContext _context;
        private readonly ISettings _setting;
        private readonly IConfiguration _config;
        private readonly ICurrentUsers _currentUsers;
        private readonly int _userid;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public HomeController(ILogger<HomeController> logger,DBContext context, IConfiguration config, ICurrentUsers currentUsers, ISettings setting, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
            )
        {
            _logger = logger;            
            _context = context;
            _config = config;
            _setting = setting;
            _currentUsers = currentUsers;
            _userid = _currentUsers.EmpId;
            this._userManager = userManager;
            this._signInManager = signInManager;

        }



        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Dashboard))]
        public IActionResult Index()
        {
            mdlFilterModel mdl = new mdlFilterModel();
            enmLoadData loadType = new enmLoadData();
            mdlHolidayPackageReportWraper returnData = new mdlHolidayPackageReportWraper();
            WingGateway.Classes.ConsProfile consProfile = new Classes.ConsProfile(_context, _config);
            returnData.HolidayPackageWrapers = consProfile.GetHolidayPackageDetails(loadType, mdl, 0, 0, false);
            returnData.AirlinesAPIList  = new APIData();
            returnData.AirlinesAPIList.APIURL = _config["AirlinesAPIList:APIURL"];
            returnData.AirlinesAPIList.AirportSearch = _config["AirlinesAPIList:AirportSearch"];

            return View(returnData);

        }

        [Authorize]
        public dynamic GetMenu([FromServices] ICurrentUsers currentUsers)
        {
            var roles = _context.UserRoles.Where(p => p.UserId == currentUsers.userId).Select(p => p.RoleId).ToList();
            var AllClaims = _context.RoleClaims.Where(p => roles.Contains(p.RoleId)).Select(p => p.ClaimType).ToList();
            List<Document> alldocument = new List<Document>();
            List<Module> allModule = new List<Module>();
            List<SubModule> allSubModule = new List<SubModule>();
            var eDms = Enum.GetValues(typeof(enmDocumentMaster)).Cast<enmDocumentMaster>().ToList();
            eDms.RemoveAll(p => !AllClaims.Contains(p.ToString()));
            foreach (var doc in eDms)
            {
                var DD = doc.GetDocumentDetails();
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


            var _document = alldocument.Select(p => new { id = p.Id, name = p.Name, subModuleId = p.EnmSubModule.HasValue ? p.EnmSubModule.Value : 0,
                moduleId = p.EnmModule.HasValue ? p.EnmModule.Value : 0,
                displayOrder = p.DisplayOrder,
                actionName = p.ActionName,
                icon = p.Icon
            }).OrderBy(p => p.displayOrder);
            var _subModule =allSubModule.Select(p => new { id = p.Id, name = p.Name,
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

            var returndata = new{_document, _subModule , _module };
            return returndata;

        }

        [Authorize]
        public async Task<IActionResult> FlightSearch()
        {
            //mdlFlightSearch flightSearch = new mdlFlightSearch()
            //{
            //    FlightSearchWraper = new mdlFlightSearchWraper()
            //    {
            //        AdultCount = 1,
            //        ChildCount = 0,
            //        InfantCount = 0,
            //        CabinClass = enmCabinClass.ECONOMY,
            //        DepartureDt = DateTime.Now,
            //        ReturnDt = null,
            //        From = "DEL",
            //        To = "BOM",
            //        JourneyType = enmJourneyType.OneWay,

            //    }
            //};
            //return View(flightSearch);
            return View();
        }



        [Authorize(policy: nameof(enmDocumentMaster.Emp_Dashboard))]
        public IActionResult Privacy()
        {
            return View();
        }

        
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [Authorize(policy: nameof(enmDocumentMaster.Gateway_UploadKyc))]
        public IActionResult UploadKyc([FromServices] ICurrentUsers currentUsers,enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            mdlKyc mdl = new mdlKyc();
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }
            string filePath = _config["FileUpload:Kyc"];

            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + filePath);
            var kycMaster = _context.tblKycMaster.Where(p => p.TcNid == currentUsers.TcNid && !p.Isdeleted).FirstOrDefault();
            if (kycMaster !=null)
            {
                mdl.ApprovalRemarks=kycMaster.ApprovalRemarks;
                mdl.IsApproved = kycMaster.IsApproved;
                mdl.IdProofType = kycMaster.IdProofType;
                mdl.DocumentNo = kycMaster.IdDocumentNo;
                mdl.Remarks = kycMaster.Remarks;
                mdl.fileData = new List<byte[]>();
                var files=kycMaster.IdDocumentName.Split(",");
                foreach (var file in files)
                {
                    mdl.fileData.Add(System.IO.File.ReadAllBytes(string.Concat(path, file)));
                }
            }
            return View(mdl);
        }
        
        [HttpPost]
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_UploadKyc))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadKycAsync([FromServices] ICurrentUsers currentUsers, mdlKyc mdl)
        {
            
            string filePath = _config["FileUpload:Kyc"];
            
            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/"+ filePath);
            if (mdl.IDDocumentUpload == null || mdl.IDDocumentUpload.Count==0 || mdl.IDDocumentUpload[0]==null || mdl.IDDocumentUpload[0].Length==0)
            {
                ModelState.AddModelError("IDDocumentUpload", "Invalid Files");
                ViewBag.SaveStatus = enmSaveStatus.danger;
                ViewBag.Message = enmMessage.InvalidData.GetDescription();
            }
            if (mdl.IdProofType == enmIdentityProof.Aadhar)
            {
                if (mdl.DocumentNo.Trim().Length != 12)
                {
                    ModelState.AddModelError("DocumentNo", "Invalid Aadhar Number");
                    ViewBag.SaveStatus = enmSaveStatus.danger;
                    ViewBag.Message = enmMessage.InvalidData.GetDescription();
                }
                else if (!mdl.DocumentNo.All(char.IsDigit))
                {
                    ModelState.AddModelError("DocumentNo", "Invalid Aadhar Number");
                    ViewBag.SaveStatus = enmSaveStatus.danger;
                    ViewBag.Message = enmMessage.InvalidData.GetDescription();
                }
                
            }
            if (ModelState.IsValid)
            {
                List<string> AllFileName = new List<string>();
                
                bool exists = System.IO.Directory.Exists(path);
                if (!exists)
                    System.IO.Directory.CreateDirectory(path);

                foreach (var file in mdl.IDDocumentUpload)
                {
                    var filename = Guid.NewGuid().ToString()+".jpeg";
                    using (var stream = new FileStream(string.Concat( path, filename), FileMode.Create))
                    {
                        AllFileName.Add(filename);
                        await file.CopyToAsync(stream);
                    }
                }

                var ExistingData=_context.tblKycMaster.FirstOrDefault(p => !p.Isdeleted && p.TcNid == currentUsers.TcNid && p.IsApproved == enmApprovalType.Rejected);
                if (ExistingData != null)
                {
                    ExistingData.Isdeleted =true;
                    _context.tblKycMaster.Update(ExistingData);
                }
                if (_context.tblKycMaster.Any(p => p.TcNid == currentUsers.TcNid && !p.Isdeleted))
                {
                    ModelState.AddModelError("", "Request Already Submited");
                    ViewBag.SaveStatus = enmSaveStatus.warning;
                    ViewBag.Message = enmMessage.AlreadyExists.GetDescription();
                }
                else
                {
                    _context.tblKycMaster.Add(new tblKycMaster
                    {
                        IdProofType = mdl.IdProofType,
                        IdDocumentNo = mdl.DocumentNo,
                        IdDocumentName = string.Join<string>(",", AllFileName),
                        CreatedBy = 0,
                        CreatedDt = DateTime.Now,
                        Remarks = mdl.Remarks,
                        IsApproved = enmApprovalType.Pending,
                        Isdeleted = false,
                        TcNid=currentUsers.TcNid,
                        ApprovalRemarks=""
                    });
                    _context.SaveChanges();
                    return RedirectToAction("UploadKyc",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.UpdateSucessfully });
                }


            }

            return View(mdl);
        }


        [AcceptVerbs("Get", "Post")]
        public IActionResult IsAccountNoInUse(string AccountNo, int BankId)
        {
            var users =  _context.tblTcBankDetails.Where(p=>p.BankId== BankId && p.AccountNo==AccountNo && !p.Isdeleted).FirstOrDefault();
            if (users == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Account No. {AccountNo} is already in use");
            }
        }

        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Bank))]
        public IActionResult Bank([FromServices] ICurrentUsers currentUsers, enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            mdlBank mdl = new mdlBank();
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }
            string filePath = _config["FileUpload:Bank"];

            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + filePath);
            var masterData = _context.tblTcBankDetails.Where(p => p.TcNid == currentUsers.TcNid && !p.Isdeleted).FirstOrDefault();
            if (masterData != null)
            {
                mdl.ApprovalRemarks = masterData.ApprovalRemarks;
                mdl.IsApproved = masterData.IsApproved;
                mdl.BankId = masterData.BankId.HasValue? masterData.BankId.Value:0;
                mdl.IFSC = masterData.IFSC;
                mdl.AccountNo = masterData.AccountNo;
                mdl.Remarks = masterData.Remarks;
                mdl.BranchAddress = masterData.BranchAddress;
                mdl.NameasonBank = masterData.NameasonBank;
                mdl.fileData = new List<byte[]>();

                var files = masterData.UploadImages.Split(",");
                foreach (var file in files)
                {
                    mdl.fileData.Add(System.IO.File.ReadAllBytes(string.Concat(path, file)));
                }
                ViewBag.BankList = new SelectList(mdl.GetBanks(_context,false), "BankId", "BankName", mdl.BankId);
            }
            else
            {
                ViewBag.BankList = new SelectList(mdl.GetBanks(_context, true), "BankId", "BankName");
            }
            return View(mdl);
        }

        [HttpPost]
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Bank))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BankAsync([FromServices] ICurrentUsers currentUsers, mdlBank mdl)
        {

            string filePath = _config["FileUpload:Bank"];

            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + filePath);
            if (mdl.UploadImages == null || mdl.UploadImages.Count == 0 || mdl.UploadImages[0] == null || mdl.UploadImages[0].Length == 0)
            {
                ModelState.AddModelError("IDDocumentUpload", "Invalid Files");
                ViewBag.SaveStatus = enmSaveStatus.danger;
                ViewBag.Message = enmMessage.InvalidData.GetDescription();
            } 
            if (ModelState.IsValid)
            {
                List<string> AllFileName = new List<string>();

                bool exists = System.IO.Directory.Exists(path);
                if (!exists)
                    System.IO.Directory.CreateDirectory(path);

                foreach (var file in mdl.UploadImages)
                {
                    var filename = Guid.NewGuid().ToString() + ".jpeg";
                    using (var stream = new FileStream(string.Concat(path, filename), FileMode.Create))
                    {
                        AllFileName.Add(filename);
                        await file.CopyToAsync(stream);
                    }
                }

                var ExistingData = _context.tblTcBankDetails.FirstOrDefault(p => !p.Isdeleted && p.TcNid == currentUsers.TcNid && p.IsApproved == enmApprovalType.Rejected);
                if (ExistingData != null)
                {
                    ExistingData.Isdeleted = true;
                    _context.tblTcBankDetails.Update(ExistingData);
                }
                if (_context.tblTcBankDetails.Any(p => p.TcNid == currentUsers.TcNid && !p.Isdeleted))
                {
                    ModelState.AddModelError("", "Request Already Submited");
                    ViewBag.SaveStatus = enmSaveStatus.warning;
                    ViewBag.Message = enmMessage.AlreadyExists.GetDescription();
                }
                else
                {
                    _context.tblTcBankDetails.Add(new tblTcBankDetails
                    {
                        BankId = mdl.BankId,
                        IFSC = mdl.IFSC,
                        AccountNo = mdl.AccountNo,
                        UploadImages = string.Join<string>(",", AllFileName),
                        CreatedBy = 0,
                        CreatedDt = DateTime.Now,
                        Remarks = mdl.Remarks,
                        IsApproved = enmApprovalType.Pending,
                        Isdeleted = false,
                        TcNid = currentUsers.TcNid,
                        ApprovalRemarks = "",
                        BranchAddress = mdl.BranchAddress,
                        NameasonBank=mdl.NameasonBank
                    }) ;
                    _context.SaveChanges();
                    return RedirectToAction("Bank",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.UpdateSucessfully });
                }

            }

            ViewBag.BankList = new SelectList(mdl.GetBanks(_context, true), "BankId", "BankName");
            return View(mdl);
        }

        #region PAN
        [AcceptVerbs("Get", "Post")]
        public IActionResult IsPANNoInUse(string PANNo, int PANId)
        {
            var users = _context.TblTcPanDetails.Where(p => p.PANNo == PANNo && !p.Isdeleted).FirstOrDefault();
            if (users == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"PAN No {PANNo} is already in use");
            }
        }

        [Authorize(policy: nameof(enmDocumentMaster.Gateway_PAN))]
        public IActionResult PAN([FromServices] ICurrentUsers currentUsers, enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            mdlPAN mdl = new mdlPAN();
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }
            string filePath = _config["FileUpload:PAN"];

            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + filePath);
            var masterData = _context.TblTcPanDetails.Where(p => p.TcNid == currentUsers.TcNid && !p.Isdeleted).FirstOrDefault();
            if (masterData != null)
            {
                mdl.ApprovalRemarks = masterData.ApprovalRemarks;
                mdl.IsApproved = masterData.IsApproved;
                mdl.PanName= masterData.PANName;

                mdl.PANNo = masterData.PANNo;
                mdl.Remarks = masterData.Remarks;                
                mdl.fileData = new List<byte[]>();
                var files = masterData.UploadImages.Split(",");
                foreach (var file in files)
                {
                    mdl.fileData.Add(System.IO.File.ReadAllBytes(string.Concat(path, file)));
                }
               
            }
            
            return View(mdl);
        }

        [HttpPost]
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_PAN))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PANAsync([FromServices] ICurrentUsers currentUsers, mdlPAN mdl)
        {

            string filePath = _config["FileUpload:PAN"];

            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot/" + filePath);
            if (mdl.UploadImages == null || mdl.UploadImages.Count == 0 || mdl.UploadImages[0] == null || mdl.UploadImages[0].Length == 0)
            {
                ModelState.AddModelError("IDDocumentUpload", "Invalid Files");
                ViewBag.SaveStatus = enmSaveStatus.danger;
                ViewBag.Message = enmMessage.InvalidData.GetDescription();
            }
            
            var supportedTypes = new[] {"jpg", "jpeg", "png"};
            var fileExt = System.IO.Path.GetExtension(mdl.UploadImages[0].FileName).Substring(1);
            if (!supportedTypes.Contains(fileExt))
            {
                ModelState.AddModelError("IDDocumentUpload","Invalid File Extension - Only Upload jpg/jpeg/png File");
                ViewBag.SaveStatus = enmSaveStatus.danger;
                ViewBag.Message = enmMessage.InvalidData.GetDescription();
            }

            if (ModelState.IsValid)
            {
                List<string> AllFileName = new List<string>();

                bool exists = System.IO.Directory.Exists(path);
                if (!exists)
                    System.IO.Directory.CreateDirectory(path);

                foreach (var file in mdl.UploadImages)
                {
                    var filename = Guid.NewGuid().ToString() + "."+fileExt;
                    using (var stream = new FileStream(string.Concat(path, filename), FileMode.Create))
                    {
                        AllFileName.Add(filename);
                        await file.CopyToAsync(stream);
                    }
                }

                var ExistingData = _context.TblTcPanDetails.FirstOrDefault(p => !p.Isdeleted && p.TcNid == currentUsers.TcNid && p.IsApproved == enmApprovalType.Rejected);
                if (ExistingData != null)
                {
                    ExistingData.Isdeleted = true;
                    _context.TblTcPanDetails.Update(ExistingData);
                }
                if (_context.TblTcPanDetails.Any(p => p.TcNid == currentUsers.TcNid && !p.Isdeleted))
                {
                    ModelState.AddModelError("", "Request Already Submited");
                    ViewBag.SaveStatus = enmSaveStatus.warning;
                    ViewBag.Message = enmMessage.AlreadyExists.GetDescription();
                }
                else
                {
                    _context.TblTcPanDetails.Add(new tblTcPanDetails
                    {

                        PANNo = mdl.PANNo,
                        PANName = mdl.PanName,
                        UploadImages = string.Join<string>(",", AllFileName),
                        CreatedBy = 0,
                        CreatedDt = DateTime.Now,
                        Remarks = mdl.Remarks,
                        IsApproved = enmApprovalType.Pending,
                        Isdeleted = false,
                        TcNid = currentUsers.TcNid,
                        ApprovalRemarks = ""
                       
                    });
                    _context.SaveChanges();
                    return RedirectToAction("PAN",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.UpdateSucessfully });
                }

            }

            return View(mdl);
        }


        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Tree))]
        public IActionResult Tree()
        {
            return View();
        }


        #endregion

        #region Nominee
        

        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Nominee))]
        public IActionResult Nominee([FromServices] ICurrentUsers currentUsers, enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            mdlNominee mdl = new mdlNominee();
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }
            
            var masterData = _context.TblTcNominee.Where(p => p.TcNid == currentUsers.TcNid && !p.Isdeleted).FirstOrDefault();
            if (masterData != null)
            {
                mdl.NomineeName = masterData.NomineeName;
                mdl.NomineeRelation = masterData.NomineeRelation;
                mdl.Remarks = masterData.Remarks;
                
            }

            return View(mdl);
        }

        [HttpPost]
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Nominee))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NomineeAsync([FromServices] ICurrentUsers currentUsers, mdlNominee mdl)
        {
            if (ModelState.IsValid)
            {
            
                var ExistingData = _context.TblTcNominee.FirstOrDefault(p => !p.Isdeleted && p.TcNid == currentUsers.TcNid );
                if (ExistingData != null) // for update the data
                {
                    //ExistingData.Isdeleted = true;
                    ExistingData.NomineeName = mdl.NomineeName;
                    ExistingData.NomineeRelation = mdl.NomineeRelation;
                    ExistingData.Remarks = mdl.Remarks;
                    ExistingData.lastModifiedBy = currentUsers.TcNid;
                    ExistingData.LastModifieddate = DateTime.Now;
                    _context.TblTcNominee.Update(ExistingData);
                    _context.SaveChanges();
                    return RedirectToAction("Nominee",
                                     new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.UpdateSucessfully });

                }

                //if (_context.TblTcNominee.Any(p => p.TcNid == currentUsers.TcNid && !p.Isdeleted))
                //{
                //    ModelState.AddModelError("", "request already submited");
                //    ViewBag.savestatus = enmSaveStatus.warning;
                //    ViewBag.message = enmMessage.AlreadyExists.GetDescription();
                //}
                else
                {
                    _context.TblTcNominee.Add(new tblTcNominee
                    {

                        NomineeName = mdl.NomineeName,
                        NomineeRelation = mdl.NomineeRelation,
                        CreatedBy = currentUsers.TcNid,
                        CreatedDt = DateTime.Now,
                        Remarks = mdl.Remarks,
                        Isdeleted = false,
                        TcNid = currentUsers.TcNid,
                        
                    });
                    _context.SaveChanges();
                    return RedirectToAction("Nominee",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.SaveSucessfully });
                }

            }

            return View(mdl);
        }




        #endregion


        #region Contact

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> ValidateContactNo(string MobileNo, [FromServices] ICurrentUsers currentuser)
        {
            var users = _context.TblTcContact.FirstOrDefault(p => p.MobileNo == MobileNo && p.TcNid != currentuser.TcNid);
            if (users == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Mobile No. {MobileNo} is already in use");
            }
            
        }


        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Contact))]
        public IActionResult Contact([FromServices] ICurrentUsers currentUsers, enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            mdlContact mdl = new mdlContact();
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }

            var masterData = _context.TblTcContact.Where(p => p.TcNid == currentUsers.TcNid && !p.Isdeleted).FirstOrDefault();
            if (masterData != null)
            {
                mdl.MobileNo = masterData.MobileNo;
                mdl.AlternateMobileNo = masterData.AlternateMobileNo;
            }
            

            return View(mdl);
        }

        [HttpPost]
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Contact))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactAsync([FromServices] ICurrentUsers currentUsers, mdlContact mdl)
        {
            if (ModelState.IsValid)
            {

                var ExistingData = _context.TblTcContact.FirstOrDefault(p => !p.Isdeleted && p.TcNid == currentUsers.TcNid);
                if (ExistingData != null) // for update the data
                {
                    //ExistingData.Isdeleted = true;
                    ExistingData.MobileNo = mdl.MobileNo;
                    ExistingData.AlternateMobileNo = mdl.AlternateMobileNo;
                    ExistingData.lastModifiedBy = currentUsers.TcNid;
                    ExistingData.LastModifieddate = DateTime.Now;
                    _context.TblTcContact.Update(ExistingData);
                    _context.SaveChanges();
                    return RedirectToAction("Contact",
                                     new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.UpdateSucessfully });

                }

                else
                {
                    _context.TblTcContact.Add(new tblTcContact
                    {

                        MobileNo = mdl.MobileNo,
                        AlternateMobileNo = mdl.AlternateMobileNo,
                        CreatedBy = currentUsers.TcNid,
                        CreatedDt = DateTime.Now,
                        Isdeleted = false,
                        TcNid = currentUsers.TcNid,

                    });
                    _context.SaveChanges();
                    return RedirectToAction("Contact",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.SaveSucessfully });
                }

            }

            return View(mdl);
        }



        #endregion


        #region Email

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> ValidateEmailID(string emailid, [FromServices] ICurrentUsers currentuser)
        {
            var users = _context.TblTcEmail.FirstOrDefault(p => p.EmailID == emailid && p.TcNid != currentuser.TcNid);
            if (users == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email ID {emailid} is already in use");
            }

        }


        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Email))]
        public IActionResult Email([FromServices] ICurrentUsers currentUsers, enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            mdlEmail mdl = new mdlEmail();
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }

            var masterData = _context.TblTcEmail.Where(p => p.TcNid == currentUsers.TcNid && !p.Isdeleted).FirstOrDefault();
            if (masterData != null)
            {
                mdl.EmailID = masterData.EmailID;
                mdl.AlternateEmailID = masterData.AlternateEmailID;
            }


            return View(mdl);
        }

        [HttpPost]
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Email))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailAsync([FromServices] ICurrentUsers currentUsers, mdlEmail mdl)
        {
            if (ModelState.IsValid)
            {

                var ExistingData = _context.TblTcEmail.FirstOrDefault(p => !p.Isdeleted && p.TcNid == currentUsers.TcNid);
                if (ExistingData != null) // for update the data
                {
                    ExistingData.EmailID= mdl.EmailID;
                    ExistingData.AlternateEmailID= mdl.AlternateEmailID;
                    ExistingData.lastModifiedBy = currentUsers.TcNid;
                    ExistingData.LastModifieddate = DateTime.Now;
                    _context.TblTcEmail.Update(ExistingData);
                    _context.SaveChanges();
                    return RedirectToAction("Email",
                                     new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.UpdateSucessfully });

                }

                else
                {
                    _context.TblTcEmail.Add(new tblTcEmail
                    {

                        EmailID = mdl.EmailID,
                        AlternateEmailID = mdl.AlternateEmailID,
                        CreatedBy = currentUsers.TcNid,
                        CreatedDt = DateTime.Now,
                        Isdeleted = false,
                        TcNid = currentUsers.TcNid,

                    });
                    _context.SaveChanges();
                    return RedirectToAction("Email",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.SaveSucessfully });
                }

            }

            return View(mdl);
        }



        #endregion

        #region MarkUp

        [AcceptVerbs("Get")]
        
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Markup))]
        public IActionResult MarkUp([FromServices] ICurrentUsers currentUsers, enmSaveStatus? _enmSaveStatus, enmMessage? _enmMessage)
        {
            mdlMarkUp mdl = new mdlMarkUp();
            if (_enmSaveStatus != null)
            {
                ViewBag.SaveStatus = (int)_enmSaveStatus.Value;
                ViewBag.Message = _enmMessage?.GetDescription();
            }

            return View();
        }



        
            [HttpPost]
        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Markup))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkUpAsync([FromServices] ICurrentUsers currentUsers, mdlMarkUp mdl)
        {
            if (ModelState.IsValid)
            {

                var ExistingData = _context.TblTcMarkUp.FirstOrDefault(p => !p.Isdeleted && p.TcNid == currentUsers.TcNid && p.BookingType== mdl.BookingType);
                if (ExistingData != null) // for update the data
                {
                    ExistingData.MarkupValue = Convert.ToDecimal( mdl.markupValue);
                    ExistingData.BookingType = mdl.BookingType;
                    ExistingData.lastModifiedBy = currentUsers.TcNid;
                    ExistingData.LastModifieddate = DateTime.Now;
                    _context.TblTcMarkUp.Update(ExistingData);
                    _context.SaveChanges();
                    return RedirectToAction("MarkUp",
                                     new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.UpdateSucessfully });

                }

                else
                {
                    _context.TblTcMarkUp.Add(new tblTcMarkUp
                    {

                        MarkupValue  =mdl.markupValue,
                        BookingType = mdl.BookingType,
                        CreatedBy = currentUsers.TcNid,
                        CreatedDt = DateTime.Now,
                        Isdeleted = false,
                        TcNid = currentUsers.TcNid,

                    });
                    _context.SaveChanges();
                    return RedirectToAction("MarkUp",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.SaveSucessfully });
                }

            }

            return View(mdl);
        }



        #endregion


        #region ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(mdlChangePassword mdl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return ViewBag.Message = "No User Exist";
                }
                var result = await _userManager.ChangePasswordAsync(user, mdl.Password, mdl.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }
                await _signInManager.RefreshSignInAsync(user);
                return View("/Home/ChangePasswordConfirmation");
            }
            return View(mdl);
        }

        #endregion

    }
}
