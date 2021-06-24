using B2BApis.Model;
using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Services.Air;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace B2BApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HomeController : ControllerBase
    {
        IConfiguration _config;
        int _customerId;
        int _userId;
        int isvalid = 0;

        public HomeController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [Route("SearchFlight")]
        [HttpPost]
        public async Task<mdlBookingSearchApi> SearchFlight([FromServices] IBooking booking, mdlSearchRequest mdl)
        {
            mdlBookingSearchApi bookingsearch = new mdlBookingSearchApi();

            if (ModelState.IsValid)
            {
                try
                { 
                    isvalid = ValidateToken();
                    if (isvalid == 0 || _customerId<=0 || _userId<=0)
                    {
                        bookingsearch.StatusCode = 0;
                        bookingsearch.StatusMessage = "Invalid Token Data";
                    }
                    else
                    {
                        booking.CustomerId = _customerId;
                        booking.UserId = _userId;
                           await booking.SearchFlight(mdl);
                        bookingsearch.StatusCode = 1;
                        bookingsearch.StatusMessage = "Success";
                    }
                }
                catch (Exception ex)
                {
                    bookingsearch.StatusCode = 0;
                    bookingsearch.StatusMessage = ex.Message;
                }
            }
            return bookingsearch;

        }

        private int ValidateToken()
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                string authHeader = Request.Headers["Authorization"];
                authHeader = authHeader.Replace("Bearer ", "");
                var jsonToken = handler.ReadToken(authHeader);
                var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
                _customerId = Convert.ToInt32( tokenS.Claims.First(claim => claim.Type == "__customerId").Value);
                _userId = Convert.ToInt32(tokenS.Claims.First(claim => claim.Type == "__UserId").Value);
                isvalid = 1;
            }
            catch {
                isvalid = 0;
            }
            return isvalid;
        }
    }
}
