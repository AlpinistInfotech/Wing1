using Database;
using Database.Classes;
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
        private readonly DBContext _context;
        public HomeController(ILogger<HomeController> logger, DBContext context)
        {
            _logger = logger;
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

        [Route("Login")]
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
        [Route("Login")]
        [HttpPost]
        public IActionResult Login([FromServices] ICaptchaGenratorBase captchaGenratorBase, mdlLogin mdl)
        {
            mdlCaptcha mdC = new mdlCaptcha();
            mdC.GenrateCaptcha(captchaGenratorBase);            
            return View();
        }

        [Route("Register")]
        public IActionResult Register([FromServices] ICaptchaGenratorBase captchaGenratorBase)
        {
            mdlCaptcha mdC = new mdlCaptcha();
            mdC.GenrateCaptcha(captchaGenratorBase);
            mdlRegistration mdl = new mdlRegistration()
            {
                CaptchaData = mdC
            };
            return View(mdl);
        }

        [Route("Register")]
        [HttpPost]
        public IActionResult Register([FromServices]IUserManager , [FromServices] ICaptchaGenratorBase captchaGenratorBase,[FromForm] mdlRegistration mdl)
        {
            if (_context.ApplicationUser.Where(p => (p.Email == mdl.EmailAddress) && !p.IsTerminated).Any())
            {
                ModelState.AddModelError(mdl.EmailAddress, "Already Exists");
            }
            if (_context.ApplicationUser.Where(p => (p.Email == mdl.EmailAddress || p.PhoneNumber == mdl.PhoneNo) && !p.IsTerminated).Any())
            {
                ModelState.AddModelError(mdl.PhoneNo, "Already Exists");
            }
            if (ModelState.IsValid)
            {
                
                ApplicationUser applicationUser = new ApplicationUser();
                applicationUser.UserName= 
                _context.ApplicationUser
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
