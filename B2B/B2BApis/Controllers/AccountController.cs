using B2BApis.Model;
using B2BClasses;
using B2BClasses.Models;
using B2BClasses.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using B2BClasses.Services.Enums;
using IdentityModel.Client;

namespace B2BApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IConfiguration _config;
        public AccountController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [Route("Login")]
        [HttpPost]
        public async Task<mdlUserMasterApi> LoginAsync([FromServices] IAccount account, mdlLogin mdl)
        {

            mdlUserMasterApi userMaster = new mdlUserMasterApi();
            string IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            if (ModelState.IsValid)
            {
                try
                {
                    tblUserMaster tbl = await account.LoginAsync(mdl, IPAddress);
                    if (tbl.CustomerId != null)
                    {
                        userMaster.TokenData =await GenerateJSONWebTokenAsync(new mdlTookenRequest() { CustomerId =tbl.CustomerId ?? 0,UserId= tbl.Id,customerType= tbl.tblCustomerMaster.CustomerType, Name=tbl.tblCustomerMaster.CustomerName } );
                        userMaster.StatusCode = 1;
                        userMaster.StatusMessage = "Success";
                    }
                    else
                    {
                        userMaster.TokenData = "";
                        userMaster.StatusCode = 0;
                        userMaster.StatusMessage = "Invalid Customer Details";
                    }
                }
                catch (Exception ex)
                {
                    userMaster.TokenData = "";
                    userMaster.StatusCode = 0;
                    userMaster.StatusMessage = ex.Message;
                }
            }
            return userMaster;
        }


        

        [Route("GenrateToken")]
        [HttpPost]
        public async Task<string> GenerateJSONWebTokenAsync(mdlTookenRequest request )
        {
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> _claim = new List<Claim>();
            _claim.Add(new Claim("_CustomerId", request.CustomerId.ToString()));
            _claim.Add(new Claim("_UserId", request.UserId.ToString()));
            _claim.Add(new Claim("_CustomerType", ((int)request.customerType).ToString()));
            _claim.Add(new Claim("_Name", request.Name ??""));
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:audience"],
              _claim,
              expires: DateTime.Now.AddHours(Convert.ToInt32(_config["Jwt:tokenExpireinhour"])),
              signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
             
            
        }

    }
}
