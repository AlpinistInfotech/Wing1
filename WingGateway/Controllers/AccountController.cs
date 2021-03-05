﻿using Database;
using Database.Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WingGateway.Models;

namespace WingGateway.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DBContext _context;


        public AccountController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DBContext context)
        {
            _logger = logger;
            this._userManager = userManager;
            this._signInManager = signInManager;
            _context = context;
        }
        public IActionResult Login([FromServices] ICaptchaGenratorBase captchaGenratorBase)
        {
            mdlCaptcha mdC = new mdlCaptcha();
            mdC.GenrateCaptcha(captchaGenratorBase);
            mdlLogin mdl = new mdlLogin()
            {
                CaptchaData = mdC
            };
            return View(mdl);
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromServices] ICaptchaGenratorBase captchaGenratorBase, mdlLogin mdl, string returnUrl)
        {
            mdlCaptcha mdC = new mdlCaptcha();
            if (!captchaGenratorBase.verifyCaptch(mdl.CaptchaData.SaltId, mdl.CaptchaData.CaptchaCode))
            {
                ModelState.AddModelError(mdl.CaptchaData.CaptchaCode, "Invalid Captcha");
            }
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(mdl.Username, mdl.Password, mdl.RememberMe, true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
                ModelState.AddModelError("", "Invalid login attempts");
            }


            return View(mdl);
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

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsSponsorValid(string SpTcId, [FromServices] IConsolidatorProfile consolidatorProfile)
        {
            if (SpTcId != null)
            {
                var validSp =consolidatorProfile.ValidateSponsorId(SpTcId, true);

                return Json(validSp);
            }
            
            return Json(true);
            
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string EmailAddress)
        {
            var users = await _userManager.FindByEmailAsync(EmailAddress);
            if (users == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {EmailAddress} is already in use");
            }
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult IsMobileInUse(string PhoneNo)
        {
            var users = _context.Users.FirstOrDefault(p => p.PhoneNumber == PhoneNo);
            if (users == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Phone {PhoneNo} is already in use");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrationAsync([FromServices] ICaptchaGenratorBase captchaGenratorBase,
                            [FromServices] ISequenceMaster sequenceMaster,
                            [FromServices] IConsolidatorProfile consolidatorProfile,
                            [FromServices] RoleManager<IdentityRole> RoleManager,
                            [FromForm] mdlRegistration mdl)
        {
            if (mdl.CaptchaData == null)
            {
                ModelState.AddModelError(mdl.CaptchaData.CaptchaCode, "Invalid Captcha");
            }
            else if (!captchaGenratorBase.verifyCaptch(mdl.CaptchaData.SaltId, mdl.CaptchaData.CaptchaCode))
            {
                ModelState.AddModelError(mdl.CaptchaData.CaptchaCode, "Invalid Captcha");
            }
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    mdl.GenrateRegistration(sequenceMaster, consolidatorProfile, _context);
                    ApplicationUser appuser = new ApplicationUser()
                    {
                        UserName = mdl.TcId,
                        Email = mdl.EmailAddress,
                        TcNid = mdl.TcNId,
                        PhoneNumber =mdl.PhoneNo,
                        UserType = enmUserType.Consolidator

                    };
                    var result = await _userManager.CreateAsync(appuser, mdl.Password);
                    var role = await RoleManager.FindByNameAsync("TC");
                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(appuser, "TC");
                    }

                    if (result.Succeeded)
                    {   
                        await _signInManager.SignInAsync(appuser, isPersistent: false);
                        transaction.Commit();
                        return RedirectToAction("Wing", "Dashboard");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    transaction.Rollback();


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", ex.Message);
                }


            }
            return View(mdl);
        }


        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();            
            return RedirectToAction("Login");
        }
    }
}
