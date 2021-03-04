using Database;
using Database.Classes;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(policy: nameof(enmDocumentMaster.Gateway_Dashboard))]
        public IActionResult Index()
        {
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
    }
}
