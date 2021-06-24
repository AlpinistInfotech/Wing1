using B2BApis.Model;
using B2BClasses;
using B2BClasses.Services.Air;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HomeController : ControllerBase
    {
        IConfiguration _config;
        public HomeController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [Route("SearchFlight")]
        [HttpPost]
        public async Task<mdlBookingSearchApi> SearchFlight([FromServices] IBooking booking, mdlSearchRequest mdl)
        {
            mdlBookingSearchApi bookingsearch = null;
            string IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            if (ModelState.IsValid)
            {
                try
                {
                    bookingsearch = (mdlBookingSearchApi)await booking.SearchFlight(mdl);
                    //bookingsearch.TokenID= GenerateJSONWebToken(userMaster);
                    bookingsearch.StatusCode = 1;
                    bookingsearch.StatusMessage = "Sucess";
                }
                catch (Exception ex)
                {
                    bookingsearch.StatusCode = 0;
                    bookingsearch.StatusMessage = ex.Message;
                }
            }
            return bookingsearch;

        }
    }
}
