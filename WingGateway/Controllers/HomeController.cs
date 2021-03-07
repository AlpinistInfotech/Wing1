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
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger,DBContext context, IConfiguration config)
        {
            _logger = logger;            
            _context = context;
            _config = config;
        }



        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Dashboard))]
        public IActionResult Index()
        {
            return View();
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
                alldocument.Add(doc.GetDocumentDetails());
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
            if (mdl.IdProofType == enmIdentityProof.Adhar)
            {
                if (mdl.DocumentNo.Trim().Length != 12)
                {
                    ModelState.AddModelError("DocumentNo", "Invalid Adhar Number");
                    ViewBag.SaveStatus = enmSaveStatus.danger;
                    ViewBag.Message = enmMessage.InvalidData.GetDescription();
                }
                else if (!mdl.DocumentNo.All(char.IsDigit))
                {
                    ModelState.AddModelError("DocumentNo", "Invalid Adhar Number");
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

                var ExistingData=_context.tblKycMaster.FirstOrDefault(p => !p.Isdeleted && p.TcNid == currentUsers.TcNid && p.IsApproved == enmApprovalType.Rejeceted);
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
                return Json($"Account No {AccountNo} is already in use");
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

                var ExistingData = _context.tblTcBankDetails.FirstOrDefault(p => !p.Isdeleted && p.TcNid == currentUsers.TcNid && p.IsApproved == enmApprovalType.Rejeceted);
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
                        BranchAddress = mdl.BranchAddress
                    }) ;
                    _context.SaveChanges();
                    return RedirectToAction("Bank",
                                 new { _enmSaveStatus = enmSaveStatus.success, _enmMessage = enmMessage.UpdateSucessfully });
                }

            }

            ViewBag.BankList = new SelectList(mdl.GetBanks(_context, true), "BankId", "BankName");
            return View(mdl);
        }
    }
}
