using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WingApi.Classes.Database;
using WingApi.Models;

namespace WingApi.Controllers
{
    

    public class AccountController : Controller
    {
        private readonly DBContext _context;
        private readonly IJwtTokenGenerator tokenGenerator;
        public AccountController(DBContext context, IJwtTokenGenerator tokenGenerator)
        {
            _context = context;
            this.tokenGenerator = tokenGenerator;
        }

        public IActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;            
            mdlLogin mdl = new mdlLogin();
            return View(mdl);
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(mdlLogin mdl, string? returnUrl=null)
        {
            ViewBag.returnUrl = returnUrl;
            var returnTo = returnUrl??"/Home/Index";

            
            if (ModelState.IsValid)
            {
                var customer=_context.tblCustomerMaster.Where(p => p.Code == mdl.CustomerCode).FirstOrDefault();
                if (customer == null || !customer.IsActive)
                {
                    ModelState.AddModelError(nameof(mdl.CustomerCode), "Invalid Customer");
                    return View(mdl);
                }


                var userMaster=_context.tblUserMaster.Where(p => p.UserName.Trim().ToLower() == mdl.Username.Trim().ToLower() && p.CustomerId == customer.Id).FirstOrDefault();
                if (userMaster==null || userMaster.Password!= mdl.Password || !userMaster.IsActive)
                {
                    ModelState.AddModelError(nameof(mdl.Username), "Invalid Username/Password");
                    return View(mdl);
                }

                List<Claim> ur = _context.tblUserRole.Where(p => p.UserId == userMaster.Id).ToList();

                var myClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.GivenName, authenticatedUser.FirstName),
                    new Claim(ClaimTypes.Surname, authenticatedUser.LastName),
                    new Claim("HasAdminRights", authenticatedUser.HasAdminRights ? "Y" : "N")
                };

                var accessTokenResult = tokenGenerator.GenerateAccessTokenWithClaimsPrincipal(
                    mdl.Username, userMaster.Id, userMaster.CustomerId,
                    AddMyClaims(userInfo));
                await HttpContext.SignInAsync(accessTokenResult.ClaimsPrincipal,
                    accessTokenResult.AuthProperties);


                return RedirectToLocal(returnTo);
            }
            //mdl.CaptchaData.GenrateCaptcha(captchaGenratorBase);
            return View(mdl);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

    }
}
