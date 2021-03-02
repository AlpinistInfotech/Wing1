using Database;
using Database.Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WingGateway.Models;

namespace WingGateway.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DBContext _context;        
        
        public HomeController(ILogger<HomeController> logger,UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager ,DBContext context)
        {
            _logger = logger;
            this._userManager = userManager;
            this._signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        
        public IActionResult Login([FromServices]ICaptchaGenratorBase captchaGenratorBase)
        {   
            mdlCaptcha mdC = new mdlCaptcha();
            mdC.GenrateCaptcha(captchaGenratorBase);
            mdlLogin mdl = new mdlLogin()
            {
                CaptchaData=mdC
            };
            return View(mdl);
        }
        
        [HttpPost]
        public IActionResult Login([FromServices] ICaptchaGenratorBase captchaGenratorBase, mdlLogin mdl)
        {
            mdlCaptcha mdC = new mdlCaptcha();
            mdC.GenrateCaptcha(captchaGenratorBase);            
            return View();
        }

        
        [HttpGet]
        public IActionResult Registration([FromServices] ICaptchaGenratorBase captchaGenratorBase)
        {
            mdlCaptcha mdC = new mdlCaptcha();
            mdC.GenrateCaptcha(captchaGenratorBase);
            mdlRegistration mdl = new mdlRegistration()
            {
                CaptchaData = mdC
            };
            return View(mdl);
        }

        
        [HttpPost]
        public async Task<IActionResult> RegistrationAsync([FromServices] ICaptchaGenratorBase captchaGenratorBase,
                            [FromServices]ISequenceMaster sequenceMaster ,
                            [FromServices] IConsolidatorProfile consolidatorProfile,
                            [FromForm] mdlRegistration mdl)
        {
            if (!captchaGenratorBase.verifyCaptch(mdl.CaptchaData.SaltId, mdl.CaptchaData.CaptchaCode))
            {
                ModelState.AddModelError(mdl.CaptchaData.CaptchaCode, "Invalid Captcha");
            }            
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try {
                    mdl.GenrateRegistration(sequenceMaster, consolidatorProfile, _context);
                    ApplicationUser appuser = new ApplicationUser()
                    {
                        UserName = mdl.TcId,
                        Email = mdl.EmailAddress,
                        TcNid = mdl.TcNId
                    };
                    var result= await _userManager.CreateAsync(appuser, mdl.Password);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(appuser, isPersistent: false);
                        return RedirectToAction("Index","home");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", ex.Message);
                }


            }
            return View(mdl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
