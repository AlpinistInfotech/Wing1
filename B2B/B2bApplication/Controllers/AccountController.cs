using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace B2bApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _context;
        public AccountController(ILogger<HomeController> logger, DBContext context)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            mdlLogin mdl = new mdlLogin();
            return View(mdl);
        }
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromServices]IAccount account, mdlLogin mdl, string? ReturnUrl)
        {
            string IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            if (ModelState.IsValid)
            {
                try {
                    var userMaster = await account.LoginAsync(mdl, IPAddress);
                    var userRoles = await account.GetEnmDocumentsAsync(userMaster.Id);
                    var userClaims = new List<Claim>();
                    foreach (var usr in userRoles)
                    {
                        userClaims.Add(new Claim(nameof(usr), usr.ToString()));
                    }
                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                    await HttpContext.SignInAsync(userPrincipal);
                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                } catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(mdl);
        }
    }

    
}
