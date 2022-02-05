using B2C.Classes;
using B2C.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace B2C.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccount _account;
        public AccountController(IAccount account)
        {
            _account = account;
        }

        public IActionResult Index()
        {
            return View();
        }

        private void LoadCaptcha(ref mdlLogin mdl)
        {
            var CaptchaData = _account.LoadCaptcha();
            if (CaptchaData.messageType == enmMessageType.Success)
            {
                mdl.CaptchaId = CaptchaData.returnId.captchaId;
                mdl.CaptchaImage = CaptchaData.returnId.captchaImage;
                mdl.TempUserId = CaptchaData.returnId.tempUserId;
            }
        }

        public IActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            mdlLogin mdl = new mdlLogin();
            LoadCaptcha(ref mdl);
            return View(mdl);
        }
        [HttpPost]
        public async Task<IActionResult> LoginAsync(mdlLogin mdl, string? ReturnUrl)
        {
            string IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            if (ModelState.IsValid)
            {
                try
                {
                    var loginResponse= _account.LoginAsync(mdl);
                    if (loginResponse.messageType == enmMessageType.Success)
                    {
                        //var userMaster = await account.LoginAsync(mdl, IPAddress);
                        //var userRoles = await account.GetEnmDocumentsAsync(userMaster.Id);
                        var userClaims = new List<Claim>();

                        userClaims.Add(new Claim("_Name", loginResponse.returnId.normalizedName));
                        userClaims.Add(new Claim("__Email", loginResponse.returnId.email));
                        userClaims.Add(new Claim("__Contact", loginResponse.returnId.phoneNumber));
                        userClaims.Add(new Claim("__CustomerId", loginResponse.returnId.customerId.ToString()));
                        userClaims.Add(new Claim("__UserId", loginResponse.returnId.userId.ToString()));
                        userClaims.Add(new Claim("__DistributorId", loginResponse.returnId.distributorId.ToString()));
                        userClaims.Add(new Claim("__CustomerType", loginResponse.returnId.customerType.ToString()));
                        userClaims.Add(new Claim("__Token", loginResponse.returnId.token));

                        var grandmaIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                        if (!string.IsNullOrEmpty(ReturnUrl))
                        {
                            return LocalRedirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        LoadCaptcha(ref mdl);
                        ModelState.AddModelError("", loginResponse.message);
                    }
                }
                catch (Exception ex)
                {
                    LoadCaptcha(ref mdl);
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(mdl);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}
